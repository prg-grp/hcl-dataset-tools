package main

import (
	"context"
	"os"
	"path/filepath"

	"github.com/mholt/archiver/v4"
	"github.com/sirupsen/logrus"
)

func main() {
	logrus.SetLevel(logrus.DebugLevel)
	logrus.Info("Compressing repository dir.")

	filepath.WalkDir("repositories", func(path string, d os.DirEntry, err error) error {
		if err != nil {
			return err
		}

		if !d.IsDir() || path == "repositories" {
			return nil
		}

		logrus.Infof("Delete non zip dir in repositories folder '%v'", path)
		os.RemoveAll(path)
		return nil
	})

	zipFiles, err := archiver.FilesFromDisk(nil, map[string]string{
		"repositories/": "",
	})
	if err != nil {
		logrus.Fatalf("Failed to create files from disk: %v", err)
	}

	os.RemoveAll("repositories.tar.gz")
	zipOut, err := os.Create("repositories.tar.gz")
	if err != nil {
		logrus.Fatalf("Failed to create tar gz file: %v", err)
	}
	defer zipOut.Close()
	err = archiver.CompressedArchive{
		Compression: archiver.Gz{},
		Archival:    archiver.Tar{},
	}.Archive(context.Background(), zipOut, zipFiles)
	if err != nil {
		logrus.Fatalf("Failed to compress archive: %v", err)
	}
}
