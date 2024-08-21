using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class addSearch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SearchKeywords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Query = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchKeywords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SearchPerformances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SearchKeywordId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Clicks = table.Column<int>(type: "int", nullable: false),
                    Impressions = table.Column<int>(type: "int", nullable: false),
                    CTR = table.Column<float>(type: "real", nullable: false),
                    Position = table.Column<float>(type: "real", nullable: false),
                    Page = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchPerformances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SearchPerformances_SearchKeywords_SearchKeywordId",
                        column: x => x.SearchKeywordId,
                        principalTable: "SearchKeywords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SearchKeywords_Id",
                table: "SearchKeywords",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_SearchPerformances_Date",
                table: "SearchPerformances",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_SearchPerformances_Page",
                table: "SearchPerformances",
                column: "Page");

            migrationBuilder.CreateIndex(
                name: "IX_SearchPerformances_SearchKeywordId",
                table: "SearchPerformances",
                column: "SearchKeywordId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SearchPerformances");

            migrationBuilder.DropTable(
                name: "SearchKeywords");
        }
    }
}
