using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoanComparer.Application.Migrations
{
    public partial class ApplicationTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "29aae5dc-bb88-4907-a34b-ad872be101ff");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "51a1a7a1-a83a-48ca-b748-25c72aacb0f9");

            migrationBuilder.AddColumn<long>(
                name: "DateOfApplication",
                table: "Offers",
                type: "bigint",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "1980b4f8-ede2-48a4-8b92-f601b5ee6d57", "477b008f-6299-4fdf-b5d6-af6f68964ed6", "Client", "CLIENT" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "9930a8fb-76d2-4211-982e-909752c3c701", "e4c346d1-ca07-4a1a-9cb0-ed7543f7f187", "BankEmployee", "BANKEMPLOYEE" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1980b4f8-ede2-48a4-8b92-f601b5ee6d57");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9930a8fb-76d2-4211-982e-909752c3c701");

            migrationBuilder.DropColumn(
                name: "DateOfApplication",
                table: "Offers");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "29aae5dc-bb88-4907-a34b-ad872be101ff", "e61ddd9d-d508-4fb6-83d2-ffe3fb69fbde", "Client", "CLIENT" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "51a1a7a1-a83a-48ca-b748-25c72aacb0f9", "7fe14097-9e7c-49ed-8f56-17ed648f6066", "BankEmployee", "BANKEMPLOYEE" });
        }
    }
}
