using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BloggerPro.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFooterEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Footers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SectionTitle = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Content = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    LinkUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    LinkText = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    FooterType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IconClass = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ParentSection = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Footers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Footers_FooterType",
                table: "Footers",
                column: "FooterType");

            migrationBuilder.CreateIndex(
                name: "IX_Footers_IsActive",
                table: "Footers",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Footers_SortOrder",
                table: "Footers",
                column: "SortOrder");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Footers");
        }
    }
}
