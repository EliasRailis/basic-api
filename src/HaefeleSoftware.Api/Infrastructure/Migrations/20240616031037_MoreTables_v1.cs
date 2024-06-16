using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HaefeleSoftware.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MoreTables_v1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "artists",
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
                    table.PrimaryKey("PK_artists", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "libraries",
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
                    is_deleted = table.Column<bool>(type: "bit", nullable: false),
                    fk_user_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_libraries", x => x.id);
                    table.ForeignKey(
                        name: "FK_libraries_users_fk_user_id",
                        column: x => x.fk_user_id,
                        principalSchema: "dbo",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "albums",
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
                    year_of_release = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    duration = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    number_of_songs = table.Column<int>(type: "int", nullable: false),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false),
                    fk_artist_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_albums", x => x.id);
                    table.ForeignKey(
                        name: "FK_albums_artists_fk_artist_id",
                        column: x => x.fk_artist_id,
                        principalSchema: "dbo",
                        principalTable: "artists",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "library_albums",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    created = table.Column<DateTime>(type: "datetime2", maxLength: 100, nullable: false),
                    last_modified = table.Column<DateTime>(type: "datetime2", maxLength: 100, nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    last_modified_by = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FK_LibraryId = table.Column<int>(type: "int", nullable: false),
                    FK_AlbumId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_library_albums", x => x.id);
                    table.ForeignKey(
                        name: "FK_library_albums_albums_FK_AlbumId",
                        column: x => x.FK_AlbumId,
                        principalSchema: "dbo",
                        principalTable: "albums",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_library_albums_libraries_FK_LibraryId",
                        column: x => x.FK_LibraryId,
                        principalSchema: "dbo",
                        principalTable: "libraries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "songs",
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
                    duration = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false),
                    FK_AlbumId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_songs", x => x.id);
                    table.ForeignKey(
                        name: "FK_songs_albums_FK_AlbumId",
                        column: x => x.FK_AlbumId,
                        principalSchema: "dbo",
                        principalTable: "albums",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_albums_fk_artist_id",
                schema: "dbo",
                table: "albums",
                column: "fk_artist_id");

            migrationBuilder.CreateIndex(
                name: "IX_libraries_fk_user_id",
                schema: "dbo",
                table: "libraries",
                column: "fk_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_library_albums_FK_AlbumId",
                schema: "dbo",
                table: "library_albums",
                column: "FK_AlbumId");

            migrationBuilder.CreateIndex(
                name: "IX_library_albums_FK_LibraryId",
                schema: "dbo",
                table: "library_albums",
                column: "FK_LibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_songs_FK_AlbumId",
                schema: "dbo",
                table: "songs",
                column: "FK_AlbumId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "library_albums",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "songs",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "libraries",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "albums",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "artists",
                schema: "dbo");
        }
    }
}
