using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace newTolkuchka.Models.Migs
{
    /// <inheritdoc />
    public partial class ModelUniqIndexChanged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Models_LineId_Name",
                table: "Models");

            migrationBuilder.CreateIndex(
                name: "IX_Models_LineId_TypeId_Name",
                table: "Models",
                columns: new[] { "LineId", "TypeId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Models_LineId_TypeId_Name",
                table: "Models");

            migrationBuilder.CreateIndex(
                name: "IX_Models_LineId_Name",
                table: "Models",
                columns: new[] { "LineId", "Name" },
                unique: true,
                filter: "[LineId] IS NOT NULL");
        }
    }
}
