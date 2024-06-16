using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HaefeleSoftware.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial_v1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "roles",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    created = table.Column<DateTime>(type: "datetime2", maxLength: 100, nullable: false),
                    last_modified = table.Column<DateTime>(type: "datetime2", maxLength: 100, nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    last_modified_by = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    created = table.Column<DateTime>(type: "datetime2", maxLength: 100, nullable: false),
                    last_modified = table.Column<DateTime>(type: "datetime2", maxLength: 100, nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    last_modified_by = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    first_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    last_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    password_hash = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    fk_role_id = table.Column<int>(type: "int", nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                    table.ForeignKey(
                        name: "FK_users_roles_fk_role_id",
                        column: x => x.fk_role_id,
                        principalSchema: "dbo",
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tokens",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    created = table.Column<DateTime>(type: "datetime2", maxLength: 100, nullable: false),
                    last_modified = table.Column<DateTime>(type: "datetime2", maxLength: 100, nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    last_modified_by = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    refresh_token = table.Column<string>(type: "nvarchar(1500)", maxLength: 1500, nullable: false),
                    expires_at = table.Column<DateTime>(type: "datetime", maxLength: 100, nullable: false),
                    created_by_ip = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    revoked_by_ip = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    fk_user_id = table.Column<int>(type: "int", nullable: false),
                    is_expired = table.Column<bool>(type: "bit", nullable: false),
                    is_revoked = table.Column<bool>(type: "bit", nullable: false),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tokens", x => x.id);
                    table.ForeignKey(
                        name: "FK_tokens_users_fk_user_id",
                        column: x => x.fk_user_id,
                        principalSchema: "dbo",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tokens_fk_user_id",
                schema: "dbo",
                table: "tokens",
                column: "fk_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_fk_role_id",
                schema: "dbo",
                table: "users",
                column: "fk_role_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tokens",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "users",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "roles",
                schema: "dbo");
        }
    }
}
