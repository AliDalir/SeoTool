using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class EditSearchConsoleTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SearchConsoleKeywords_SearchConsoleDates_SearchConsoleDateId",
                table: "SearchConsoleKeywords");

            migrationBuilder.DropForeignKey(
                name: "FK_SearchConsoleKeywords_SearchConsoleUrls_SearchConsoleUrlId",
                table: "SearchConsoleKeywords");

            migrationBuilder.DropIndex(
                name: "IX_SearchConsoleKeywords_SearchConsoleDateId",
                table: "SearchConsoleKeywords");

            migrationBuilder.DropIndex(
                name: "IX_SearchConsoleKeywords_SearchConsoleUrlId",
                table: "SearchConsoleKeywords");

            migrationBuilder.DropColumn(
                name: "SearchConsoleDateId",
                table: "SearchConsoleKeywords");

            migrationBuilder.DropColumn(
                name: "SearchConsoleUrlId",
                table: "SearchConsoleKeywords");

            migrationBuilder.AddColumn<int>(
                name: "SearchConsoleKeywordId",
                table: "SearchConsoleUrls",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SearchConsoleUrls_SearchConsoleKeywordId",
                table: "SearchConsoleUrls",
                column: "SearchConsoleKeywordId");

            migrationBuilder.AddForeignKey(
                name: "FK_SearchConsoleUrls_SearchConsoleKeywords_SearchConsoleKeywordId",
                table: "SearchConsoleUrls",
                column: "SearchConsoleKeywordId",
                principalTable: "SearchConsoleKeywords",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SearchConsoleUrls_SearchConsoleKeywords_SearchConsoleKeywordId",
                table: "SearchConsoleUrls");

            migrationBuilder.DropIndex(
                name: "IX_SearchConsoleUrls_SearchConsoleKeywordId",
                table: "SearchConsoleUrls");

            migrationBuilder.DropColumn(
                name: "SearchConsoleKeywordId",
                table: "SearchConsoleUrls");

            migrationBuilder.AddColumn<int>(
                name: "SearchConsoleDateId",
                table: "SearchConsoleKeywords",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SearchConsoleUrlId",
                table: "SearchConsoleKeywords",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SearchConsoleKeywords_SearchConsoleDateId",
                table: "SearchConsoleKeywords",
                column: "SearchConsoleDateId");

            migrationBuilder.CreateIndex(
                name: "IX_SearchConsoleKeywords_SearchConsoleUrlId",
                table: "SearchConsoleKeywords",
                column: "SearchConsoleUrlId",
                unique: true,
                filter: "[SearchConsoleUrlId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_SearchConsoleKeywords_SearchConsoleDates_SearchConsoleDateId",
                table: "SearchConsoleKeywords",
                column: "SearchConsoleDateId",
                principalTable: "SearchConsoleDates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SearchConsoleKeywords_SearchConsoleUrls_SearchConsoleUrlId",
                table: "SearchConsoleKeywords",
                column: "SearchConsoleUrlId",
                principalTable: "SearchConsoleUrls",
                principalColumn: "Id");
        }
    }
}
