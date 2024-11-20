using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CisternasGAMC.Migrations
{
    /// <inheritdoc />
    public partial class UrlTelegram : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UrlTelegram",
                table: "Otbs",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UrlTelegram",
                table: "Otbs");
        }
    }
}
