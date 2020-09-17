using Microsoft.EntityFrameworkCore.Migrations;

namespace TaskManager.Data.Migrations
{
    public partial class SubTaskModify : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubTasks_TaskEmployees_LastUpdatedById",
                table: "SubTasks");

            migrationBuilder.DropIndex(
                name: "IX_SubTasks_LastUpdatedById",
                table: "SubTasks");

            migrationBuilder.DropColumn(
                name: "LastUpdatedById",
                table: "SubTasks");

            migrationBuilder.AddColumn<string>(
                name: "LastUpdatedBy",
                table: "SubTasks",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubTaskAssigneeUserName",
                table: "SubTasks",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastUpdatedBy",
                table: "SubTasks");

            migrationBuilder.DropColumn(
                name: "SubTaskAssigneeUserName",
                table: "SubTasks");

            migrationBuilder.AddColumn<int>(
                name: "LastUpdatedById",
                table: "SubTasks",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubTasks_LastUpdatedById",
                table: "SubTasks",
                column: "LastUpdatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_SubTasks_TaskEmployees_LastUpdatedById",
                table: "SubTasks",
                column: "LastUpdatedById",
                principalTable: "TaskEmployees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
