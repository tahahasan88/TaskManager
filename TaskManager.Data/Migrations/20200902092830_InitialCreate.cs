using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TaskManager.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TaskCapacities",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Capacity = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskCapacities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaskPriority",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Priority = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskPriority", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaskStatus",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskProgress = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Target = table.Column<DateTime>(nullable: false),
                    TaskStatusId = table.Column<int>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    TaskPriorityId = table.Column<int>(nullable: true),
                    LastUpdatedAt = table.Column<DateTime>(nullable: false),
                    LastUpdatedBy = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tasks_TaskPriority_TaskPriorityId",
                        column: x => x.TaskPriorityId,
                        principalTable: "TaskPriority",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tasks_TaskStatus_TaskStatusId",
                        column: x => x.TaskStatusId,
                        principalTable: "TaskStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TaskEmployees",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserCode = table.Column<string>(nullable: true),
                    TaskId = table.Column<int>(nullable: true),
                    TaskCapacityId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskEmployees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskEmployees_TaskCapacities_TaskCapacityId",
                        column: x => x.TaskCapacityId,
                        principalTable: "TaskCapacities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaskEmployees_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubTasks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskId = table.Column<int>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    TaskStatusId = table.Column<int>(nullable: true),
                    LastUpdatedAt = table.Column<DateTime>(nullable: false),
                    LastUpdatedById = table.Column<int>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubTasks_TaskEmployees_LastUpdatedById",
                        column: x => x.LastUpdatedById,
                        principalTable: "TaskEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubTasks_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubTasks_TaskStatus_TaskStatusId",
                        column: x => x.TaskStatusId,
                        principalTable: "TaskStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TaskFollowUps",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Remarks = table.Column<string>(nullable: true),
                    TaskId = table.Column<int>(nullable: true),
                    CreatorId = table.Column<int>(nullable: true),
                    LastUpdatedAt = table.Column<DateTime>(nullable: false),
                    LastUpdatedBy = table.Column<DateTime>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskFollowUps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskFollowUps_TaskEmployees_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "TaskEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaskFollowUps_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TaskFollowUpEmployees",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskFollowUpId = table.Column<int>(nullable: true),
                    AssigneeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskFollowUpEmployees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskFollowUpEmployees_TaskEmployees_AssigneeId",
                        column: x => x.AssigneeId,
                        principalTable: "TaskEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaskFollowUpEmployees_TaskFollowUps_TaskFollowUpId",
                        column: x => x.TaskFollowUpId,
                        principalTable: "TaskFollowUps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TaskFollowUpResponses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskFollowUpId = table.Column<int>(nullable: true),
                    TaskResponse = table.Column<string>(nullable: true),
                    RespondedById = table.Column<int>(nullable: true),
                    LastUpdatedAt = table.Column<DateTime>(nullable: false),
                    LastUpdatedBy = table.Column<DateTime>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskFollowUpResponses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskFollowUpResponses_TaskEmployees_RespondedById",
                        column: x => x.RespondedById,
                        principalTable: "TaskEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaskFollowUpResponses_TaskFollowUps_TaskFollowUpId",
                        column: x => x.TaskFollowUpId,
                        principalTable: "TaskFollowUps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubTasks_LastUpdatedById",
                table: "SubTasks",
                column: "LastUpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SubTasks_TaskId",
                table: "SubTasks",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_SubTasks_TaskStatusId",
                table: "SubTasks",
                column: "TaskStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskEmployees_TaskCapacityId",
                table: "TaskEmployees",
                column: "TaskCapacityId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskEmployees_TaskId",
                table: "TaskEmployees",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskFollowUpEmployees_AssigneeId",
                table: "TaskFollowUpEmployees",
                column: "AssigneeId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskFollowUpEmployees_TaskFollowUpId",
                table: "TaskFollowUpEmployees",
                column: "TaskFollowUpId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskFollowUpResponses_RespondedById",
                table: "TaskFollowUpResponses",
                column: "RespondedById");

            migrationBuilder.CreateIndex(
                name: "IX_TaskFollowUpResponses_TaskFollowUpId",
                table: "TaskFollowUpResponses",
                column: "TaskFollowUpId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskFollowUps_CreatorId",
                table: "TaskFollowUps",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskFollowUps_TaskId",
                table: "TaskFollowUps",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_TaskPriorityId",
                table: "Tasks",
                column: "TaskPriorityId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_TaskStatusId",
                table: "Tasks",
                column: "TaskStatusId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubTasks");

            migrationBuilder.DropTable(
                name: "TaskFollowUpEmployees");

            migrationBuilder.DropTable(
                name: "TaskFollowUpResponses");

            migrationBuilder.DropTable(
                name: "TaskFollowUps");

            migrationBuilder.DropTable(
                name: "TaskEmployees");

            migrationBuilder.DropTable(
                name: "TaskCapacities");

            migrationBuilder.DropTable(
                name: "Tasks");

            migrationBuilder.DropTable(
                name: "TaskPriority");

            migrationBuilder.DropTable(
                name: "TaskStatus");
        }
    }
}
