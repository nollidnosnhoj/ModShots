using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ModShots.Application.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreateUploadAndMediaTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "posts",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    public_id = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    severity = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    published_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_posts", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "medias",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(26)", nullable: false),
                    caption = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    file_path = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    original_file_name = table.Column<string>(type: "text", nullable: false),
                    mime_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    file_size = table.Column<long>(type: "bigint", nullable: false),
                    width = table.Column<int>(type: "integer", nullable: false),
                    height = table.Column<int>(type: "integer", nullable: false),
                    md5 = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    blur_hash = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    is_complete = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    post_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_medias", x => x.id);
                    table.ForeignKey(
                        name: "fk_medias_posts_post_id",
                        column: x => x.post_id,
                        principalTable: "posts",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "ix_medias_post_id",
                table: "medias",
                column: "post_id");

            migrationBuilder.CreateIndex(
                name: "ix_posts_public_id",
                table: "posts",
                column: "public_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "medias");

            migrationBuilder.DropTable(
                name: "posts");
        }
    }
}
