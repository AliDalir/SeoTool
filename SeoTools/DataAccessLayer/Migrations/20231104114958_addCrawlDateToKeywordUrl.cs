using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class addCrawlDateToKeywordUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CrawlDateId",
                table: "KeywordUrl",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_KeywordUrl_CrawlDateId",
                table: "KeywordUrl",
                column: "CrawlDateId");

            migrationBuilder.AddForeignKey(
                name: "FK_KeywordUrl_CrawlDates_CrawlDateId",
                table: "KeywordUrl",
                column: "CrawlDateId",
                principalTable: "CrawlDates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KeywordUrl_CrawlDates_CrawlDateId",
                table: "KeywordUrl");

            migrationBuilder.DropIndex(
                name: "IX_KeywordUrl_CrawlDateId",
                table: "KeywordUrl");

            migrationBuilder.DropColumn(
                name: "CrawlDateId",
                table: "KeywordUrl");
        }
    }
}
