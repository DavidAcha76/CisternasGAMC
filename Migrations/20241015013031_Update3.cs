using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CisternasGAMC.Migrations
{
    /// <inheritdoc />
    public partial class Update3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastUpdate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RegisterDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastUpdate",
                table: "Otbs");

            migrationBuilder.DropColumn(
                name: "LastUpdate",
                table: "Cisterns");

            migrationBuilder.DropColumn(
                name: "RegisterDate",
                table: "Cisterns");

            migrationBuilder.CreateIndex(
                name: "IX_WaterDeliveries_CisternId",
                table: "WaterDeliveries",
                column: "CisternId");

            migrationBuilder.CreateIndex(
                name: "IX_WaterDeliveries_DriverId",
                table: "WaterDeliveries",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_WaterDeliveries_OtbId",
                table: "WaterDeliveries",
                column: "OtbId");

            migrationBuilder.AddForeignKey(
                name: "FK_WaterDeliveries_Cisterns_CisternId",
                table: "WaterDeliveries",
                column: "CisternId",
                principalTable: "Cisterns",
                principalColumn: "CisternId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WaterDeliveries_Otbs_OtbId",
                table: "WaterDeliveries",
                column: "OtbId",
                principalTable: "Otbs",
                principalColumn: "OtbId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WaterDeliveries_Users_DriverId",
                table: "WaterDeliveries",
                column: "DriverId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WaterDeliveries_Cisterns_CisternId",
                table: "WaterDeliveries");

            migrationBuilder.DropForeignKey(
                name: "FK_WaterDeliveries_Otbs_OtbId",
                table: "WaterDeliveries");

            migrationBuilder.DropForeignKey(
                name: "FK_WaterDeliveries_Users_DriverId",
                table: "WaterDeliveries");

            migrationBuilder.DropIndex(
                name: "IX_WaterDeliveries_CisternId",
                table: "WaterDeliveries");

            migrationBuilder.DropIndex(
                name: "IX_WaterDeliveries_DriverId",
                table: "WaterDeliveries");

            migrationBuilder.DropIndex(
                name: "IX_WaterDeliveries_OtbId",
                table: "WaterDeliveries");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdate",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "RegisterDate",
                table: "Users",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdate",
                table: "Otbs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdate",
                table: "Cisterns",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "RegisterDate",
                table: "Cisterns",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
