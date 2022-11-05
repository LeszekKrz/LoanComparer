using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoanComparer.Application.Migrations
{
    public partial class JobTypeMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JobTypes",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobTypes", x => x.Name);
                });

            migrationBuilder.InsertData(
                table: "JobTypes",
                column: "Name",
                value: "JobType1");

            migrationBuilder.InsertData(
                table: "JobTypes",
                column: "Name",
                value: "JobType2");

            migrationBuilder.InsertData(
                table: "JobTypes",
                column: "Name",
                value: "Other");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobTypes");
        }
    }
}
