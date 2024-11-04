using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Finance.Domain.Migrations
{
    /// <inheritdoc />
    public partial class TableNameSingularization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CreditCardMovements_CreditCard_CreditCardId",
                table: "CreditCardMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_IOLInvestmentAssets_IOLInvestmentAssetTypes_TypeId",
                table: "IOLInvestmentAssets");

            migrationBuilder.DropForeignKey(
                name: "FK_IOLInvestments_IOLInvestmentAssets_AssetId",
                table: "IOLInvestments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IOLInvestments",
                table: "IOLInvestments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IOLInvestmentAssetTypes",
                table: "IOLInvestmentAssetTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IOLInvestmentAssets",
                table: "IOLInvestmentAssets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CreditCardMovements",
                table: "CreditCardMovements");

            migrationBuilder.RenameTable(
                name: "IOLInvestments",
                newName: "IOLInvestment");

            migrationBuilder.RenameTable(
                name: "IOLInvestmentAssetTypes",
                newName: "IOLInvestmentAssetType");

            migrationBuilder.RenameTable(
                name: "IOLInvestmentAssets",
                newName: "IOLInvestmentAsset");

            migrationBuilder.RenameTable(
                name: "CreditCardMovements",
                newName: "CreditCardMovement");

            migrationBuilder.RenameIndex(
                name: "IX_IOLInvestments_AssetId",
                table: "IOLInvestment",
                newName: "IX_IOLInvestment_AssetId");

            migrationBuilder.RenameIndex(
                name: "IX_IOLInvestmentAssets_TypeId",
                table: "IOLInvestmentAsset",
                newName: "IX_IOLInvestmentAsset_TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_CreditCardMovements_CreditCardId",
                table: "CreditCardMovement",
                newName: "IX_CreditCardMovement_CreditCardId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IOLInvestment",
                table: "IOLInvestment",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IOLInvestmentAssetType",
                table: "IOLInvestmentAssetType",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IOLInvestmentAsset",
                table: "IOLInvestmentAsset",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CreditCardMovement",
                table: "CreditCardMovement",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCardMovement_CreditCard_CreditCardId",
                table: "CreditCardMovement",
                column: "CreditCardId",
                principalTable: "CreditCard",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IOLInvestment_IOLInvestmentAsset_AssetId",
                table: "IOLInvestment",
                column: "AssetId",
                principalTable: "IOLInvestmentAsset",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IOLInvestmentAsset_IOLInvestmentAssetType_TypeId",
                table: "IOLInvestmentAsset",
                column: "TypeId",
                principalTable: "IOLInvestmentAssetType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CreditCardMovement_CreditCard_CreditCardId",
                table: "CreditCardMovement");

            migrationBuilder.DropForeignKey(
                name: "FK_IOLInvestment_IOLInvestmentAsset_AssetId",
                table: "IOLInvestment");

            migrationBuilder.DropForeignKey(
                name: "FK_IOLInvestmentAsset_IOLInvestmentAssetType_TypeId",
                table: "IOLInvestmentAsset");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IOLInvestmentAssetType",
                table: "IOLInvestmentAssetType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IOLInvestmentAsset",
                table: "IOLInvestmentAsset");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IOLInvestment",
                table: "IOLInvestment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CreditCardMovement",
                table: "CreditCardMovement");

            migrationBuilder.RenameTable(
                name: "IOLInvestmentAssetType",
                newName: "IOLInvestmentAssetTypes");

            migrationBuilder.RenameTable(
                name: "IOLInvestmentAsset",
                newName: "IOLInvestmentAssets");

            migrationBuilder.RenameTable(
                name: "IOLInvestment",
                newName: "IOLInvestments");

            migrationBuilder.RenameTable(
                name: "CreditCardMovement",
                newName: "CreditCardMovements");

            migrationBuilder.RenameIndex(
                name: "IX_IOLInvestmentAsset_TypeId",
                table: "IOLInvestmentAssets",
                newName: "IX_IOLInvestmentAssets_TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_IOLInvestment_AssetId",
                table: "IOLInvestments",
                newName: "IX_IOLInvestments_AssetId");

            migrationBuilder.RenameIndex(
                name: "IX_CreditCardMovement_CreditCardId",
                table: "CreditCardMovements",
                newName: "IX_CreditCardMovements_CreditCardId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IOLInvestmentAssetTypes",
                table: "IOLInvestmentAssetTypes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IOLInvestmentAssets",
                table: "IOLInvestmentAssets",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IOLInvestments",
                table: "IOLInvestments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CreditCardMovements",
                table: "CreditCardMovements",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCardMovements_CreditCard_CreditCardId",
                table: "CreditCardMovements",
                column: "CreditCardId",
                principalTable: "CreditCard",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IOLInvestmentAssets_IOLInvestmentAssetTypes_TypeId",
                table: "IOLInvestmentAssets",
                column: "TypeId",
                principalTable: "IOLInvestmentAssetTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IOLInvestments_IOLInvestmentAssets_AssetId",
                table: "IOLInvestments",
                column: "AssetId",
                principalTable: "IOLInvestmentAssets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
