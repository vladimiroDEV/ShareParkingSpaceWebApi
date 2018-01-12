using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ShareParkingSpaceWebApi.Migrations
{
    public partial class chengeModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Auto_AutoID",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_AutoID",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "AutoID",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "UderID",
                table: "Auto",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UderID",
                table: "Auto");

            migrationBuilder.AddColumn<long>(
                name: "AutoID",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_AutoID",
                table: "AspNetUsers",
                column: "AutoID");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Auto_AutoID",
                table: "AspNetUsers",
                column: "AutoID",
                principalTable: "Auto",
                principalColumn: "AutoID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
