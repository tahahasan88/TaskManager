using Microsoft.EntityFrameworkCore.Migrations;

namespace TaskManager.Data.Migrations
{
    public partial class V52 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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
                name: "LastUpdateBy",
                table: "Tasks",
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

        protected override void Down(MigrationBuilder migrationBuilder)
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
                name: "LastUpdatedById",
                table: "Tasks",
                type: "int",
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
    }
}
