using Microsoft.EntityFrameworkCore.Migrations;

namespace CarTrade.Data.Migrations
{
    public partial class ModifyRelationVehicleVignette : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Vignettes_VehicleId",
                table: "Vignettes");

            migrationBuilder.DropColumn(
                name: "VignetteId",
                table: "Vehicles");

            migrationBuilder.CreateIndex(
                name: "IX_Vignettes_VehicleId",
                table: "Vignettes",
                column: "VehicleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Vignettes_VehicleId",
                table: "Vignettes");

            migrationBuilder.AddColumn<int>(
                name: "VignetteId",
                table: "Vehicles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Vignettes_VehicleId",
                table: "Vignettes",
                column: "VehicleId",
                unique: true);
        }
    }
}
