using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace newTolkuchka.Models.Migs
{
    /// <inheritdoc />
    public partial class modelIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Models_LineId_TypeId_Name",
                table: "Models");

            migrationBuilder.CreateIndex(
                name: "IX_Models_LineId_TypeId_BrandId_Name",
                table: "Models",
                columns: new[] { "LineId", "TypeId", "BrandId", "Name" },
                unique: true,
                filter: "[LineId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Models_LineId_TypeId_BrandId_Name",
                table: "Models");

            migrationBuilder.CreateIndex(
                name: "IX_Models_LineId_TypeId_Name",
                table: "Models",
                columns: new[] { "LineId", "TypeId", "Name" },
                unique: true,
                filter: "[LineId] IS NOT NULL");
        }
    }
}
