using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CisternasGAMC.Migrations
{
    /// <inheritdoc />
    public partial class president : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "OtbId",
                table: "Users",
                type: "smallint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OtbId",
                table: "Users");
        }
    }
}
