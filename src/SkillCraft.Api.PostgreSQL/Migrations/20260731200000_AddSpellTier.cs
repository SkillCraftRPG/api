using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillCraft.Api.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class AddSpellTier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Tier",
                schema: "Game",
                table: "Spells",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Spells_WorldId_Tier",
                schema: "Game",
                table: "Spells",
                columns: new[] { "WorldId", "Tier" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Spells_WorldId_Tier",
                schema: "Game",
                table: "Spells");

            migrationBuilder.DropColumn(
                name: "Tier",
                schema: "Game",
                table: "Spells");
        }
    }
}
