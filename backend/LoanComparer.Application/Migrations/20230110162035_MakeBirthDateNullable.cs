using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoanComparer.Application.Migrations
{
    public partial class MakeBirthDateNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "895b0c75-32b8-48f9-ad58-c6bb011780fe");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f0f4076a-331b-4118-b3fa-f6dcc9ba1b46");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "050d2178-18c6-4b48-94c9-3b42b4b7899d", "c7b7cb3c-2f85-4f7e-be48-751a6a111be3", "BankEmployee", "BANKEMPLOYEE" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "b063b9c6-7cdf-4f56-9cae-846538c40f4c", "89fcf395-c410-44fc-b4a1-2972472b5e8b", "Client", "CLIENT" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "050d2178-18c6-4b48-94c9-3b42b4b7899d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b063b9c6-7cdf-4f56-9cae-846538c40f4c");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "895b0c75-32b8-48f9-ad58-c6bb011780fe", "d780c420-e788-4f0b-8537-184b0926b811", "BankEmployee", "BANKEMPLOYEE" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "f0f4076a-331b-4118-b3fa-f6dcc9ba1b46", "395a07b2-3423-490e-87f8-00481492d9d7", "Client", "CLIENT" });
        }
    }
}
