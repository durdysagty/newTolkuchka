using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace newTolkuchka.Models.Migs
{
    /// <inheritdoc />
    public partial class RemoveNotNeededTablesAndColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Brands_BrandId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Categories_CategoryId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Lines_LineId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Types_TypeId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Warranties_WarrantyId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "CategoryProductAdLinks");

            migrationBuilder.DropIndex(
                name: "IX_Products_BrandId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_CategoryId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_LineId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_TypeId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_WarrantyId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "BrandId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "LineId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "TypeId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "WarrantyId",
                table: "Products");

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CategoryId", "TypeId", "WarrantyId" },
                values: new object[] { 11, 1, 2 });

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CategoryId", "TypeId", "WarrantyId" },
                values: new object[] { 11, 1, 2 });

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CategoryId", "TypeId", "WarrantyId" },
                values: new object[] { 11, 1, 2 });

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CategoryId", "TypeId", "WarrantyId" },
                values: new object[] { 11, 1, 2 });

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CategoryId", "TypeId", "WarrantyId" },
                values: new object[] { 178, 5, 1 });

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CategoryId", "TypeId", "WarrantyId" },
                values: new object[] { 11, 1, 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BrandId",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LineId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TypeId",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WarrantyId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CategoryProductAdLinks",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryProductAdLinks", x => new { x.CategoryId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_CategoryProductAdLinks_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryProductAdLinks_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id");
                });

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CategoryId", "TypeId", "WarrantyId" },
                values: new object[] { 0, 0, null });

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CategoryId", "TypeId", "WarrantyId" },
                values: new object[] { 0, 0, null });

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CategoryId", "TypeId", "WarrantyId" },
                values: new object[] { 0, 0, null });

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CategoryId", "TypeId", "WarrantyId" },
                values: new object[] { 0, 0, null });

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CategoryId", "TypeId", "WarrantyId" },
                values: new object[] { 0, 0, null });

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CategoryId", "TypeId", "WarrantyId" },
                values: new object[] { 0, 0, null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "BrandId", "CategoryId", "LineId", "TypeId", "WarrantyId" },
                values: new object[] { 1, 11, 1, 1, 2 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "BrandId", "CategoryId", "LineId", "TypeId", "WarrantyId" },
                values: new object[] { 1, 11, 1, 1, 2 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "BrandId", "CategoryId", "LineId", "TypeId", "WarrantyId" },
                values: new object[] { 1, 11, 1, 1, 2 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "BrandId", "CategoryId", "LineId", "TypeId", "WarrantyId" },
                values: new object[] { 1, 11, 1, 1, 2 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "BrandId", "CategoryId", "LineId", "TypeId", "WarrantyId" },
                values: new object[] { 1, 11, 1, 1, 2 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "BrandId", "CategoryId", "LineId", "TypeId", "WarrantyId" },
                values: new object[] { 1, 11, 1, 1, 2 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "BrandId", "CategoryId", "LineId", "TypeId", "WarrantyId" },
                values: new object[] { 1, 11, 1, 1, 2 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "BrandId", "CategoryId", "LineId", "TypeId", "WarrantyId" },
                values: new object[] { 1, 11, 1, 1, 2 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "BrandId", "CategoryId", "LineId", "TypeId", "WarrantyId" },
                values: new object[] { 1, 11, 1, 1, 2 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "BrandId", "CategoryId", "LineId", "TypeId", "WarrantyId" },
                values: new object[] { 1, 11, 1, 1, 2 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "BrandId", "CategoryId", "LineId", "TypeId", "WarrantyId" },
                values: new object[] { 1, 11, 1, 1, 2 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "BrandId", "CategoryId", "LineId", "TypeId", "WarrantyId" },
                values: new object[] { 1, 11, 1, 1, 2 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "BrandId", "CategoryId", "LineId", "TypeId", "WarrantyId" },
                values: new object[] { 1, 11, 1, 1, 2 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "BrandId", "CategoryId", "LineId", "TypeId", "WarrantyId" },
                values: new object[] { 1, 11, 1, 1, 2 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "BrandId", "CategoryId", "LineId", "TypeId", "WarrantyId" },
                values: new object[] { 1, 11, 1, 1, 2 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "BrandId", "CategoryId", "LineId", "TypeId", "WarrantyId" },
                values: new object[] { 1, 11, 1, 1, 2 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "BrandId", "CategoryId", "LineId", "TypeId", "WarrantyId" },
                values: new object[] { 1, 11, 1, 1, 2 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "BrandId", "CategoryId", "LineId", "TypeId", "WarrantyId" },
                values: new object[] { 1, 11, 1, 1, 2 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "BrandId", "CategoryId", "LineId", "TypeId", "WarrantyId" },
                values: new object[] { 1, 11, 1, 1, 2 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "BrandId", "CategoryId", "LineId", "TypeId", "WarrantyId" },
                values: new object[] { 1, 11, 1, 1, 2 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "BrandId", "CategoryId", "LineId", "TypeId", "WarrantyId" },
                values: new object[] { 1, 11, 1, 1, 2 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "BrandId", "CategoryId", "LineId", "TypeId", "WarrantyId" },
                values: new object[] { 6, 15, null, 5, 2 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "BrandId", "CategoryId", "LineId", "TypeId", "WarrantyId" },
                values: new object[] { 2, 11, 4, 1, 1 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "BrandId", "CategoryId", "LineId", "TypeId", "WarrantyId" },
                values: new object[] { 2, 11, 4, 1, 1 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "BrandId", "CategoryId", "LineId", "TypeId", "WarrantyId" },
                values: new object[] { 2, 11, 4, 1, 1 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "BrandId", "CategoryId", "LineId", "TypeId", "WarrantyId" },
                values: new object[] { 2, 11, 4, 1, 1 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "BrandId", "CategoryId", "LineId", "TypeId", "WarrantyId" },
                values: new object[] { 2, 11, 4, 1, 1 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "BrandId", "CategoryId", "LineId", "TypeId", "WarrantyId" },
                values: new object[] { 2, 11, 4, 1, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_Products_BrandId",
                table: "Products",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_LineId",
                table: "Products",
                column: "LineId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_TypeId",
                table: "Products",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_WarrantyId",
                table: "Products",
                column: "WarrantyId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryProductAdLinks_ProductId",
                table: "CategoryProductAdLinks",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Brands_BrandId",
                table: "Products",
                column: "BrandId",
                principalTable: "Brands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Categories_CategoryId",
                table: "Products",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Lines_LineId",
                table: "Products",
                column: "LineId",
                principalTable: "Lines",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Types_TypeId",
                table: "Products",
                column: "TypeId",
                principalTable: "Types",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Warranties_WarrantyId",
                table: "Products",
                column: "WarrantyId",
                principalTable: "Warranties",
                principalColumn: "Id");
        }
    }
}
