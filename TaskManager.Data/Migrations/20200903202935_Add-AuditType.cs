using Microsoft.EntityFrameworkCore.Migrations;

namespace TaskManager.Data.Migrations
{
    public partial class AddAuditType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "TaskAudit");

            migrationBuilder.AddColumn<int>(
                name: "TypeId",
                table: "TaskAudit",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AuditType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditType", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskAudit_TypeId",
                table: "TaskAudit",
                column: "TypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskAudit_AuditType_TypeId",
                table: "TaskAudit",
                column: "TypeId",
                principalTable: "AuditType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskAudit_AuditType_TypeId",
                table: "TaskAudit");

            migrationBuilder.DropTable(
                name: "AuditType");

            migrationBuilder.DropIndex(
                name: "IX_TaskAudit_TypeId",
                table: "TaskAudit");

            migrationBuilder.DropColumn(
                name: "TypeId",
                table: "TaskAudit");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "TaskAudit",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
