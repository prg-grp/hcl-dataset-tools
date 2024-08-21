# dataset_cleaner

The `dataset_cleaner` script is designed to clean up the dataset of repositories by removing unnecessary files, empty folders, and invalid repositories that do not contain any modules or resources.

## Features

- Recursively delete non-Terraform files from repositories.
- Delete empty folders within the repositories.
- Delete repositories that do not contain any Terraform modules or resources.

## Prerequisites

- Python 3.8 or later
- SQLite3
- pandas library

## Setup

1. Clone the repository:

   ```sh
   git clone <repository-url>
   cd src/dataset_cleaner
   ```

2. Install the required Python packages:

   ```sh
   pip install pandas
   ```

3. Ensure you have a SQLite database named `TerraDS.sqlite` with the necessary tables and data.

## Usage

1. Set the `REPOS_PATH` environmebnt variable to the directory containing the repositories.

2. Run the script:

   ```sh
   python clean-repos.py
   ```

   This will clean up the dataset by deleting non-Terraform files, empty folders, and invalid repositories.

## Notes

- The script assumes that the SQLite database `TerraDS.sqlite` is located in the same directory as the script.
- Ensure that the database contains the `RedistributableRepositories`, `Modules`, and `Resources` tables with the appropriate schema.
