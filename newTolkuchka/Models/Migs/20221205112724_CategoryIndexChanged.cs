using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace newTolkuchka.Models.Migs
{
    /// <inheritdoc />
    public partial class CategoryIndexChanged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Categories_NameEn",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_NameRu",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_NameTm",
                table: "Categories");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ParentId_NameEn",
                table: "Categories",
                columns: new[] { "ParentId", "NameEn" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ParentId_NameRu",
                table: "Categories",
                columns: new[] { "ParentId", "NameRu" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ParentId_NameTm",
                table: "Categories",
                columns: new[] { "ParentId", "NameTm" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Categories_ParentId_NameEn",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_ParentId_NameRu",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_ParentId_NameTm",
                table: "Categories");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_NameEn",
                table: "Categories",
                column: "NameEn",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_NameRu",
                table: "Categories",
                column: "NameRu",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_NameTm",
                table: "Categories",
                column: "NameTm",
                unique: true);
        }
    }
}
