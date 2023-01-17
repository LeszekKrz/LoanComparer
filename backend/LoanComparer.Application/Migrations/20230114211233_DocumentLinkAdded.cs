using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoanComparer.Application.Migrations
{
    public partial class DocumentLinkAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InquiryStatuses_OfferId",
                table: "InquiryStatuses");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7a3c43ec-03a3-40ff-8334-fe2849855b60");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e7527a1b-63e0-43be-a111-7804ae2426d1");

            migrationBuilder.AddColumn<string>(
                name: "DocumentLink",
                table: "Offers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "26dec652-87e1-47d6-a44e-c20f84b6b13f", "d74cc774-15c7-425e-93ff-d1521df68d74", "BankEmployee", "BANKEMPLOYEE" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "e432809e-d894-4ab6-8480-57c702b7d74c", "81f1a4a3-22f0-464d-bb50-3453fba4e789", "Client", "CLIENT" });

            migrationBuilder.CreateIndex(
                name: "IX_InquiryStatuses_OfferId",
                table: "InquiryStatuses",
                column: "OfferId",
                unique: true,
                filter: "[OfferId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InquiryStatuses_OfferId",
                table: "InquiryStatuses");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "26dec652-87e1-47d6-a44e-c20f84b6b13f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e432809e-d894-4ab6-8480-57c702b7d74c");

            migrationBuilder.DropColumn(
                name: "DocumentLink",
                table: "Offers");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "7a3c43ec-03a3-40ff-8334-fe2849855b60", "9b4c5545-6062-4de7-b0b4-1cd7bfcaaab7", "BankEmployee", "BANKEMPLOYEE" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "e7527a1b-63e0-43be-a111-7804ae2426d1", "0b5d7a25-31bc-40f7-8c04-7804c6853784", "Client", "CLIENT" });

            migrationBuilder.CreateIndex(
                name: "IX_InquiryStatuses_OfferId",
                table: "InquiryStatuses",
                column: "OfferId");
        }
    }
}
