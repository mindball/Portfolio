using Microsoft.EntityFrameworkCore.Migrations;

namespace CarTrade.Data.Migrations
{
    public partial class RemoveNullableExpireColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "Expired",
                table: "Vignettes",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Expired",
                table: "InsurancePolicies",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "Expired",
                table: "Vignettes",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "Expired",
                table: "InsurancePolicies",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool));
        }
    }
}
