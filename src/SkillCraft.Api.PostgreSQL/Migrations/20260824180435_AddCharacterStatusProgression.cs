using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillCraft.Api.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class AddCharacterStatusProgression : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BloodAlcoholContent",
                schema: "Game",
                table: "Characters",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrentHope",
                schema: "Game",
                table: "Characters",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrentVitality",
                schema: "Game",
                table: "Characters",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Experience",
                schema: "Game",
                table: "Characters",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Intoxication",
                schema: "Game",
                table: "Characters",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Level",
                schema: "Game",
                table: "Characters",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaximumHope",
                schema: "Game",
                table: "Characters",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Stamina",
                schema: "Game",
                table: "Characters",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StunDamage",
                schema: "Game",
                table: "Characters",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TemporaryVitality",
                schema: "Game",
                table: "Characters",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Tier",
                schema: "Game",
                table: "Characters",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BloodAlcoholContent",
                schema: "Game",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "CurrentHope",
                schema: "Game",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "CurrentVitality",
                schema: "Game",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "Experience",
                schema: "Game",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "Intoxication",
                schema: "Game",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "Level",
                schema: "Game",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "MaximumHope",
                schema: "Game",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "Stamina",
                schema: "Game",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "StunDamage",
                schema: "Game",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "TemporaryVitality",
                schema: "Game",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "Tier",
                schema: "Game",
                table: "Characters");
        }
    }
}
