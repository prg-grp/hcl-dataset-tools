set dotenv-load := true
set dotenv-required := true

export DB_DIR := join(invocation_directory(), env("DB_DIR"))

default:
    just -l

print-db-dir:
    echo {{DB_DIR}}

[confirm('This will delete the database and all data in it. Are you sure?')]
delete-db:
    rm -rf {{DB_DIR}} && mkdir {{DB_DIR}}

update-db:
    dotnet ef database update --project src/Database

reset-db: delete-db update-db

fetch-metadata:
    cd src/RepositorySearcher && dotnet run -c Release

download-repositories:
    cd src/repository_analyzer && go run ./cmd/analyzer/

cleanup-repositories:
    export DB_PATH="{{join(DB_DIR, "database.sqlite")}}" && \
    export REPOS_PATH="{{join(invocation_directory(), "./src/repository_analyzer/repositories")}}" && \
    python ./src/dataset_cleaner/clean-repos.py

archive-repositories:
    cd src/repository_analyzer && go run ./cmd/compress/
