using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoanComparer.Application.Migrations
{
    public partial class InquiriesAndOffers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "310ccae3-8559-4385-91dc-bb172a737428");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bba87489-0938-47dc-8210-f09b8c05fe99");

            migrationBuilder.CreateTable(
                name: "Inquiries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerUsername = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NotificationEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreationTimestamp = table.Column<long>(type: "bigint", nullable: false),
                    AmountRequestedAsSmallestNominal = table.Column<long>(type: "bigint", nullable: false),
                    NumberOfInstallments = table.Column<int>(type: "int", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BirthDateTimestamp = table.Column<long>(type: "bigint", nullable: false),
                    JobName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JobDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JobStartDateTimestamp = table.Column<long>(type: "bigint", nullable: true),
                    JobEndDateTimestamp = table.Column<long>(type: "bigint", nullable: true),
                    GovtIdType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GovtIdValue = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inquiries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Offers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoanValueAsSmallestNominal = table.Column<long>(type: "bigint", nullable: false),
                    NumberOfInstallments = table.Column<int>(type: "int", nullable: false),
                    Percentage = table.Column<double>(type: "float", nullable: false),
                    MonthlyInstallmentAsSmallestNominal = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Offers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InquiryStatuses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InquiryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    OfferId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InquiryStatuses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InquiryStatuses_Inquiries_InquiryId",
                        column: x => x.InquiryId,
                        principalTable: "Inquiries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InquiryStatuses_Offers_OfferId",
                        column: x => x.OfferId,
                        principalTable: "Offers",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "250c7c93-ef39-49e8-b2ca-aa3ef8e75ef3", "b3ca825f-32b9-4c52-aed6-815b8f769f3c", "BankEmployee", "BANKEMPLOYEE" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "99767d13-7559-489e-befe-ab5f88c660ff", "6646fe5d-7d7a-4224-9dec-dafb579df1c3", "Client", "CLIENT" });

            migrationBuilder.CreateIndex(
                name: "IX_InquiryStatuses_InquiryId",
                table: "InquiryStatuses",
                column: "InquiryId");

            migrationBuilder.CreateIndex(
                name: "IX_InquiryStatuses_OfferId",
                table: "InquiryStatuses",
                column: "OfferId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InquiryStatuses");

            migrationBuilder.DropTable(
                name: "Inquiries");

            migrationBuilder.DropTable(
                name: "Offers");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "250c7c93-ef39-49e8-b2ca-aa3ef8e75ef3");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "99767d13-7559-489e-befe-ab5f88c660ff");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "310ccae3-8559-4385-91dc-bb172a737428", "7eeaa674-eb66-4a01-b3f9-de5d38bbe621", "BankEmployee", "BANKEMPLOYEE" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "bba87489-0938-47dc-8210-f09b8c05fe99", "c77b2f83-dc8e-4220-96d0-02b465db7cc0", "Client", "CLIENT" });
        }
    }
}
