using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace stok.Migrations
{
    /// <inheritdoc />
    public partial class positiontypefknullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAccounts_PositionTypes_PositionTypeId",
                table: "UserAccounts");

            migrationBuilder.AlterColumn<int>(
                name: "PositionTypeId",
                table: "UserAccounts",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAccounts_PositionTypes_PositionTypeId",
                table: "UserAccounts",
                column: "PositionTypeId",
                principalTable: "PositionTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAccounts_PositionTypes_PositionTypeId",
                table: "UserAccounts");

            migrationBuilder.AlterColumn<int>(
                name: "PositionTypeId",
                table: "UserAccounts",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAccounts_PositionTypes_PositionTypeId",
                table: "UserAccounts",
                column: "PositionTypeId",
                principalTable: "PositionTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
