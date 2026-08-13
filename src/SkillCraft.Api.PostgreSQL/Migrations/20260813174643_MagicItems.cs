using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillCraft.Api.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class MagicItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AttunementRequirements",
                schema: "Game",
                table: "Items",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAttunementRequired",
                schema: "Game",
                table: "Items",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsMagic",
                schema: "Game",
                table: "Items",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Items_WorldId_IsAttunementRequired",
                schema: "Game",
                table: "Items",
                columns: new[] { "WorldId", "IsAttunementRequired" });

            migrationBuilder.CreateIndex(
                name: "IX_Items_WorldId_IsMagic",
                schema: "Game",
                table: "Items",
                columns: new[] { "WorldId", "IsMagic" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Items_WorldId_IsAttunementRequired",
                schema: "Game",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_WorldId_IsMagic",
                schema: "Game",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "AttunementRequirements",
                schema: "Game",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "IsAttunementRequired",
                schema: "Game",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "IsMagic",
                schema: "Game",
                table: "Items");
        }
    }
}
