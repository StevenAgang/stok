using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace stok.Migrations
{
    /// <inheritdoc />
    public partial class changes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TYpe",
                table: "PositionTypes",
                newName: "Type");

            migrationBuilder.AddColumn<string>(
                name: "MiddleName",
                table: "UserInformations",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MiddleName",
                table: "UserInformations");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "PositionTypes",
                newName: "TYpe");
        }
    }
}
