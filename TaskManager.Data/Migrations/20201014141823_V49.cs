using Microsoft.EntityFrameworkCore.Migrations;

namespace TaskManager.Data.Migrations
{
    public partial class V49 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastUpdatedBy",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "FollowerUserName",
                table: "TaskFollowUps");

            migrationBuilder.DropColumn(
                name: "RespondedBy",
                table: "TaskFollowUpResponses");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "TaskEmployees");

            migrationBuilder.DropColumn(
                name: "ActionBy",
                table: "TaskAudit");

            migrationBuilder.DropColumn(
                name: "LastUpdatedBy",
                table: "SubTasks");

            migrationBuilder.DropColumn(
                name: "SubTaskAssigneeUserName",
                table: "SubTasks");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Employees");

            migrationBuilder.AddColumn<int>(
                name: "Employee",
                table: "Tasks",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FollowerId",
                table: "TaskFollowUps",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RespondedById",
                table: "TaskFollowUpResponses",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ActionById",
                table: "TaskAudit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LastUpdatedById",
                table: "SubTasks",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubTaskAssigneeId",
                table: "SubTasks",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_Employee",
                table: "Tasks",
                column: "Employee");

            migrationBuilder.CreateIndex(
                name: "IX_TaskFollowUps_FollowerId",
                table: "TaskFollowUps",
                column: "FollowerId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskFollowUpResponses_RespondedById",
                table: "TaskFollowUpResponses",
                column: "RespondedById");

            migrationBuilder.CreateIndex(
                name: "IX_TaskAudit_ActionById",
                table: "TaskAudit",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_SubTasks_LastUpdatedById",
                table: "SubTasks",
                column: "LastUpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SubTasks_SubTaskAssigneeId",
                table: "SubTasks",
                column: "SubTaskAssigneeId");

            migrationBuilder.AddForeignKey(
                name: "FK_SubTasks_Employees_LastUpdatedById",
                table: "SubTasks",
                column: "LastUpdatedById",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SubTasks_Employees_SubTaskAssigneeId",
                table: "SubTasks",
                column: "SubTaskAssigneeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskAudit_Employees_ActionById",
                table: "TaskAudit",
                column: "ActionById",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskFollowUpResponses_Employees_RespondedById",
                table: "TaskFollowUpResponses",
                column: "RespondedById",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskFollowUps_Employees_FollowerId",
                table: "TaskFollowUps",
                column: "FollowerId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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
                name: "FK_SubTasks_Employees_LastUpdatedById",
                table: "SubTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_SubTasks_Employees_SubTaskAssigneeId",
                table: "SubTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskAudit_Employees_ActionById",
                table: "TaskAudit");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskFollowUpResponses_Employees_RespondedById",
                table: "TaskFollowUpResponses");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskFollowUps_Employees_FollowerId",
                table: "TaskFollowUps");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Employees_Employee",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_Employee",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_TaskFollowUps_FollowerId",
                table: "TaskFollowUps");

            migrationBuilder.DropIndex(
                name: "IX_TaskFollowUpResponses_RespondedById",
                table: "TaskFollowUpResponses");

            migrationBuilder.DropIndex(
                name: "IX_TaskAudit_ActionById",
                table: "TaskAudit");

            migrationBuilder.DropIndex(
                name: "IX_SubTasks_LastUpdatedById",
                table: "SubTasks");

            migrationBuilder.DropIndex(
                name: "IX_SubTasks_SubTaskAssigneeId",
                table: "SubTasks");

            migrationBuilder.DropColumn(
                name: "Employee",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "FollowerId",
                table: "TaskFollowUps");

            migrationBuilder.DropColumn(
                name: "RespondedById",
                table: "TaskFollowUpResponses");

            migrationBuilder.DropColumn(
                name: "ActionById",
                table: "TaskAudit");

            migrationBuilder.DropColumn(
                name: "LastUpdatedById",
                table: "SubTasks");

            migrationBuilder.DropColumn(
                name: "SubTaskAssigneeId",
                table: "SubTasks");

            migrationBuilder.AddColumn<string>(
                name: "LastUpdatedBy",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FollowerUserName",
                table: "TaskFollowUps",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RespondedBy",
                table: "TaskFollowUpResponses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "TaskEmployees",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ActionBy",
                table: "TaskAudit",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastUpdatedBy",
                table: "SubTasks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubTaskAssigneeUserName",
                table: "SubTasks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
