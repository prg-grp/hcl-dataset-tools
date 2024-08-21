package database

import "encoding/json"

func (db *Database) GetRedistributableRepositories() (int, <-chan Repository) {
	var count int
	db.db.QueryRow("select count(1) as c from RedistributableRepositories").Scan(&count)

	result := make(chan Repository, count)
	go func() {
		defer close(result)
		rows, _ := db.db.Query("select Id, Name, CloneURL from RedistributableRepositories")
		defer rows.Close()

		for rows.Next() {
			repo := Repository{}
			rows.Scan(&repo.ID, &repo.Name, &repo.CloneURL)
			result <- repo
		}
	}()

	return count, result
}

func (db *Database) UpdateCommitSHA(repoID int, commitSHA string) error {
	_, err := db.db.Exec("update Repositories set LatestCommitSha = $1 where Id = $2", commitSHA, repoID)
	return err
}

func (db *Database) StoreAnalysis(module Module, diag *string, managedResources []Resource, dataResources []Resource) error {
	tx, err := db.db.Begin()
	if err != nil {
		return err
	}

	modStmt, _ := tx.Prepare(`insert into Modules
                              (RepositoryId, Path, Providers, ModuleCalls, DiagnosticMessages)
                              values ($1, $2, $3, $4, $5)
                              returning Id`)
	defer modStmt.Close()
	resStmt, _ := tx.Prepare(`insert into Resources
                              (ModuleId, ResourceType, Name, Type, Provider)
                              values ($1, $2, $3, $4, $5)`)
	defer resStmt.Close()

	pB, _ := json.Marshal(module.Providers)
	mB, _ := json.Marshal(module.ModuleCalls)

	var id int
	err = modStmt.QueryRow(module.RepositoryID, module.Path, string(pB), string(mB), diag).Scan(&id)
	if err != nil {
		tx.Rollback()
		return err
	}

	for _, r := range managedResources {
		_, err = resStmt.Exec(id, r.ResourceType, r.Name, r.Type, r.Provider)
		if err != nil {
			tx.Rollback()
			return err
		}
	}

	for _, r := range dataResources {
		_, err = resStmt.Exec(id, r.ResourceType, r.Name, r.Type, r.Provider)
		if err != nil {
			tx.Rollback()
			return err
		}
	}

	return tx.Commit()
}
