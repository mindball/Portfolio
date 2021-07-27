using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CarTrade.Data.Migrations
{
    public partial class VehicleCreatedOn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Vehicles",
                nullable: true                
               );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Vehicles",
                type: "datetime2",
                nullable: false              
               );
        }
    }
}
