using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddSomeIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SiteUrl",
                table: "Sites",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "SiteName",
                table: "Sites",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Sites_SiteName",
                table: "Sites",
                column: "SiteName");

            migrationBuilder.CreateIndex(
                name: "IX_Sites_SiteUrl",
                table: "Sites",
                column: "SiteUrl");

            migrationBuilder.CreateIndex(
                name: "IX_Ranks_Position",
                table: "Ranks",
                column: "Position");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sites_SiteName",
                table: "Sites");

            migrationBuilder.DropIndex(
                name: "IX_Sites_SiteUrl",
                table: "Sites");

            migrationBuilder.DropIndex(
                name: "IX_Ranks_Position",
                table: "Ranks");

            migrationBuilder.AlterColumn<string>(
                name: "SiteUrl",
                table: "Sites",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "SiteName",
                table: "Sites",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
