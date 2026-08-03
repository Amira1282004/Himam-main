using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Himam_main.Migrations
{
    /// <inheritdoc />
    public partial class AddPlatformNameToSocialMedia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PlatformName",
                table: "SocialMediaLinks",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlatformName",
                table: "SocialMediaLinks");
        }
    }
}
