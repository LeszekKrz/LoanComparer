using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoanComparer.Application.Migrations
{
    public partial class AddedSignedContractToTheOffer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "26dec652-87e1-47d6-a44e-c20f84b6b13f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e432809e-d894-4ab6-8480-57c702b7d74c");

            migrationBuilder.AddColumn<byte[]>(
                name: "SignedContractContent",
                table: "Offers",
                type: "varbinary(max)",
                nullable: true);

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

        protected override void Down(MigrationBuilder migrationBuilder)
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
                name: "SignedContractContent",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "SignedContractName",
                table: "Offers");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "26dec652-87e1-47d6-a44e-c20f84b6b13f", "d74cc774-15c7-425e-93ff-d1521df68d74", "BankEmployee", "BANKEMPLOYEE" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "e432809e-d894-4ab6-8480-57c702b7d74c", "81f1a4a3-22f0-464d-bb50-3453fba4e789", "Client", "CLIENT" });
        }
    }
}
