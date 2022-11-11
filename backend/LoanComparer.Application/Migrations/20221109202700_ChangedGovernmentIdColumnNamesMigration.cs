using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoanComparer.Application.Migrations
{
    public partial class ChangedGovernmentIdColumnNamesMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Value",
                table: "AspNetUsers",
                newName: "GovernmentIdValue");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "AspNetUsers",
                newName: "GovernmentIdType");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GovernmentIdValue",
                table: "AspNetUsers",
                newName: "Value");

            migrationBuilder.RenameColumn(
                name: "GovernmentIdType",
                table: "AspNetUsers",
                newName: "Type");
        }
    }
}
