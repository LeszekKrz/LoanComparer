using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoanComparer.Application.Migrations
{
    public partial class MigrationAfterMerge : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InquiryStatuses_OfferId",
                table: "InquiryStatuses");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "132e3c63-e4a2-4fbe-992d-a97ecd16f605");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4d6e423f-67c6-4d64-97ae-3b1ba826fc36");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "d7dacdc8-6ea9-4138-9ca6-0eb1163e26a2", "d61af7af-4a86-4e31-be7e-c354de8136af", "BankEmployee", "BANKEMPLOYEE" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "e871e297-8a60-460b-ba0e-8213e11066d1", "32bb333b-a59f-4346-8233-2ea84dff7448", "Client", "CLIENT" });

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
                keyValue: "d7dacdc8-6ea9-4138-9ca6-0eb1163e26a2");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e871e297-8a60-460b-ba0e-8213e11066d1");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "132e3c63-e4a2-4fbe-992d-a97ecd16f605", "6f4ee3fe-095c-4661-b79d-b3aaab6fcd2a", "Client", "CLIENT" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "4d6e423f-67c6-4d64-97ae-3b1ba826fc36", "c528c2c8-493c-4c6a-82cb-0d7b01dedf67", "BankEmployee", "BANKEMPLOYEE" });

            migrationBuilder.CreateIndex(
                name: "IX_InquiryStatuses_OfferId",
                table: "InquiryStatuses",
                column: "OfferId");
        }
    }
}
