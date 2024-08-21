package repositoryanalyzer

import (
	"context"
	"fmt"
	"os"
	"os/exec"
	"path/filepath"
	"strings"
	"sync"

	"github.com/hashicorp/terraform-config-inspect/tfconfig"
	"github.com/mholt/archiver/v4"
	"github.com/sirupsen/logrus"
	"hcl-dataset.dev/repo-analyzer/database"
	"hcl-dataset.dev/repo-analyzer/utils"
)

func ProcessRepositories(db *database.Database, workerCount int) error {
	err := os.MkdirAll("repositories", os.ModeDir|0755)
	if err != nil {
		logrus.Errorf("Failed to create repository directory: %v", err)
		return err
	}

	// this is the dispatcher for the workers
	repoCount, repoChan := db.GetRedistributableRepositories()
	logrus.Infof("Processing %v repositories with %v workers", repoCount, workerCount)

	jobs := make(chan job, repoCount)
	results := make(chan result, repoCount)

	var jobSync sync.WaitGroup
	jobSync.Add(workerCount)
	for w := 1; w <= workerCount; w++ {
		go repoProcessor(w, jobs, results, &jobSync)
	}

	var resultSync sync.WaitGroup
	resultSync.Add(1)
	go resultProcessor(db, results, &resultSync)

	for repo := range repoChan {
		jobs <- job{repo: repo}
	}
	close(jobs)

	jobSync.Wait()
	close(results)

	resultSync.Wait()

	return nil
}

type job struct {
	repo database.Repository
}

type result struct {
	repo    database.Repository
	modules []resultModule
}

type resultModule struct {
	module           database.Module
	diagnostics      *string
	managedResources []database.Resource
	dataResources    []database.Resource
}

