using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class KeywordUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KeywordUrl_CrawlDates_CrawlDateId",
                table: "KeywordUrl");

            migrationBuilder.DropForeignKey(
                name: "FK_KeywordUrl_Keywords_KeywordId",
                table: "KeywordUrl");

            migrationBuilder.DropPrimaryKey(
                name: "PK_KeywordUrl",
                table: "KeywordUrl");

            migrationBuilder.RenameTable(
                name: "KeywordUrl",
                newName: "KeywordUrls");

            migrationBuilder.RenameIndex(
                name: "IX_KeywordUrl_KeywordId",
                table: "KeywordUrls",
                newName: "IX_KeywordUrls_KeywordId");

            migrationBuilder.RenameIndex(
                name: "IX_KeywordUrl_CrawlDateId",
                table: "KeywordUrls",
                newName: "IX_KeywordUrls_CrawlDateId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_KeywordUrls",
                table: "KeywordUrls",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_KeywordUrls_CrawlDates_CrawlDateId",
                table: "KeywordUrls",
                column: "CrawlDateId",
                principalTable: "CrawlDates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_KeywordUrls_Keywords_KeywordId",
                table: "KeywordUrls",
                column: "KeywordId",
                principalTable: "Keywords",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KeywordUrls_CrawlDates_CrawlDateId",
                table: "KeywordUrls");

            migrationBuilder.DropForeignKey(
                name: "FK_KeywordUrls_Keywords_KeywordId",
                table: "KeywordUrls");

            migrationBuilder.DropPrimaryKey(
                name: "PK_KeywordUrls",
                table: "KeywordUrls");

            migrationBuilder.RenameTable(
                name: "KeywordUrls",
                newName: "KeywordUrl");

            migrationBuilder.RenameIndex(
                name: "IX_KeywordUrls_KeywordId",
                table: "KeywordUrl",
                newName: "IX_KeywordUrl_KeywordId");

            migrationBuilder.RenameIndex(
                name: "IX_KeywordUrls_CrawlDateId",
                table: "KeywordUrl",
                newName: "IX_KeywordUrl_CrawlDateId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_KeywordUrl",
                table: "KeywordUrl",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_KeywordUrl_CrawlDates_CrawlDateId",
                table: "KeywordUrl",
                column: "CrawlDateId",
                principalTable: "CrawlDates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_KeywordUrl_Keywords_KeywordId",
                table: "KeywordUrl",
                column: "KeywordId",
                principalTable: "Keywords",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
