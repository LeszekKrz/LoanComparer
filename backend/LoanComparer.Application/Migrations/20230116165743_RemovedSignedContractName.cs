using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoanComparer.Application.Migrations
{
    public partial class RemovedSignedContractName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a61c6a0c-8b79-4794-b269-c32e363cf3b2");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a6a1773f-fc37-4f96-8090-3bf82a113202");

            migrationBuilder.DropColumn(
                name: "SignedContractName",
                table: "Offers");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "132e3c63-e4a2-4fbe-992d-a97ecd16f605", "6f4ee3fe-095c-4661-b79d-b3aaab6fcd2a", "Client", "CLIENT" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "4d6e423f-67c6-4d64-97ae-3b1ba826fc36", "c528c2c8-493c-4c6a-82cb-0d7b01dedf67", "BankEmployee", "BANKEMPLOYEE" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "132e3c63-e4a2-4fbe-992d-a97ecd16f605");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4d6e423f-67c6-4d64-97ae-3b1ba826fc36");

            migrationBuilder.AddColumn<string>(
                name: "SignedContractName",
                table: "Offers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "a61c6a0c-8b79-4794-b269-c32e363cf3b2", "cdf334c2-9701-4241-80e4-ed05c3128873", "Client", "CLIENT" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "a6a1773f-fc37-4f96-8090-3bf82a113202", "3746ab41-7daa-4134-abf3-399f5856cc9b", "BankEmployee", "BANKEMPLOYEE" });
        }
    }
}
