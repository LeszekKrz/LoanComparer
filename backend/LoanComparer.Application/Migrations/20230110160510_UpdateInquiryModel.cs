using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoanComparer.Application.Migrations
{
    public partial class UpdateInquiryModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "072de3e4-7ed3-45fd-9a10-8faf7997b4b4");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ca58ca26-4f81-41db-8c6d-2715eb0649c7");

            migrationBuilder.AddColumn<long>(
                name: "IncomeLevelAsSmallestNominal",
                table: "Inquiries",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "895b0c75-32b8-48f9-ad58-c6bb011780fe", "d780c420-e788-4f0b-8537-184b0926b811", "BankEmployee", "BANKEMPLOYEE" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "f0f4076a-331b-4118-b3fa-f6dcc9ba1b46", "395a07b2-3423-490e-87f8-00481492d9d7", "Client", "CLIENT" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "895b0c75-32b8-48f9-ad58-c6bb011780fe");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f0f4076a-331b-4118-b3fa-f6dcc9ba1b46");

            migrationBuilder.DropColumn(
                name: "IncomeLevelAsSmallestNominal",
                table: "Inquiries");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "072de3e4-7ed3-45fd-9a10-8faf7997b4b4", "62ab270d-9e8d-47d2-8b53-6d40afa65240", "BankEmployee", "BANKEMPLOYEE" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "ca58ca26-4f81-41db-8c6d-2715eb0649c7", "0dda7e4e-fa5d-4e7b-b97c-8294197022f7", "Client", "CLIENT" });
        }
    }
}
