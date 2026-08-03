using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Himam_main.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToTeamMember : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "TeamMembers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembers_UserId",
                table: "TeamMembers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMembers_Users",
                table: "TeamMembers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamMembers_Users",
                table: "TeamMembers");

            migrationBuilder.DropIndex(
                name: "IX_TeamMembers_UserId",
                table: "TeamMembers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TeamMembers");
        }
    }
}
