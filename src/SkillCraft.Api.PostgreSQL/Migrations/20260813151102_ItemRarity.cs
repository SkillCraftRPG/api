using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillCraft.Api.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class ItemRarity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Rarity",
                schema: "Game",
                table: "Items",
                type: "character varying(16)",
                maxLength: 16,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_WorldId_Rarity",
                schema: "Game",
                table: "Items",
                columns: new[] { "WorldId", "Rarity" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Items_WorldId_Rarity",
                schema: "Game",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Rarity",
                schema: "Game",
                table: "Items");
        }
    }
}
