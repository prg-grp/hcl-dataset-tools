package database

type Repository struct {
	ID              int
	Name            string
	CloneURL        string
	LatestCommitSha string
}

type ModuleCall struct {
	Name   string `json:"name"`
	Source string `json:"source"`
}

type Module struct {
	ID                 int
	RepositoryID       int
	Path               string
	Providers          []string
	ModuleCalls        []ModuleCall
	DiagnosticMessages *string
}

type Resource struct {
	ID           int
	ModuleID     int
	ResourceType string
	Name         string
	Type         string
	Provider     string
}
