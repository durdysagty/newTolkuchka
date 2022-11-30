using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace newTolkuchka.Models.Migs
{
    /// <inheritdoc />
    public partial class InvoiceLanguage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "Invoices",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: false,
                defaultValue: "ru");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Language",
                table: "Invoices");
        }
    }
}
