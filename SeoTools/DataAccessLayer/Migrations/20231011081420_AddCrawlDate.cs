using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddCrawlDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CrawlDateId",
                table: "Ranks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CrawlDates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CrawlDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrawlDates", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ranks_CrawlDateId",
                table: "Ranks",
                column: "CrawlDateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ranks_CrawlDates_CrawlDateId",
                table: "Ranks",
                column: "CrawlDateId",
                principalTable: "CrawlDates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ranks_CrawlDates_CrawlDateId",
                table: "Ranks");

            migrationBuilder.DropTable(
                name: "CrawlDates");

            migrationBuilder.DropIndex(
                name: "IX_Ranks_CrawlDateId",
                table: "Ranks");

            migrationBuilder.DropColumn(
                name: "CrawlDateId",
                table: "Ranks");
        }
    }
}