func repoProcessor(workerID int, jobs <-chan job, results chan<- result, sync *sync.WaitGroup) {
	defer sync.Done()
	workLogger := logrus.WithField("worker_id", workerID)
	for job := range jobs {
		// to process a repository, first clone it, then delete the .git folder.
		// then recursively search for all directories that contain *.tf files.
		// all of those directories are terraform modules.
		// create a resulting module with information and return the result for storage.

		repoLogger := workLogger.WithField("repository_id", job.repo.ID)

		repo := job.repo
		repoLogger.Infof("Processing repository %v", repo.ID)

		// download the repo
		os.RemoveAll(fmt.Sprintf("repositories/%v", repo.ID))
		repoLogger.Infof("Cloning %s", repo.CloneURL)
		cmd := exec.Command("git", "clone", "--depth=1", repo.CloneURL, fmt.Sprintf("repositories/%v", repo.ID))
		out, err := cmd.CombinedOutput()
		if err != nil {
			repoLogger.Errorf("Failed to clone repository: %v %v", string(out), err)
			continue
		}

		cmd = exec.Command("git", "rev-parse", "HEAD")
		cmd.Dir = fmt.Sprintf("repositories/%v", repo.ID)
		out, err = cmd.CombinedOutput()
		if err != nil {
			repoLogger.Errorf("Failed to get latest commit SHA: %v %v", string(out), err)
			repo.LatestCommitSha = "unknown"
		} else {
			repo.LatestCommitSha = strings.TrimSuffix(string(out), "\n")
		}

		// remove the .git folder
		os.RemoveAll(fmt.Sprintf("repositories/%v/.git", repo.ID))

		// search all folders that contain at least 1 tf file.
		moduleDirectories := []string{}
		err = filepath.WalkDir(fmt.Sprintf("repositories/%v", repo.ID), func(path string, d os.DirEntry, err error) error {
			if err != nil {
				return err
			}

			if d.IsDir() {
				return nil
			}

			dirPath := filepath.Dir(path)
			if utils.ArrayContains(moduleDirectories, dirPath) {
				return nil
			}

			if filepath.Ext(path) == ".tf" {
				moduleDirectories = append(moduleDirectories, dirPath)
			}

			return nil
		})

		if err != nil {
			repoLogger.Errorf("Failed to iterate / search repo dir: %v", err)
			continue
		}
		repoLogger.Debugf("Found %v module directories", len(moduleDirectories))

		// analyze the modules
		resultModules := make([]resultModule, len(moduleDirectories))
		for k, moduleDir := range moduleDirectories {
			module, diag := tfconfig.LoadModule(moduleDir)
			if diag.HasErrors() {
				repoLogger.Warnf("Diagnostic messages: %v", diag.Err())
			}

			relPath, _ := filepath.Rel(fmt.Sprintf("repositories/%v", repo.ID), moduleDir)
			res := resultModule{
				module: database.Module{
					RepositoryID: repo.ID,
					Path:         relPath,
					Providers:    utils.MapGetAllKeys(module.RequiredProviders),
					ModuleCalls: utils.MapToArray(module.ModuleCalls, func(k string, v *tfconfig.ModuleCall) database.ModuleCall {
						return database.ModuleCall{
							Name:   k,
							Source: v.Source,
						}
					}),
				},
				managedResources: utils.MapToArray(module.ManagedResources, func(_ string, v *tfconfig.Resource) database.Resource {
					return database.Resource{
						ResourceType: "Managed",
						Name:         v.Name,
						Type:         v.Type,
						Provider:     v.Provider.Name,
					}
				}),
				dataResources: utils.MapToArray(module.DataResources, func(_ string, v *tfconfig.Resource) database.Resource {
					return database.Resource{
						ResourceType: "Data",
						Name:         v.Name,
						Type:         v.Type,
						Provider:     v.Provider.Name,
					}
				}),
			}

			if diag.HasErrors() {
				diagStr := diag.Error()
				res.diagnostics = &diagStr
			}

			resultModules[k] = res
		}

		repoLogger.Infof("Analyzed %v modules", len(resultModules))

		zipFiles, err := archiver.FilesFromDisk(nil, map[string]string{
			fmt.Sprintf("repositories/%v/", repo.ID): "",
		})
		if err != nil {
			repoLogger.Errorf("Failed to create files from disk: %v", err)
			continue
		}

		os.RemoveAll(fmt.Sprintf("repositories/%v.tar.gz", repo.ID))
		zipOut, err := os.Create(fmt.Sprintf("repositories/%v.tar.gz", repo.ID))
		if err != nil {
			repoLogger.Errorf("Failed to create tar gz file: %v", err)
			continue
		}
		err = archiver.CompressedArchive{
			Compression: archiver.Gz{},
			Archival:    archiver.Tar{},
		}.Archive(context.Background(), zipOut, zipFiles)
		if err != nil {
			repoLogger.Errorf("Failed to compress archive: %v", err)
			continue
		}
		zipOut.Close()

		repoLogger.Infof("Created TAR GZ file %v", fmt.Sprintf("repositories/%v.tar.gz", repo.ID))

		os.RemoveAll(fmt.Sprintf("repositories/%v", repo.ID))
		repoLogger.Infof("Removed repository directory %v", fmt.Sprintf("repositories/%v", repo.ID))

		results <- result{
			repo:    repo,
			modules: resultModules,
		}
	}
}

func resultProcessor(db *database.Database, results <-chan result, sync *sync.WaitGroup) {
	defer sync.Done()
	for result := range results {
		logger := logrus.WithField("repository_id", result.repo.ID)
		err := db.UpdateCommitSHA(result.repo.ID, result.repo.LatestCommitSha)
		if err != nil {
			logger.Errorf("Failed to update commit SHA: %v", err)
		} else {
			logger.Infof("Updated commit SHA %v", result.repo.LatestCommitSha)
		}
		for _, obj := range result.modules {
			err := db.StoreAnalysis(obj.module, obj.diagnostics, obj.managedResources, obj.dataResources)
			if err != nil {
				logger.Errorf("Failed to store module: %v", err)
			} else {
				logger.Infof("Stored module %v", obj.module.Path)
			}
		}
	}
}
