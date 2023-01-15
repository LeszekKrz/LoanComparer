using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoanComparer.Application.Migrations
{
    public partial class FixNullability : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "050d2178-18c6-4b48-94c9-3b42b4b7899d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b063b9c6-7cdf-4f56-9cae-846538c40f4c");

            migrationBuilder.AlterColumn<long>(
                name: "BirthDateTimestamp",
                table: "Inquiries",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "662de062-f4a8-4f66-945a-802bf78efd7c", "2d6bae29-7d05-4a84-9c58-b20833029953", "BankEmployee", "BANKEMPLOYEE" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "d638564c-1e0b-4e08-8266-40e3fbd8459a", "bcbc5e1c-969c-4178-a77b-a775cd302091", "Client", "CLIENT" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "662de062-f4a8-4f66-945a-802bf78efd7c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d638564c-1e0b-4e08-8266-40e3fbd8459a");

            migrationBuilder.AlterColumn<long>(
                name: "BirthDateTimestamp",
                table: "Inquiries",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "050d2178-18c6-4b48-94c9-3b42b4b7899d", "c7b7cb3c-2f85-4f7e-be48-751a6a111be3", "BankEmployee", "BANKEMPLOYEE" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "b063b9c6-7cdf-4f56-9cae-846538c40f4c", "89fcf395-c410-44fc-b4a1-2972472b5e8b", "Client", "CLIENT" });
        }
    }
}
