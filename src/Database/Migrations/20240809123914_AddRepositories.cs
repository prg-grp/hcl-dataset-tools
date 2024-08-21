using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class AddRepositories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Repositories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    FullName = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    License = table.Column<string>(type: "TEXT", nullable: true),
                    IsFork = table.Column<bool>(type: "INTEGER", nullable: false),
                    ForkCount = table.Column<int>(type: "INTEGER", nullable: false),
                    HtmlUrl = table.Column<string>(type: "TEXT", nullable: false),
                    GitUrl = table.Column<string>(type: "TEXT", nullable: false),
                    SshUrl = table.Column<string>(type: "TEXT", nullable: false),
                    CloneUrl = table.Column<string>(type: "TEXT", nullable: false),
                    Homepage = table.Column<string>(type: "TEXT", nullable: true),
                    Archived = table.Column<bool>(type: "INTEGER", nullable: false),
                    Topics = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Repositories", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Repositories");
        }
    }
}
