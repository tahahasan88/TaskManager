using Microsoft.EntityFrameworkCore.Migrations;

namespace TaskManager.Data.Migrations
{
    public partial class V54 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Employees_LastUpdateBy",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_LastUpdateBy",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "LastUpdateBy",
                table: "Tasks");

            migrationBuilder.AddColumn<int>(
                name: "Employee",
                table: "Tasks",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_Employee",
                table: "Tasks",
                column: "Employee");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Employees_Employee",
                table: "Tasks",
                column: "Employee",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Employees_Employee",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_Employee",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Employee",
                table: "Tasks");

            migrationBuilder.AddColumn<int>(
                name: "LastUpdateBy",
                table: "Tasks",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_LastUpdateBy",
                table: "Tasks",
                column: "LastUpdateBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Employees_LastUpdateBy",
                table: "Tasks",
                column: "LastUpdateBy",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
