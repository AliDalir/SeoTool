using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddKeywordGroupHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KeywordGroupHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CrawlDateId = table.Column<int>(type: "int", nullable: false),
                    KeywordGroupId = table.Column<int>(type: "int", nullable: false),
                    AllAvgPosition = table.Column<double>(type: "float", nullable: false),
                    LastAvgPosition = table.Column<double>(type: "float", nullable: false),
                    Top3Count = table.Column<int>(type: "int", nullable: false),
                    Top3AvgPosition = table.Column<double>(type: "float", nullable: false),
                    Top10Count = table.Column<int>(type: "int", nullable: false),
                    Top10AvgPosition = table.Column<double>(type: "float", nullable: false),
                    Top100Count = table.Column<int>(type: "int", nullable: false),
                    Top100AvgPosition = table.Column<double>(type: "float", nullable: false),
                    NoRankCount = table.Column<int>(type: "int", nullable: false),
                    KeywordCount = table.Column<int>(type: "int", nullable: false),
                    CreationDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModificationDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeywordGroupHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KeywordGroupHistories_CrawlDates_CrawlDateId",
                        column: x => x.CrawlDateId,
                        principalTable: "CrawlDates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KeywordGroupHistories_KeywordGroups_KeywordGroupId",
                        column: x => x.KeywordGroupId,
                        principalTable: "KeywordGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KeywordGroupHistories_CrawlDateId",
                table: "KeywordGroupHistories",
                column: "CrawlDateId");

            migrationBuilder.CreateIndex(
                name: "IX_KeywordGroupHistories_KeywordGroupId",
                table: "KeywordGroupHistories",
                column: "KeywordGroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KeywordGroupHistories");
        }
    }
}
