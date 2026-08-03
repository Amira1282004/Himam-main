using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Himam_main.Migrations
{
    /// <inheritdoc />
    public partial class AddStaticContentTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "ServiceCategories",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsVisible",
                table: "ServiceCategories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "ServiceCategories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AboutSections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Eyebrow = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    AdditionalDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChairmanName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ChairmanTitle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ChairmanImage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__AboutSections__3214EC07A5545B4E", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AboutSections_Users",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CompanyValues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Content = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CompanyValues__3214EC07E6C337E4", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyValues_Users",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ContactInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    WorkingHours = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MapEmbedUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ContactInfos__3214EC07DF95702A", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContactInfos_Users",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HeroSections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Subtitle = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CtaText = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CtaLink = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    YoutubeVideoId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsVideoEnabled = table.Column<bool>(type: "bit", nullable: false),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__HeroSections__3214EC07AA5B1B4C", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HeroSections_Users",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProcessSteps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Subtitle = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    StepNumber = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ProcessSteps__3214EC072C9D1A93", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcessSteps_Users",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Sectors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Image = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Sectors__3214EC07C2C85624", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sectors_Users",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SocialMediaLinks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Platform = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IconSvg = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SocialMediaLinks__3214EC07E853A1DE", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SocialMediaLinks_Users",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StatItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Value = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__StatItems__3214EC07781EF791", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StatItems_Users",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCategories_UserId",
                table: "ServiceCategories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AboutSections_UserId",
                table: "AboutSections",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyValues_UserId",
                table: "CompanyValues",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ContactInfos_UserId",
                table: "ContactInfos",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_HeroSections_UserId",
                table: "HeroSections",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessSteps_UserId",
                table: "ProcessSteps",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Sectors_UserId",
                table: "Sectors",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SocialMediaLinks_UserId",
                table: "SocialMediaLinks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_StatItems_UserId",
                table: "StatItems",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceCategories_Users",
                table: "ServiceCategories",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceCategories_Users",
                table: "ServiceCategories");

            migrationBuilder.DropTable(
                name: "AboutSections");

            migrationBuilder.DropTable(
                name: "CompanyValues");

            migrationBuilder.DropTable(
                name: "ContactInfos");

            migrationBuilder.DropTable(
                name: "HeroSections");

            migrationBuilder.DropTable(
                name: "ProcessSteps");

            migrationBuilder.DropTable(
                name: "Sectors");

            migrationBuilder.DropTable(
                name: "SocialMediaLinks");

            migrationBuilder.DropTable(
                name: "StatItems");

            migrationBuilder.DropIndex(
                name: "IX_ServiceCategories_UserId",
                table: "ServiceCategories");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "ServiceCategories");

            migrationBuilder.DropColumn(
                name: "IsVisible",
                table: "ServiceCategories");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ServiceCategories");
        }
    }
}
