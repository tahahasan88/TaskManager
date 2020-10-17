using Microsoft.EntityFrameworkCore.Migrations;

namespace TaskManager.Data.Migrations
{
    public partial class V55 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskFollowUpResponses_Tasks_TaskId",
                table: "TaskFollowUpResponses");

            migrationBuilder.DropIndex(
                name: "IX_TaskFollowUpResponses_TaskId",
                table: "TaskFollowUpResponses");

            migrationBuilder.DropColumn(
                name: "TaskId",
                table: "TaskFollowUpResponses");

            migrationBuilder.AddColumn<int>(
                name: "StatusId",
                table: "TaskFollowUps",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TaskFollowUpId",
                table: "TaskFollowUpResponses",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TaskFollowUpStatus",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskFollowUpStatus", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskFollowUps_StatusId",
                table: "TaskFollowUps",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskFollowUpResponses_TaskFollowUpId",
                table: "TaskFollowUpResponses",
                column: "TaskFollowUpId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskFollowUpResponses_TaskFollowUps_TaskFollowUpId",
                table: "TaskFollowUpResponses",
                column: "TaskFollowUpId",
                principalTable: "TaskFollowUps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskFollowUps_TaskFollowUpStatus_StatusId",
                table: "TaskFollowUps",
                column: "StatusId",
                principalTable: "TaskFollowUpStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskFollowUpResponses_TaskFollowUps_TaskFollowUpId",
                table: "TaskFollowUpResponses");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskFollowUps_TaskFollowUpStatus_StatusId",
                table: "TaskFollowUps");

            migrationBuilder.DropTable(
                name: "TaskFollowUpStatus");

            migrationBuilder.DropIndex(
                name: "IX_TaskFollowUps_StatusId",
                table: "TaskFollowUps");

            migrationBuilder.DropIndex(
                name: "IX_TaskFollowUpResponses_TaskFollowUpId",
                table: "TaskFollowUpResponses");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "TaskFollowUps");

            migrationBuilder.DropColumn(
                name: "TaskFollowUpId",
                table: "TaskFollowUpResponses");

            migrationBuilder.AddColumn<int>(
                name: "TaskId",
                table: "TaskFollowUpResponses",
                type: "int",
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
    }
}
