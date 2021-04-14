using Microsoft.EntityFrameworkCore.Migrations;

namespace CarTrade.Data.Migrations
{
    public partial class AddInsuaranceCompanyRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InsurancePolicies_InsuanceCompanies_InsuanceCompanyId",
                table: "InsurancePolicies");

            migrationBuilder.DropTable(
                name: "InsuanceCompanies");

            migrationBuilder.CreateTable(
                name: "InsuranceCompanies",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsuranceCompanies", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_InsurancePolicies_InsuranceCompanies_InsuanceCompanyId",
                table: "InsurancePolicies",
                column: "InsuanceCompanyId",
                principalTable: "InsuranceCompanies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InsurancePolicies_InsuranceCompanies_InsuanceCompanyId",
                table: "InsurancePolicies");

            migrationBuilder.DropTable(
                name: "InsuranceCompanies");

            migrationBuilder.CreateTable(
                name: "InsuanceCompanies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsuanceCompanies", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_InsurancePolicies_InsuanceCompanies_InsuanceCompanyId",
                table: "InsurancePolicies",
                column: "InsuanceCompanyId",
                principalTable: "InsuanceCompanies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
