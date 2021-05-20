using Microsoft.EntityFrameworkCore.Migrations;

namespace CarTrade.Data.Migrations
{
    public partial class AddDbSetVignette : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vignette_Vehicles_VehicleId",
                table: "Vignette");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Vignette",
                table: "Vignette");

            migrationBuilder.RenameTable(
                name: "Vignette",
                newName: "Vignettes");

            migrationBuilder.RenameIndex(
                name: "IX_Vignette_VehicleId",
                table: "Vignettes",
                newName: "IX_Vignettes_VehicleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Vignettes",
                table: "Vignettes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vignettes_Vehicles_VehicleId",
                table: "Vignettes",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vignettes_Vehicles_VehicleId",
                table: "Vignettes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Vignettes",
                table: "Vignettes");

            migrationBuilder.RenameTable(
                name: "Vignettes",
                newName: "Vignette");

            migrationBuilder.RenameIndex(
                name: "IX_Vignettes_VehicleId",
                table: "Vignette",
                newName: "IX_Vignette_VehicleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Vignette",
                table: "Vignette",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vignette_Vehicles_VehicleId",
                table: "Vignette",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
