using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace newTolkuchka.Models.Migs
{
    /// <inheritdoc />
    public partial class modelsVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Warranties",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Types",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Suppliers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "SpecsValues",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "SpecsValueMods",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Specs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Slides",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Purchases",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "PurchaseInvoices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Promotions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Positions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Models",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Lines",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Invoices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Headings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Employees",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Currencies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Categories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Brands",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Articles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 1,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 2,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 3,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 4,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 5,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 6,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 8,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 9,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 10,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 11,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 12,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 13,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 15,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 17,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: 1,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: 2,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 2,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Lines",
                keyColumn: "Id",
                keyValue: 1,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Lines",
                keyColumn: "Id",
                keyValue: 2,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Lines",
                keyColumn: "Id",
                keyValue: 3,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Lines",
                keyColumn: "Id",
                keyValue: 4,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Lines",
                keyColumn: "Id",
                keyValue: 5,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Lines",
                keyColumn: "Id",
                keyValue: 6,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Lines",
                keyColumn: "Id",
                keyValue: 7,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Lines",
                keyColumn: "Id",
                keyValue: 8,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Lines",
                keyColumn: "Id",
                keyValue: 9,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 1,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 2,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 3,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 4,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 5,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 6,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Positions",
                keyColumn: "Id",
                keyValue: 1,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Positions",
                keyColumn: "Id",
                keyValue: 2,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Positions",
                keyColumn: "Id",
                keyValue: 3,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Positions",
                keyColumn: "Id",
                keyValue: 4,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 9,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 10,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 11,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 12,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 13,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 14,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 15,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 16,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 17,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 18,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 19,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 20,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 21,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 22,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 23,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 24,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 25,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 26,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 27,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 28,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Slides",
                keyColumn: "Id",
                keyValue: 1,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Slides",
                keyColumn: "Id",
                keyValue: 2,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Specs",
                keyColumn: "Id",
                keyValue: 1,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Specs",
                keyColumn: "Id",
                keyValue: 2,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Specs",
                keyColumn: "Id",
                keyValue: 3,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Specs",
                keyColumn: "Id",
                keyValue: 4,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Specs",
                keyColumn: "Id",
                keyValue: 5,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "SpecsValueMods",
                keyColumn: "Id",
                keyValue: 1,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "SpecsValueMods",
                keyColumn: "Id",
                keyValue: 2,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "SpecsValueMods",
                keyColumn: "Id",
                keyValue: 3,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "SpecsValueMods",
                keyColumn: "Id",
                keyValue: 4,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "SpecsValueMods",
                keyColumn: "Id",
                keyValue: 5,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "SpecsValues",
                keyColumn: "Id",
                keyValue: 1,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "SpecsValues",
                keyColumn: "Id",
                keyValue: 2,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "SpecsValues",
                keyColumn: "Id",
                keyValue: 3,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "SpecsValues",
                keyColumn: "Id",
                keyValue: 4,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "SpecsValues",
                keyColumn: "Id",
                keyValue: 5,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "SpecsValues",
                keyColumn: "Id",
                keyValue: 6,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "SpecsValues",
                keyColumn: "Id",
                keyValue: 7,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "SpecsValues",
                keyColumn: "Id",
                keyValue: 8,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "SpecsValues",
                keyColumn: "Id",
                keyValue: 9,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "SpecsValues",
                keyColumn: "Id",
                keyValue: 10,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "SpecsValues",
                keyColumn: "Id",
                keyValue: 11,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "SpecsValues",
                keyColumn: "Id",
                keyValue: 12,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "SpecsValues",
                keyColumn: "Id",
                keyValue: 13,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "SpecsValues",
                keyColumn: "Id",
                keyValue: 14,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "SpecsValues",
                keyColumn: "Id",
                keyValue: 15,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "SpecsValues",
                keyColumn: "Id",
                keyValue: 16,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "SpecsValues",
                keyColumn: "Id",
                keyValue: 17,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "SpecsValues",
                keyColumn: "Id",
                keyValue: 18,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "SpecsValues",
                keyColumn: "Id",
                keyValue: 19,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "SpecsValues",
                keyColumn: "Id",
                keyValue: 20,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "SpecsValues",
                keyColumn: "Id",
                keyValue: 21,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "SpecsValues",
                keyColumn: "Id",
                keyValue: 22,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "SpecsValues",
                keyColumn: "Id",
                keyValue: 23,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "SpecsValues",
                keyColumn: "Id",
                keyValue: 24,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "SpecsValues",
                keyColumn: "Id",
                keyValue: 25,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "SpecsValues",
                keyColumn: "Id",
                keyValue: 26,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "SpecsValues",
                keyColumn: "Id",
                keyValue: 27,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "SpecsValues",
                keyColumn: "Id",
                keyValue: 28,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "SpecsValues",
                keyColumn: "Id",
                keyValue: 29,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "SpecsValues",
                keyColumn: "Id",
                keyValue: 30,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "SpecsValues",
                keyColumn: "Id",
                keyValue: 31,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "SpecsValues",
                keyColumn: "Id",
                keyValue: 32,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "SpecsValues",
                keyColumn: "Id",
                keyValue: 33,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "SpecsValues",
                keyColumn: "Id",
                keyValue: 34,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "SpecsValues",
                keyColumn: "Id",
                keyValue: 35,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "SpecsValues",
                keyColumn: "Id",
                keyValue: 36,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "SpecsValues",
                keyColumn: "Id",
                keyValue: 37,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "SpecsValues",
                keyColumn: "Id",
                keyValue: 38,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "SpecsValues",
                keyColumn: "Id",
                keyValue: 39,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "SpecsValues",
                keyColumn: "Id",
                keyValue: 40,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "SpecsValues",
                keyColumn: "Id",
                keyValue: 41,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "SpecsValues",
                keyColumn: "Id",
                keyValue: 42,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "SpecsValues",
                keyColumn: "Id",
                keyValue: 43,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "SpecsValues",
                keyColumn: "Id",
                keyValue: 44,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "SpecsValues",
                keyColumn: "Id",
                keyValue: 45,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "SpecsValues",
                keyColumn: "Id",
                keyValue: 46,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "SpecsValues",
                keyColumn: "Id",
                keyValue: 47,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "SpecsValues",
                keyColumn: "Id",
                keyValue: 48,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "SpecsValues",
                keyColumn: "Id",
                keyValue: 49,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "SpecsValues",
                keyColumn: "Id",
                keyValue: 50,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "SpecsValues",
                keyColumn: "Id",
                keyValue: 51,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "SpecsValues",
                keyColumn: "Id",
                keyValue: 52,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "SpecsValues",
                keyColumn: "Id",
                keyValue: 53,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: 1,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: 2,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Types",
                keyColumn: "Id",
                keyValue: 1,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Types",
                keyColumn: "Id",
                keyValue: 2,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Types",
                keyColumn: "Id",
                keyValue: 3,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Types",
                keyColumn: "Id",
                keyValue: 4,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Types",
                keyColumn: "Id",
                keyValue: 5,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Warranties",
                keyColumn: "Id",
                keyValue: 1,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Warranties",
                keyColumn: "Id",
                keyValue: 2,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Warranties",
                keyColumn: "Id",
                keyValue: 3,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Warranties",
                keyColumn: "Id",
                keyValue: 4,
                column: "Version",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Warranties",
                keyColumn: "Id",
                keyValue: 5,
                column: "Version",
                value: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Version",
                table: "Warranties");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Types");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "SpecsValues");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "SpecsValueMods");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Specs");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Slides");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "PurchaseInvoices");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Promotions");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Positions");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Models");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Lines");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Headings");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Currencies");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Brands");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Articles");
        }
    }
}
