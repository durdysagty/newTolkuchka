using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace newTolkuchka.Models.Migs
{
    /// <inheritdoc />
    public partial class FromProductToModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Models",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "TypeId",
                table: "Models",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "WarrantyId",
                table: "Models",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CategoryModelAdLinks",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    ModelId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryModelAdLinks", x => new { x.CategoryId, x.ModelId });
                    table.ForeignKey(
                        name: "FK_CategoryModelAdLinks_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryModelAdLinks_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id");
                });

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CategoryId", "TypeId", "WarrantyId" },
                values: new object[] { 1, 1, null });

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CategoryId", "TypeId", "WarrantyId" },
                values: new object[] { 1, 1, null });

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CategoryId", "TypeId", "WarrantyId" },
                values: new object[] { 1, 1, null });

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CategoryId", "TypeId", "WarrantyId" },
                values: new object[] { 1, 1, null });

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CategoryId", "TypeId", "WarrantyId" },
                values: new object[] { 1, 1, null });

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CategoryId", "TypeId", "WarrantyId" },
                values: new object[] { 1, 1, null });

            migrationBuilder.CreateIndex(
                name: "IX_Models_CategoryId",
                table: "Models",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Models_TypeId",
                table: "Models",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Models_WarrantyId",
                table: "Models",
                column: "WarrantyId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryModelAdLinks_ModelId",
                table: "CategoryModelAdLinks",
                column: "ModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Models_Categories_CategoryId",
                table: "Models",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Models_Types_TypeId",
                table: "Models",
                column: "TypeId",
                principalTable: "Types",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Models_Warranties_WarrantyId",
                table: "Models",
                column: "WarrantyId",
                principalTable: "Warranties",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Models_Categories_CategoryId",
                table: "Models");

            migrationBuilder.DropForeignKey(
                name: "FK_Models_Types_TypeId",
                table: "Models");

            migrationBuilder.DropForeignKey(
                name: "FK_Models_Warranties_WarrantyId",
                table: "Models");

            migrationBuilder.DropTable(
                name: "CategoryModelAdLinks");

            migrationBuilder.DropIndex(
                name: "IX_Models_CategoryId",
                table: "Models");

            migrationBuilder.DropIndex(
                name: "IX_Models_TypeId",
                table: "Models");

            migrationBuilder.DropIndex(
                name: "IX_Models_WarrantyId",
                table: "Models");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Models");

            migrationBuilder.DropColumn(
                name: "TypeId",
                table: "Models");

            migrationBuilder.DropColumn(
                name: "WarrantyId",
                table: "Models");
        }
    }
}
