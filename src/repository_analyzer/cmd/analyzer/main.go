package main

import (
	"io"
	"os"
	"runtime"
	"strconv"

	"github.com/sirupsen/logrus"
	repositoryanalyzer "hcl-dataset.dev/repo-analyzer"
	"hcl-dataset.dev/repo-analyzer/database"
)

func main() {
	file, _ := os.OpenFile("repo-analyzer.log", os.O_CREATE|os.O_WRONLY|os.O_APPEND, 0755)
	defer file.Close()
	multi := io.MultiWriter(file, os.Stdout)
	logrus.SetOutput(multi)

	logrus.Info("Process all redistributable repositories.")
	logrus.SetLevel(logrus.DebugLevel)
	db, closer, err := database.InitDatabase()
	if err != nil {
		logrus.Fatalf("Failed to initialize database: %v", err)
	}
	defer closer()

	cpuCount := runtime.NumCPU()
	if envWorkerCount, ok := os.LookupEnv("WORKER_COUNT"); ok {
		count, err := strconv.Atoi(envWorkerCount)
		if err != nil {
			logrus.Fatalf("Failed to parse WORKER_COUNT: %v", err)
		}
		cpuCount = count
	}

	logrus.Info("Start Processing.")
	err = repositoryanalyzer.ProcessRepositories(db, cpuCount)
	if err != nil {
		logrus.Fatalf("Failed to process repositories: %v", err)
	}
	logrus.Info("Processing complete.")
}
