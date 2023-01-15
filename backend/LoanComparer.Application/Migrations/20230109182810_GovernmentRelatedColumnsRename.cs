using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoanComparer.Application.Migrations
{
    public partial class GovernmentRelatedColumnsRename : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "250c7c93-ef39-49e8-b2ca-aa3ef8e75ef3");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "99767d13-7559-489e-befe-ab5f88c660ff");

            migrationBuilder.RenameColumn(
                name: "GovtIdValue",
                table: "Inquiries",
                newName: "GovernmentIdValue");

            migrationBuilder.RenameColumn(
                name: "GovtIdType",
                table: "Inquiries",
                newName: "GovernmentIdType");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "072de3e4-7ed3-45fd-9a10-8faf7997b4b4", "62ab270d-9e8d-47d2-8b53-6d40afa65240", "BankEmployee", "BANKEMPLOYEE" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "ca58ca26-4f81-41db-8c6d-2715eb0649c7", "0dda7e4e-fa5d-4e7b-b97c-8294197022f7", "Client", "CLIENT" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "072de3e4-7ed3-45fd-9a10-8faf7997b4b4");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ca58ca26-4f81-41db-8c6d-2715eb0649c7");

            migrationBuilder.RenameColumn(
                name: "GovernmentIdValue",
                table: "Inquiries",
                newName: "GovtIdValue");

            migrationBuilder.RenameColumn(
                name: "GovernmentIdType",
                table: "Inquiries",
                newName: "GovtIdType");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "250c7c93-ef39-49e8-b2ca-aa3ef8e75ef3", "b3ca825f-32b9-4c52-aed6-815b8f769f3c", "BankEmployee", "BANKEMPLOYEE" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "99767d13-7559-489e-befe-ab5f88c660ff", "6646fe5d-7d7a-4224-9dec-dafb579df1c3", "Client", "CLIENT" });
        }
    }
}
