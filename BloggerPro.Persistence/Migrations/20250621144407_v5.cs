using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BloggerPro.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class v5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SeoMetadataId",
                table: "Posts",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SeoMetadata",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Keywords = table.Column<string>(type: "text", nullable: false),
                    LanguageCode = table.Column<string>(type: "text", nullable: false),
                    CanonicalGroupId = table.Column<Guid>(type: "uuid", nullable: true),
                    PostModuleId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeoMetadata", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SeoMetadata_PostModule_PostModuleId",
                        column: x => x.PostModuleId,
                        principalTable: "PostModule",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Posts_SeoMetadataId",
                table: "Posts",
                column: "SeoMetadataId");

            migrationBuilder.CreateIndex(
                name: "IX_SeoMetadata_PostModuleId",
                table: "SeoMetadata",
                column: "PostModuleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_SeoMetadata_SeoMetadataId",
                table: "Posts",
                column: "SeoMetadataId",
                principalTable: "SeoMetadata",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_SeoMetadata_SeoMetadataId",
                table: "Posts");

            migrationBuilder.DropTable(
                name: "SeoMetadata");

            migrationBuilder.DropIndex(
                name: "IX_Posts_SeoMetadataId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "SeoMetadataId",
                table: "Posts");
        }
    }
}
