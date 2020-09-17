using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TaskManager.Data.Migrations
{
    public partial class ChangeUserNameColumnName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastUpdatedBy",
                table: "TaskFollowUps");

            migrationBuilder.DropColumn(
                name: "LastUpdatedBy",
                table: "TaskFollowUpResponses");

            migrationBuilder.DropColumn(
                name: "UserCode",
                table: "TaskEmployees");

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "TaskEmployees",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserName",
                table: "TaskEmployees");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedBy",
                table: "TaskFollowUps",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedBy",
                table: "TaskFollowUpResponses",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserCode",
                table: "TaskEmployees",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
