using Microsoft.EntityFrameworkCore.Migrations;

namespace CarTrade.Data.Migrations
{
    public partial class WrongFKVehicleToInsPolocy_ExpireInsPolicyMakeNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_InsurancePolicies_InsurancePolicyId",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_InsurancePolicyId",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "InsurancePolicyId",
                table: "Vehicles");

            migrationBuilder.AddColumn<int>(
                name: "VehicleId",
                table: "InsurancePolicies",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_InsurancePolicies_VehicleId",
                table: "InsurancePolicies",
                column: "VehicleId");

            migrationBuilder.AddForeignKey(
                name: "FK_InsurancePolicies_Vehicles_VehicleId",
                table: "InsurancePolicies",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InsurancePolicies_Vehicles_VehicleId",
                table: "InsurancePolicies");

            migrationBuilder.DropIndex(
                name: "IX_InsurancePolicies_VehicleId",
                table: "InsurancePolicies");

            migrationBuilder.DropColumn(
                name: "VehicleId",
                table: "InsurancePolicies");

            migrationBuilder.AddColumn<int>(
                name: "InsurancePolicyId",
                table: "Vehicles",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_InsurancePolicyId",
                table: "Vehicles",
                column: "InsurancePolicyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_InsurancePolicies_InsurancePolicyId",
                table: "Vehicles",
                column: "InsurancePolicyId",
                principalTable: "InsurancePolicies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
