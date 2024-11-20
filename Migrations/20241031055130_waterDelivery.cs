using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CisternasGAMC.Migrations
{
    /// <inheritdoc />
    public partial class waterDelivery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "DeliveredAmount",
                table: "WaterDeliveries",
                type: "real",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "DeliveredAmount",
                table: "WaterDeliveries",
                type: "real",
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);
        }
    }
}
