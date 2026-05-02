using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace stok.Migrations
{
    /// <inheritdoc />
    public partial class renametable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_forgotPasswordTokenManagers_UserAccounts_UserAccountId",
                table: "forgotPasswordTokenManagers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_forgotPasswordTokenManagers",
                table: "forgotPasswordTokenManagers");

            migrationBuilder.RenameTable(
                name: "forgotPasswordTokenManagers",
                newName: "ForgotPasswordTokenManagers");

            migrationBuilder.RenameIndex(
                name: "IX_forgotPasswordTokenManagers_UserAccountId",
                table: "ForgotPasswordTokenManagers",
                newName: "IX_ForgotPasswordTokenManagers_UserAccountId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ForgotPasswordTokenManagers",
                table: "ForgotPasswordTokenManagers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ForgotPasswordTokenManagers_UserAccounts_UserAccountId",
                table: "ForgotPasswordTokenManagers",
                column: "UserAccountId",
                principalTable: "UserAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ForgotPasswordTokenManagers_UserAccounts_UserAccountId",
                table: "ForgotPasswordTokenManagers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ForgotPasswordTokenManagers",
                table: "ForgotPasswordTokenManagers");

            migrationBuilder.RenameTable(
                name: "ForgotPasswordTokenManagers",
                newName: "forgotPasswordTokenManagers");

            migrationBuilder.RenameIndex(
                name: "IX_ForgotPasswordTokenManagers_UserAccountId",
                table: "forgotPasswordTokenManagers",
                newName: "IX_forgotPasswordTokenManagers_UserAccountId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_forgotPasswordTokenManagers",
                table: "forgotPasswordTokenManagers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_forgotPasswordTokenManagers_UserAccounts_UserAccountId",
                table: "forgotPasswordTokenManagers",
                column: "UserAccountId",
                principalTable: "UserAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
