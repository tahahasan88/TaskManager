using Microsoft.EntityFrameworkCore.Migrations;

namespace TaskManager.Data.Migrations
{
    public partial class TaskFollowUpChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskFollowUpEmployees_TaskEmployees_AssigneeId",
                table: "TaskFollowUpEmployees");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskFollowUps_TaskEmployees_CreatorId",
                table: "TaskFollowUps");

            migrationBuilder.DropIndex(
                name: "IX_TaskFollowUps_CreatorId",
                table: "TaskFollowUps");

            migrationBuilder.DropIndex(
                name: "IX_TaskFollowUpEmployees_AssigneeId",
                table: "TaskFollowUpEmployees");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "TaskFollowUps");

            migrationBuilder.DropColumn(
                name: "AssigneeId",
                table: "TaskFollowUpEmployees");

            migrationBuilder.AddColumn<string>(
                name: "FollowerUserName",
                table: "TaskFollowUps",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssigneeUserName",
                table: "TaskFollowUpEmployees",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FollowerUserName",
                table: "TaskFollowUps");

            migrationBuilder.DropColumn(
                name: "AssigneeUserName",
                table: "TaskFollowUpEmployees");

            migrationBuilder.AddColumn<int>(
                name: "CreatorId",
                table: "TaskFollowUps",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AssigneeId",
                table: "TaskFollowUpEmployees",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskFollowUps_CreatorId",
                table: "TaskFollowUps",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskFollowUpEmployees_AssigneeId",
                table: "TaskFollowUpEmployees",
                column: "AssigneeId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskFollowUpEmployees_TaskEmployees_AssigneeId",
                table: "TaskFollowUpEmployees",
                column: "AssigneeId",
                principalTable: "TaskEmployees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskFollowUps_TaskEmployees_CreatorId",
                table: "TaskFollowUps",
                column: "CreatorId",
                principalTable: "TaskEmployees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
