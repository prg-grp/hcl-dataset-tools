# HCL Dataset Tools

[![DOI](https://zenodo.org/badge/DOI/10.5281/zenodo.14217385.svg)](https://doi.org/10.5281/zenodo.14217385)

This repository contains a set of tools for creating and processing datasets, specifically focusing on Terraform repositories. The tools included are:

- RepositorySearcher
- Analyzer
- Cleaner
- Compressor

## Usage

To produce the same result as "TerraDS", follow these steps:

1. **Run the RepositorySearcher** with the provided queries. This creates the `dataset.sqlite` database.
2. **Run the Analyzer**.
3. **Run the Cleaner**.
4. **Run the Compressor**.
5. **Delete the `RedistributableRepositories` and `_EFMigrationHistory` tables** from the database.

### RepositorySearcher

The RepositorySearcher can be used to search for repositories based on provided queries. While it can search for various types of repositories, the other tools in this repository are specifically focused on Terraform.

### Analyzer

The Analyzer processes the overall metadata about repositories in `dataset.sqlite`. This means that all publicly available (e.g. permissive licensed) repositories are downloaded and analyzed.

### Cleaner

The Cleaner removes unnecessary data from the `dataset.sqlite` database to prepare it for compression. It also removes non-Terraform files, empty directories, and repositories containing non-Terraform code.

### Compressor

The Compressor compresses the cleaned data, making it easier to store and distribute.

## Using the Justfile

A `justfile` is provided to simplify the usage of these tools. [Just](https://just.systems) is a command runner that allows you to define and run commands easily.

To use the `justfile`, follow these steps:

1. Install `just` by following the instructions on the [Just website](https://just.systems).
2. Prepare the `.env` file from `.env.dist`.
3. Run the following commands in order:

```sh
just fetch-metadata
just download-repositories
just cleanup-repositories
just archive-repositories
```

## License

This project is licensed under the terms of the Creative Commons Attribution 4.0 International License. For more details, see the [LICENSE](LICENSE) file.
