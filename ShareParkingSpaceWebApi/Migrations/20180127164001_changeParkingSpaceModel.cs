using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ShareParkingSpaceWebApi.Migrations
{
    public partial class changeParkingSpaceModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "ParkingSpaces",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "ParkingSpaceActions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "ParkingSpaces");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "ParkingSpaceActions");
        }
    }
}
