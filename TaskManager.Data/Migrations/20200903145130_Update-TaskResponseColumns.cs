using Microsoft.EntityFrameworkCore.Migrations;

namespace TaskManager.Data.Migrations
{
    public partial class UpdateTaskResponseColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskFollowUpResponses_TaskEmployees_RespondedById",
                table: "TaskFollowUpResponses");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskFollowUpResponses_TaskFollowUps_TaskFollowUpId",
                table: "TaskFollowUpResponses");

            migrationBuilder.DropIndex(
                name: "IX_TaskFollowUpResponses_RespondedById",
                table: "TaskFollowUpResponses");

            migrationBuilder.DropIndex(
                name: "IX_TaskFollowUpResponses_TaskFollowUpId",
                table: "TaskFollowUpResponses");

            migrationBuilder.DropColumn(
                name: "RespondedById",
                table: "TaskFollowUpResponses");

            migrationBuilder.DropColumn(
                name: "TaskFollowUpId",
                table: "TaskFollowUpResponses");

            migrationBuilder.AddColumn<string>(
                name: "RespondedBy",
                table: "TaskFollowUpResponses",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TaskId",
                table: "TaskFollowUpResponses",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskFollowUpResponses_TaskId",
                table: "TaskFollowUpResponses",
                column: "TaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskFollowUpResponses_Tasks_TaskId",
                table: "TaskFollowUpResponses",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskFollowUpResponses_Tasks_TaskId",
                table: "TaskFollowUpResponses");

            migrationBuilder.DropIndex(
                name: "IX_TaskFollowUpResponses_TaskId",
                table: "TaskFollowUpResponses");

            migrationBuilder.DropColumn(
                name: "RespondedBy",
                table: "TaskFollowUpResponses");

            migrationBuilder.DropColumn(
                name: "TaskId",
                table: "TaskFollowUpResponses");

            migrationBuilder.AddColumn<int>(
                name: "RespondedById",
                table: "TaskFollowUpResponses",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TaskFollowUpId",
                table: "TaskFollowUpResponses",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskFollowUpResponses_RespondedById",
                table: "TaskFollowUpResponses",
                column: "RespondedById");

            migrationBuilder.CreateIndex(
                name: "IX_TaskFollowUpResponses_TaskFollowUpId",
                table: "TaskFollowUpResponses",
                column: "TaskFollowUpId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskFollowUpResponses_TaskEmployees_RespondedById",
                table: "TaskFollowUpResponses",
                column: "RespondedById",
                principalTable: "TaskEmployees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskFollowUpResponses_TaskFollowUps_TaskFollowUpId",
                table: "TaskFollowUpResponses",
                column: "TaskFollowUpId",
                principalTable: "TaskFollowUps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
