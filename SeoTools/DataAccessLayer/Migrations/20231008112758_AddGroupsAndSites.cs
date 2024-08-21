using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddGroupsAndSites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SiteId",
                table: "Ranks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "KeywordGroupId",
                table: "Keywords",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "KeywordGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreationDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModificationDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeywordGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KeywordGroupId = table.Column<int>(type: "int", nullable: false),
                    SiteUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SiteName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreationDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModificationDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sites_KeywordGroups_KeywordGroupId",
                        column: x => x.KeywordGroupId,
                        principalTable: "KeywordGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ranks_SiteId",
                table: "Ranks",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_Keywords_KeywordGroupId",
                table: "Keywords",
                column: "KeywordGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Sites_KeywordGroupId",
                table: "Sites",
                column: "KeywordGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Keywords_KeywordGroups_KeywordGroupId",
                table: "Keywords",
                column: "KeywordGroupId",
                principalTable: "KeywordGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ranks_Sites_SiteId",
                table: "Ranks",
                column: "SiteId",
                principalTable: "Sites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Keywords_KeywordGroups_KeywordGroupId",
                table: "Keywords");

            migrationBuilder.DropForeignKey(
                name: "FK_Ranks_Sites_SiteId",
                table: "Ranks");

            migrationBuilder.DropTable(
                name: "Sites");

            migrationBuilder.DropTable(
                name: "KeywordGroups");

            migrationBuilder.DropIndex(
                name: "IX_Ranks_SiteId",
                table: "Ranks");

            migrationBuilder.DropIndex(
                name: "IX_Keywords_KeywordGroupId",
                table: "Keywords");

            migrationBuilder.DropColumn(
                name: "SiteId",
                table: "Ranks");

            migrationBuilder.DropColumn(
                name: "KeywordGroupId",
                table: "Keywords");
        }
    }
}
