using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoanComparer.Application.Migrations
{
    public partial class GovernmentIdIsOptional : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d7dacdc8-6ea9-4138-9ca6-0eb1163e26a2");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e871e297-8a60-460b-ba0e-8213e11066d1");

            migrationBuilder.AlterColumn<string>(
                name: "GovernmentIdValue",
                table: "AspNetUsers",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(11)",
                oldMaxLength: 11);

            migrationBuilder.AlterColumn<string>(
                name: "GovernmentIdType",
                table: "AspNetUsers",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "29aae5dc-bb88-4907-a34b-ad872be101ff", "e61ddd9d-d508-4fb6-83d2-ffe3fb69fbde", "Client", "CLIENT" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "51a1a7a1-a83a-48ca-b748-25c72aacb0f9", "7fe14097-9e7c-49ed-8f56-17ed648f6066", "BankEmployee", "BANKEMPLOYEE" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "29aae5dc-bb88-4907-a34b-ad872be101ff");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "51a1a7a1-a83a-48ca-b748-25c72aacb0f9");

            migrationBuilder.AlterColumn<string>(
                name: "GovernmentIdValue",
                table: "AspNetUsers",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(11)",
                oldMaxLength: 11,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "GovernmentIdType",
                table: "AspNetUsers",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15,
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "d7dacdc8-6ea9-4138-9ca6-0eb1163e26a2", "d61af7af-4a86-4e31-be7e-c354de8136af", "BankEmployee", "BANKEMPLOYEE" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "e871e297-8a60-460b-ba0e-8213e11066d1", "32bb333b-a59f-4346-8233-2ea84dff7448", "Client", "CLIENT" });
        }
    }
}
