using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CarTrade.Data.Migrations
{
    public partial class AddNewEntityVignette : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VignetteId",
                table: "Vehicles",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Vignette",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    VehicleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vignette", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vignette_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vignette_VehicleId",
                table: "Vignette",
                column: "VehicleId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Vignette");

            migrationBuilder.DropColumn(
                name: "VignetteId",
                table: "Vehicles");
        }
    }
}
