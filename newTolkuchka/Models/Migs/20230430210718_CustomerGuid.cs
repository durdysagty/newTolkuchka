using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace newTolkuchka.Models.Migs
{
    /// <inheritdoc />
    public partial class CustomerGuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CustomerGuidId",
                table: "Invoices",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CustomerGuids",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeniedInvoices = table.Column<int>(type: "int", nullable: false),
                    IsBanned = table.Column<bool>(type: "bit", nullable: false),
                    BannedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerGuids", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_CustomerGuidId",
                table: "Invoices",
                column: "CustomerGuidId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_CustomerGuids_CustomerGuidId",
                table: "Invoices",
                column: "CustomerGuidId",
                principalTable: "CustomerGuids",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_CustomerGuids_CustomerGuidId",
                table: "Invoices");

            migrationBuilder.DropTable(
                name: "CustomerGuids");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_CustomerGuidId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "CustomerGuidId",
                table: "Invoices");
        }
    }
}
