# repository_analyzer

The `repository_analyzer` is a tool designed to process and analyze Terraform repositories. It clones repositories, identifies Terraform modules, and stores analysis results in a database.

## Features

- Clone repositories and remove `.git` folders.
- Identify directories containing Terraform files.
- Analyze Terraform modules and store results in a database.
- Compress and archive processed repositories.

## Prerequisites

- Go 1.22.5 or later
- SQLite3
- Environment variables:
  - `DB_DIR`: Directory for the database.

## Setup

1. Clone the repository:

   ```sh
   git clone <repository-url>
   cd src/repository_analyzer
   ```

2. Install dependencies:

   ```sh
   go mod download
   ```

3. Set up environment variable:

   - `DB_DIR`: Directory for the database.

## Usage

1. Run the analyzer:

   ```sh
   go run ./cmd/analyzer/
   ```

   This will process all redistributable repositories and store the analysis results in the database.

2. Compress processed repositories:

   ```sh
   go run ./cmd/compress/
   ```

   This will compress the processed repositories into a tar.gz archive.
