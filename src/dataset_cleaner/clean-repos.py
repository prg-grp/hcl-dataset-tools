import os
import sqlite3

import pandas as pd

PATH = os.environ.get("REPOS_PATH", "./repositories")
DB_PATH = os.environ.get("DB_PATH", "./TerraDS.sqlite")


# Recursively delete all non-Terraform files in the given directory (recursively)
def delete_non_tf_files_recursively(dir_path):
    for item in os.listdir(dir_path):
        item_path = os.path.join(dir_path, item)
        if os.path.isdir(item_path):
            delete_non_tf_files_recursively(item_path)
        elif os.path.isfile(item_path) and not (
            item.endswith(".tf") or item == "clean-repos.py"
        ):
            try:
                os.remove(item_path)
            except Exception as e:
                print(f"Failed to delete {item_path}: {e}")


# Delete all empty folders in the given root directory
def delete_empty_folders(root):
    for dirpath, dirnames, _ in os.walk(root, topdown=False):
        for dirname in dirnames:
            full_path = os.path.join(dirpath, dirname)
            if os.path.islink(full_path):
                print(f"Symbolic Link: {full_path}")
            elif os.path.isdir(full_path):
                if not os.listdir(full_path):
                    os.rmdir(full_path)
                    print(f"Deleted empty folder: {full_path}")
            else:
                print(f"Not a directory: {full_path}")


# Delete repositories that do not contain any modules or resources
def delete_invalid_repos(path):
    conn = sqlite3.connect(DB_PATH)
    df = pd.read_sql_query(
        """
            SELECT DISTINCT rr.Id
            FROM Repositories rr
            WHERE NOT EXISTS (
                SELECT 1
                FROM Modules m
                JOIN Resources r ON r.ModuleId = m.Id
                WHERE m.RepositoryId = rr.Id
            );
            """,
        conn,
    )
    for repo_id in df["Id"]:
        repo_path = os.path.join(path, str(repo_id))
        os.system(f"rm -r {repo_path}")
        conn.execute("DELETE FROM Repositories WHERE Id = ?", (repo_id,))
    conn.close()
    print("Deleted invalid repositories")


delete_non_tf_files_recursively(PATH)
delete_empty_folders(PATH)
delete_invalid_repos(PATH)
