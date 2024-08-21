using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class AddAdditionalRepoInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsFork",
                table: "Repositories",
                newName: "StarCount");

            migrationBuilder.AddColumn<DateTime>(
                name: "LatestCommitAt",
                table: "Repositories",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "LatestCommitSha",
                table: "Repositories",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SizeInKb",
                table: "Repositories",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "DiagnosticMessages",
                table: "Modules",
                type: "TEXT",
                nullable: true);
            
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS IgnoreDuplicateRepoTrigger;");
            
            migrationBuilder.Sql("""
                                 CREATE TRIGGER IF NOT EXISTS IgnoreDuplicateRepoTrigger
                                     BEFORE INSERT
                                     ON Repositories
                                     FOR EACH ROW
                                 BEGIN
                                     INSERT OR IGNORE
                                     INTO Repositories
                                     VALUES (new.Id, new.Name, new.FullName, new.CreatedAt, new.Description, new.License, new.StarCount, new.ForkCount,
                                             new.HtmlUrl, new.GitUrl, new.SshUrl, new.CloneUrl, new.Homepage, new.Archived, new.Topics, new.LatestCommitAt,
                                             new.LatestCommitSha, new.SizeInKb);
                                     select RAISE(IGNORE);
                                 END;
                                 """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LatestCommitAt",
                table: "Repositories");

            migrationBuilder.DropColumn(
                name: "LatestCommitSha",
                table: "Repositories");

            migrationBuilder.DropColumn(
                name: "SizeInKb",
                table: "Repositories");

            migrationBuilder.DropColumn(
                name: "DiagnosticMessages",
                table: "Modules");

            migrationBuilder.RenameColumn(
                name: "StarCount",
                table: "Repositories",
                newName: "IsFork");
            
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS IgnoreDuplicateRepoTrigger;");
            
            migrationBuilder.Sql("""
                                 CREATE TRIGGER IF NOT EXISTS IgnoreDuplicateRepoTrigger
                                     BEFORE INSERT
                                     ON Repositories
                                     FOR EACH ROW
                                 BEGIN
                                     INSERT OR IGNORE
                                     INTO Repositories
                                     VALUES (new.Id, new.Name, new.FullName, new.CreatedAt, new.Description, new.License, new.IsFork, new.ForkCount,
                                             new.HtmlUrl, new.GitUrl, new.SshUrl, new.CloneUrl, new.Homepage, new.Archived, new.Topics);
                                     select RAISE(IGNORE);
                                 END;
                                 """);
        }
    }
}
