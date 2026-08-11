using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillCraft.Api.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class ItemChargesReplacement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChargesDepletionBehavior",
                schema: "Game",
                table: "Items",
                type: "character varying(8)",
                maxLength: 8,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaximumCharges",
                schema: "Game",
                table: "Items",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReplacementId",
                schema: "Game",
                table: "Items",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_ReplacementId",
                schema: "Game",
                table: "Items",
                column: "ReplacementId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_WorldId_ReplacementId",
                schema: "Game",
                table: "Items",
                columns: new[] { "WorldId", "ReplacementId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Items_ReplacementId",
                schema: "Game",
                table: "Items",
                column: "ReplacementId",
                principalSchema: "Game",
                principalTable: "Items",
                principalColumn: "ItemId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Items_ReplacementId",
                schema: "Game",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_ReplacementId",
                schema: "Game",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_WorldId_ReplacementId",
                schema: "Game",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "ChargesDepletionBehavior",
                schema: "Game",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "MaximumCharges",
                schema: "Game",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "ReplacementId",
                schema: "Game",
                table: "Items");
        }
    }
}
