using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace stok.Migrations
{
    /// <inheritdoc />
    public partial class updatescrapemodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "token",
                table: "ScrapeServiceTokenManagers",
                newName: "Token");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Token",
                table: "ScrapeServiceTokenManagers",
                newName: "token");
        }
    }
}
