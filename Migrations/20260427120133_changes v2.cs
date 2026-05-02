using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace stok.Migrations
{
    /// <inheritdoc />
    public partial class changesv2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DisabledTimer",
                table: "UserAccounts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Disbaled",
                table: "UserAccounts",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisabledTimer",
                table: "UserAccounts");

            migrationBuilder.DropColumn(
                name: "Disbaled",
                table: "UserAccounts");
        }
    }
}
