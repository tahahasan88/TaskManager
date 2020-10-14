using Microsoft.EntityFrameworkCore.Migrations;

namespace TaskManager.Data.Migrations
{
    public partial class V50 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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
                name: "TaskManager.Data.LastUpdatedBy",
                table: "Tasks",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_TaskManager.Data.LastUpdatedBy",
                table: "Tasks",
                column: "TaskManager.Data.LastUpdatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Employees_TaskManager.Data.LastUpdatedBy",
                table: "Tasks",
                column: "TaskManager.Data.LastUpdatedBy",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Employees_TaskManager.Data.LastUpdatedBy",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_TaskManager.Data.LastUpdatedBy",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "TaskManager.Data.LastUpdatedBy",
                table: "Tasks");

            migrationBuilder.AddColumn<int>(
                name: "Employee",
                table: "Tasks",
                type: "int",
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
    }
}
