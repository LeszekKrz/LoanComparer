using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoanComparer.Application.Migrations
{
    public partial class AddAdditionalDataToInquiryStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "662de062-f4a8-4f66-945a-802bf78efd7c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d638564c-1e0b-4e08-8266-40e3fbd8459a");

            migrationBuilder.AddColumn<string>(
                name: "AdditionalData",
                table: "InquiryStatuses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "7a3c43ec-03a3-40ff-8334-fe2849855b60", "9b4c5545-6062-4de7-b0b4-1cd7bfcaaab7", "BankEmployee", "BANKEMPLOYEE" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "e7527a1b-63e0-43be-a111-7804ae2426d1", "0b5d7a25-31bc-40f7-8c04-7804c6853784", "Client", "CLIENT" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7a3c43ec-03a3-40ff-8334-fe2849855b60");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e7527a1b-63e0-43be-a111-7804ae2426d1");

            migrationBuilder.DropColumn(
                name: "AdditionalData",
                table: "InquiryStatuses");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "662de062-f4a8-4f66-945a-802bf78efd7c", "2d6bae29-7d05-4a84-9c58-b20833029953", "BankEmployee", "BANKEMPLOYEE" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "d638564c-1e0b-4e08-8266-40e3fbd8459a", "bcbc5e1c-969c-4178-a77b-a775cd302091", "Client", "CLIENT" });
        }
    }
}
