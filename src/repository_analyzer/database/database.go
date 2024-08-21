package database

import (
	"database/sql"
	"fmt"
	"os"

	_ "github.com/mattn/go-sqlite3"
)

type Database struct {
	db *sql.DB
}

func InitDatabase() (*Database, func() error, error) {
	path, ok := os.LookupEnv("DB_DIR")
	if !ok {
		return nil, nil, fmt.Errorf("DB_DIR not set")
	}

	path = fmt.Sprintf("%s/dataset.sqlite?_fk=true&mode=rw", path)

	db, err := sql.Open("sqlite3", path)
	if err != nil {
		return nil, nil, err
	}
	return &Database{db}, db.Close, nil
}
