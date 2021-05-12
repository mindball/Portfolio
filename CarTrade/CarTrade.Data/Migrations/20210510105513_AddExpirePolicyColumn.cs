using Microsoft.EntityFrameworkCore.Migrations;

namespace CarTrade.Data.Migrations
{
    public partial class AddExpirePolicyColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Expired",
                table: "InsurancePolicies",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Expired",
                table: "InsurancePolicies");
        }
    }
}
