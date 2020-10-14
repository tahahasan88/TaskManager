using Microsoft.EntityFrameworkCore.Migrations;

namespace TaskManager.Data.Migrations
{
    public partial class V51 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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
                name: "LastUpdatedById",
                table: "Tasks",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_LastUpdatedById",
                table: "Tasks",
                column: "LastUpdatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Employees_LastUpdatedById",
                table: "Tasks",
                column: "LastUpdatedById",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Employees_LastUpdatedById",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_LastUpdatedById",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "LastUpdatedById",
                table: "Tasks");

            migrationBuilder.AddColumn<int>(
                name: "TaskManager.Data.LastUpdatedBy",
                table: "Tasks",
                type: "int",
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
    }
}
