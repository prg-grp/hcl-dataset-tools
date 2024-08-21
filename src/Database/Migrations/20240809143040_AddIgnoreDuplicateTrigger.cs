using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class AddIgnoreDuplicateTrigger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS IgnoreDuplicateRepoTrigger;");
        }
    }
}
