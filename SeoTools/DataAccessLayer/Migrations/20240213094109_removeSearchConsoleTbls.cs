using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class removeSearchConsoleTbls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SearchConsoleKeywordRanks_SearchConsoleDates_SearchConsoleDateId",
                table: "SearchConsoleKeywordRanks");

            migrationBuilder.DropForeignKey(
                name: "FK_SearchConsoleKeywordRanks_SearchConsoleKeywords_SearchConsoleKeywordId",
                table: "SearchConsoleKeywordRanks");

            migrationBuilder.DropForeignKey(
                name: "FK_SearchConsoleUrls_SearchConsoleKeywords_SearchConsoleKeywordId",
                table: "SearchConsoleUrls");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SearchConsoleUrls",
                table: "SearchConsoleUrls");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SearchConsoleKeywords",
                table: "SearchConsoleKeywords");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SearchConsoleKeywordRanks",
                table: "SearchConsoleKeywordRanks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SearchConsoleDates",
                table: "SearchConsoleDates");

            migrationBuilder.RenameTable(
                name: "SearchConsoleUrls",
                newName: "SearchConsoleUrl");

            migrationBuilder.RenameTable(
                name: "SearchConsoleKeywords",
                newName: "SearchConsoleKeyword");

            migrationBuilder.RenameTable(
                name: "SearchConsoleKeywordRanks",
                newName: "SearchConsoleKeywordRank");

            migrationBuilder.RenameTable(
                name: "SearchConsoleDates",
                newName: "SearchConsoleDate");

            migrationBuilder.RenameIndex(
                name: "IX_SearchConsoleUrls_SearchConsoleKeywordId",
                table: "SearchConsoleUrl",
                newName: "IX_SearchConsoleUrl_SearchConsoleKeywordId");

            migrationBuilder.RenameIndex(
                name: "IX_SearchConsoleKeywordRanks_SearchConsoleKeywordId",
                table: "SearchConsoleKeywordRank",
                newName: "IX_SearchConsoleKeywordRank_SearchConsoleKeywordId");

            migrationBuilder.RenameIndex(
                name: "IX_SearchConsoleKeywordRanks_SearchConsoleDateId",
                table: "SearchConsoleKeywordRank",
                newName: "IX_SearchConsoleKeywordRank_SearchConsoleDateId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SearchConsoleUrl",
                table: "SearchConsoleUrl",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SearchConsoleKeyword",
                table: "SearchConsoleKeyword",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SearchConsoleKeywordRank",
                table: "SearchConsoleKeywordRank",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SearchConsoleDate",
                table: "SearchConsoleDate",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SearchConsoleKeywordRank_SearchConsoleDate_SearchConsoleDateId",
                table: "SearchConsoleKeywordRank",
                column: "SearchConsoleDateId",
                principalTable: "SearchConsoleDate",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SearchConsoleKeywordRank_SearchConsoleKeyword_SearchConsoleKeywordId",
                table: "SearchConsoleKeywordRank",
                column: "SearchConsoleKeywordId",
                principalTable: "SearchConsoleKeyword",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SearchConsoleUrl_SearchConsoleKeyword_SearchConsoleKeywordId",
                table: "SearchConsoleUrl",
                column: "SearchConsoleKeywordId",
                principalTable: "SearchConsoleKeyword",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SearchConsoleKeywordRank_SearchConsoleDate_SearchConsoleDateId",
                table: "SearchConsoleKeywordRank");

            migrationBuilder.DropForeignKey(
                name: "FK_SearchConsoleKeywordRank_SearchConsoleKeyword_SearchConsoleKeywordId",
                table: "SearchConsoleKeywordRank");

            migrationBuilder.DropForeignKey(
                name: "FK_SearchConsoleUrl_SearchConsoleKeyword_SearchConsoleKeywordId",
                table: "SearchConsoleUrl");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SearchConsoleUrl",
                table: "SearchConsoleUrl");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SearchConsoleKeywordRank",
                table: "SearchConsoleKeywordRank");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SearchConsoleKeyword",
                table: "SearchConsoleKeyword");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SearchConsoleDate",
                table: "SearchConsoleDate");

            migrationBuilder.RenameTable(
                name: "SearchConsoleUrl",
                newName: "SearchConsoleUrls");

            migrationBuilder.RenameTable(
                name: "SearchConsoleKeywordRank",
                newName: "SearchConsoleKeywordRanks");

            migrationBuilder.RenameTable(
                name: "SearchConsoleKeyword",
                newName: "SearchConsoleKeywords");

            migrationBuilder.RenameTable(
                name: "SearchConsoleDate",
                newName: "SearchConsoleDates");

            migrationBuilder.RenameIndex(
                name: "IX_SearchConsoleUrl_SearchConsoleKeywordId",
                table: "SearchConsoleUrls",
                newName: "IX_SearchConsoleUrls_SearchConsoleKeywordId");

            migrationBuilder.RenameIndex(
                name: "IX_SearchConsoleKeywordRank_SearchConsoleKeywordId",
                table: "SearchConsoleKeywordRanks",
                newName: "IX_SearchConsoleKeywordRanks_SearchConsoleKeywordId");

            migrationBuilder.RenameIndex(
                name: "IX_SearchConsoleKeywordRank_SearchConsoleDateId",
                table: "SearchConsoleKeywordRanks",
                newName: "IX_SearchConsoleKeywordRanks_SearchConsoleDateId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SearchConsoleUrls",
                table: "SearchConsoleUrls",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SearchConsoleKeywordRanks",
                table: "SearchConsoleKeywordRanks",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SearchConsoleKeywords",
                table: "SearchConsoleKeywords",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SearchConsoleDates",
                table: "SearchConsoleDates",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SearchConsoleKeywordRanks_SearchConsoleDates_SearchConsoleDateId",
                table: "SearchConsoleKeywordRanks",
                column: "SearchConsoleDateId",
                principalTable: "SearchConsoleDates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SearchConsoleKeywordRanks_SearchConsoleKeywords_SearchConsoleKeywordId",
                table: "SearchConsoleKeywordRanks",
                column: "SearchConsoleKeywordId",
                principalTable: "SearchConsoleKeywords",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SearchConsoleUrls_SearchConsoleKeywords_SearchConsoleKeywordId",
                table: "SearchConsoleUrls",
                column: "SearchConsoleKeywordId",
                principalTable: "SearchConsoleKeywords",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
