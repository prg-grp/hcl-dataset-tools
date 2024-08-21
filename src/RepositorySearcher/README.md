# RepositorySearcher

RepositorySearcher is a tool designed to fetch and store repositories from GitHub. It retrieves repository data based on specified queries and stores the information in a database for further analysis.

## Features

- Fetch repositories from GitHub based on search queries.
- Store (public) repository data in a database.
- Handle GitHub API rate limits and pagination.
- Log over-limit queries and errors.

## Prerequisites

- .NET SDK
- GitHub API token
- CSV files for queries, over-limit records, and errors

## Setup

1. Clone the repository:

   ```sh
   git clone <repository-url>
   cd src/RepositorySearcher
   ```

2. Install the required .NET tools:

   ```sh
   dotnet tool restore
   ```

3. Set up environment variables:

   - `GITHUB_TOKEN`: Your GitHub API token.
   - `QUERY_FILE`: Path to the CSV file containing search queries (default: `./queries.csv`).
   - `OVER_LIMIT_FILE`: Path to the CSV file for logging over-limit queries (default: `./over_limit.csv`).
   - `ERROR_FILE`: Path to the CSV file for logging errors (default: `./errors.csv`).
   - `DB_DIR`: Directory for the database.

4. Build the project:

   ```sh
   dotnet build
   ```

## Usage

1. Run the RepositorySearcher:

   ```sh
   dotnet run --project src/RepositorySearcher/RepositorySearcher.csproj
   ```

2. The tool will fetch repositories based on the queries specified in the `QUERY_FILE` and store the data in the database.

Please note that each query must not return more than 1000 results due to GitHub API limitations.

## Project Structure

- `src/RepositorySearcher/`: Contains the main code for fetching and processing GitHub repositories.
- `src/RepositorySearcher/GitHub/`: Contains models and classes related to GitHub API responses.
- `src/RepositorySearcher/Program.cs`: Main entry point for the application.
- `src/Database/`: Contains database context and models.
