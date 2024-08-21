using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class AddModulesAndResources : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Modules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RepositoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    Path = table.Column<string>(type: "TEXT", nullable: false),
                    Providers = table.Column<string>(type: "TEXT", nullable: false),
                    ModuleCalls = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Modules_Repositories_RepositoryId",
                        column: x => x.RepositoryId,
                        principalTable: "Repositories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Resources",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ModuleId = table.Column<int>(type: "INTEGER", nullable: false),
                    ResourceType = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    Provider = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resources", x => x.Id);
                    table.CheckConstraint("CK_Resources_ResourceType_Enum", "\"ResourceType\" IN ('Managed', 'Data')");
                    table.ForeignKey(
                        name: "FK_Resources_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Modules_RepositoryId_Path",
                table: "Modules",
                columns: new[] { "RepositoryId", "Path" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Resources_ModuleId",
                table: "Resources",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Resources_Provider_Type",
                table: "Resources",
                columns: new[] { "Provider", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_Resources_Provider_Type_Name",
                table: "Resources",
                columns: new[] { "Provider", "Type", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_Resources_Type_Name",
                table: "Resources",
                columns: new[] { "Type", "Name" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Resources");

            migrationBuilder.DropTable(
                name: "Modules");
        }
    }
}
