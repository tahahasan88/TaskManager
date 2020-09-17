using Microsoft.EntityFrameworkCore.Migrations;

namespace TaskManager.Data.Migrations
{
    public partial class RemoveTaskFollowUpEmployee : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskFollowUpEmployees");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "TaskEmployees",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "TaskEmployees");

            migrationBuilder.CreateTable(
                name: "TaskFollowUpEmployees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssigneeUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaskFollowUpId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskFollowUpEmployees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskFollowUpEmployees_TaskFollowUps_TaskFollowUpId",
                        column: x => x.TaskFollowUpId,
                        principalTable: "TaskFollowUps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskFollowUpEmployees_TaskFollowUpId",
                table: "TaskFollowUpEmployees",
                column: "TaskFollowUpId");
        }
    }
}
