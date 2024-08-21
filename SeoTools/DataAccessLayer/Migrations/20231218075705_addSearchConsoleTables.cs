using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class addSearchConsoleTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SearchConsoleDates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchConsoleDates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SearchConsoleUrls",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Page = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchConsoleUrls", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SearchConsoleKeywords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SearchConsoleDateId = table.Column<int>(type: "int", nullable: false),
                    SearchConsoleUrlId = table.Column<int>(type: "int", nullable: true),
                    Query = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchConsoleKeywords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SearchConsoleKeywords_SearchConsoleDates_SearchConsoleDateId",
                        column: x => x.SearchConsoleDateId,
                        principalTable: "SearchConsoleDates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SearchConsoleKeywords_SearchConsoleUrls_SearchConsoleUrlId",
                        column: x => x.SearchConsoleUrlId,
                        principalTable: "SearchConsoleUrls",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SearchConsoleKeywordRanks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SearchConsoleKeywordId = table.Column<int>(type: "int", nullable: false),
                    SearchConsoleDateId = table.Column<int>(type: "int", nullable: false),
                    Clicks = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ctr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Impressions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchConsoleKeywordRanks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SearchConsoleKeywordRanks_SearchConsoleDates_SearchConsoleDateId",
                        column: x => x.SearchConsoleDateId,
                        principalTable: "SearchConsoleDates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SearchConsoleKeywordRanks_SearchConsoleKeywords_SearchConsoleKeywordId",
                        column: x => x.SearchConsoleKeywordId,
                        principalTable: "SearchConsoleKeywords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SearchConsoleKeywordRanks_SearchConsoleDateId",
                table: "SearchConsoleKeywordRanks",
                column: "SearchConsoleDateId");

            migrationBuilder.CreateIndex(
                name: "IX_SearchConsoleKeywordRanks_SearchConsoleKeywordId",
                table: "SearchConsoleKeywordRanks",
                column: "SearchConsoleKeywordId");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SearchConsoleKeywordRanks");

            migrationBuilder.DropTable(
                name: "SearchConsoleKeywords");

            migrationBuilder.DropTable(
                name: "SearchConsoleDates");

            migrationBuilder.DropTable(
                name: "SearchConsoleUrls");
        }
    }
}
