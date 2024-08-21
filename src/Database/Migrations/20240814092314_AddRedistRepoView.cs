using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class AddRedistRepoView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                                 create view if not exists RedistributableRepositories
                                 as
                                 select *
                                 from Repositories
                                 where License is not null
                                   and License != 'NOASSERTION';
                                 """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("drop view if exists RedistributableRepositories;");
        }
    }
}
