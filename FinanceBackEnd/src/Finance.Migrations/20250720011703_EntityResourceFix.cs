using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Finance.Domain.Migrations
{
    /// <inheritdoc />
    public partial class EntityResourceFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IOLInvestmentAssetTypeResource_IOLInvestmentAssetType_Resou~",
                table: "IOLInvestmentAssetTypeResource");

            migrationBuilder.DropIndex(
                name: "IX_IOLInvestmentAssetTypeResource_ResourceSourceId1",
                table: "IOLInvestmentAssetTypeResource");

            // Eliminar la columna incorrecta
            migrationBuilder.DropColumn(
                name: "ResourceSourceId",
                table: "IOLInvestmentAssetTypeResource");

            // Renombrar la columna correcta
            migrationBuilder.RenameColumn(
                name: "ResourceSourceId1",
                table: "IOLInvestmentAssetTypeResource",
                newName: "ResourceSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_IOLInvestmentAssetTypeResource_ResourceSourceId",
                table: "IOLInvestmentAssetTypeResource",
                column: "ResourceSourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_IOLInvestmentAssetTypeResource_IOLInvestmentAssetType_Resou~",
                table: "IOLInvestmentAssetTypeResource",
                column: "ResourceSourceId",
                principalTable: "IOLInvestmentAssetType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IOLInvestmentAssetTypeResource_IOLInvestmentAssetType_Resou~",
                table: "IOLInvestmentAssetTypeResource");

            migrationBuilder.DropIndex(
                name: "IX_IOLInvestmentAssetTypeResource_ResourceSourceId",
                table: "IOLInvestmentAssetTypeResource");

            // Renombrar la columna de vuelta
            migrationBuilder.RenameColumn(
                name: "ResourceSourceId",
                table: "IOLInvestmentAssetTypeResource",
                newName: "ResourceSourceId1");

            // Volver a crear la columna eliminada
            migrationBuilder.AddColumn<Guid>(
                name: "ResourceSourceId",
                table: "IOLInvestmentAssetTypeResource",
                type: "uuid",
                nullable: false,
                defaultValue: Guid.Empty);

            migrationBuilder.CreateIndex(
                name: "IX_IOLInvestmentAssetTypeResource_ResourceSourceId1",
                table: "IOLInvestmentAssetTypeResource",
                column: "ResourceSourceId1");

            migrationBuilder.AddForeignKey(
                name: "FK_IOLInvestmentAssetTypeResource_IOLInvestmentAssetType_Resou~",
                table: "IOLInvestmentAssetTypeResource",
                column: "ResourceSourceId1",
                principalTable: "IOLInvestmentAssetType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
