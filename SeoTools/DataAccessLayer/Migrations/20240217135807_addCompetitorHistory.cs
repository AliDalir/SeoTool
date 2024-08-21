using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class addCompetitorHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CompetitorsSummeries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SiteId = table.Column<int>(type: "int", nullable: false),
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
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompetitorsSummeries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompetitorsSummeries_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompetitorsSummeries_SiteId",
                table: "CompetitorsSummeries",
                column: "SiteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompetitorsSummeries");
        }
    }
}
