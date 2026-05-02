using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace stok.Migrations
{
    /// <inheritdoc />
    public partial class forgotpasswordtokenmanager : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "forgotPasswordTokenManagers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserAccountId = table.Column<int>(type: "integer", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Created_By = table.Column<int>(type: "integer", nullable: true),
                    Created_At = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Updated_By = table.Column<int>(type: "integer", nullable: true),
                    Updated_At = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Deleted_By = table.Column<int>(type: "integer", nullable: true),
                    Deleted_At = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_forgotPasswordTokenManagers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_forgotPasswordTokenManagers_UserAccounts_UserAccountId",
                        column: x => x.UserAccountId,
                        principalTable: "UserAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_forgotPasswordTokenManagers_UserAccountId",
                table: "forgotPasswordTokenManagers",
                column: "UserAccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "forgotPasswordTokenManagers");
        }
    }
}
