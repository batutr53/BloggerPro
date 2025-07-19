using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BloggerPro.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTikTokAndYouTubeUrlToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TikTokUrl",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "YouTubeUrl",
                table: "AspNetUsers",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TikTokUrl",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "YouTubeUrl",
                table: "AspNetUsers");
        }
    }
}
