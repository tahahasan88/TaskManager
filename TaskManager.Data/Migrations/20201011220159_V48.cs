using Microsoft.EntityFrameworkCore.Migrations;

namespace TaskManager.Data.Migrations
{
    public partial class V48 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EmployeeId",
                table: "TaskEmployees",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskEmployees_EmployeeId",
                table: "TaskEmployees",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskEmployees_Employees_EmployeeId",
                table: "TaskEmployees",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskEmployees_Employees_EmployeeId",
                table: "TaskEmployees");

            migrationBuilder.DropIndex(
                name: "IX_TaskEmployees_EmployeeId",
                table: "TaskEmployees");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "TaskEmployees");
        }
    }
}
