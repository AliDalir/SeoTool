using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class editSearchKeyword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SearchKeywords_Query",
                table: "SearchKeywords");

            migrationBuilder.CreateIndex(
                name: "IX_SearchKeywords_Query",
                table: "SearchKeywords",
                column: "Query",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SearchKeywords_Query",
                table: "SearchKeywords");

            migrationBuilder.CreateIndex(
                name: "IX_SearchKeywords_Query",
                table: "SearchKeywords",
                column: "Query");
        }
    }
}
