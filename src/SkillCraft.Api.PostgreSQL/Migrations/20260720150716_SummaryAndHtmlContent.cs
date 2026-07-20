using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillCraft.Api.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class SummaryAndHtmlContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                schema: "Game",
                table: "Worlds",
                newName: "HtmlContent");

            migrationBuilder.RenameColumn(
                name: "Description",
                schema: "Game",
                table: "Scripts",
                newName: "HtmlContent");

            migrationBuilder.RenameColumn(
                name: "Description",
                schema: "Game",
                table: "Languages",
                newName: "HtmlContent");

            migrationBuilder.RenameColumn(
                name: "Description",
                schema: "Game",
                table: "Customizations",
                newName: "HtmlContent");

            migrationBuilder.AddColumn<string>(
                name: "Summary",
                schema: "Game",
                table: "Scripts",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Summary",
                schema: "Game",
                table: "Languages",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Summary",
                schema: "Game",
                table: "Customizations",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Scripts_WorldId_Summary",
                schema: "Game",
                table: "Scripts",
                columns: new[] { "WorldId", "Summary" });

            migrationBuilder.CreateIndex(
                name: "IX_Languages_WorldId_Summary",
                schema: "Game",
                table: "Languages",
                columns: new[] { "WorldId", "Summary" });

            migrationBuilder.CreateIndex(
                name: "IX_Customizations_WorldId_Summary",
                schema: "Game",
                table: "Customizations",
                columns: new[] { "WorldId", "Summary" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Scripts_WorldId_Summary",
                schema: "Game",
                table: "Scripts");

            migrationBuilder.DropIndex(
                name: "IX_Languages_WorldId_Summary",
                schema: "Game",
                table: "Languages");

            migrationBuilder.DropIndex(
                name: "IX_Customizations_WorldId_Summary",
                schema: "Game",
                table: "Customizations");

            migrationBuilder.DropColumn(
                name: "Summary",
                schema: "Game",
                table: "Scripts");

            migrationBuilder.DropColumn(
                name: "Summary",
                schema: "Game",
                table: "Languages");

            migrationBuilder.DropColumn(
                name: "Summary",
                schema: "Game",
                table: "Customizations");

            migrationBuilder.RenameColumn(
                name: "HtmlContent",
                schema: "Game",
                table: "Worlds",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "HtmlContent",
                schema: "Game",
                table: "Scripts",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "HtmlContent",
                schema: "Game",
                table: "Languages",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "HtmlContent",
                schema: "Game",
                table: "Customizations",
                newName: "Description");
        }
    }
}
