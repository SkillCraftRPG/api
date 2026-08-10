using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillCraft.Api.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class ItemCategoryPriceWeightProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Weight",
                schema: "Game",
                table: "Items",
                type: "integer",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Price",
                schema: "Game",
                table: "Items",
                type: "integer",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                schema: "Game",
                table: "Items",
                type: "character varying(16)",
                maxLength: 16,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Properties",
                schema: "Game",
                table: "Items",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_WorldId_Category",
                schema: "Game",
                table: "Items",
                columns: new[] { "WorldId", "Category" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Items_WorldId_Category",
                schema: "Game",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Category",
                schema: "Game",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Properties",
                schema: "Game",
                table: "Items");

            migrationBuilder.AlterColumn<double>(
                name: "Weight",
                schema: "Game",
                table: "Items",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Price",
                schema: "Game",
                table: "Items",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }
    }
}
