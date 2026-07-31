using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillCraft.Api.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class RenamedHtmlContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HtmlContent",
                schema: "Game",
                table: "Worlds",
                newName: "Content");

            migrationBuilder.RenameColumn(
                name: "HtmlContent",
                schema: "Game",
                table: "Talents",
                newName: "Content");

            migrationBuilder.RenameColumn(
                name: "HtmlContent",
                schema: "Game",
                table: "Scripts",
                newName: "Content");

            migrationBuilder.RenameColumn(
                name: "HtmlContent",
                schema: "Game",
                table: "Languages",
                newName: "Content");

            migrationBuilder.RenameColumn(
                name: "HtmlContent",
                schema: "Game",
                table: "Educations",
                newName: "FeatureContent");

            migrationBuilder.RenameColumn(
                name: "FeatureHtmlContent",
                schema: "Game",
                table: "Educations",
                newName: "Content");

            migrationBuilder.RenameColumn(
                name: "HtmlContent",
                schema: "Game",
                table: "Customizations",
                newName: "Content");

            migrationBuilder.RenameColumn(
                name: "HtmlContent",
                schema: "Game",
                table: "Castes",
                newName: "FeatureContent");

            migrationBuilder.RenameColumn(
                name: "FeatureHtmlContent",
                schema: "Game",
                table: "Castes",
                newName: "Content");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Content",
                schema: "Game",
                table: "Worlds",
                newName: "HtmlContent");

            migrationBuilder.RenameColumn(
                name: "Content",
                schema: "Game",
                table: "Talents",
                newName: "HtmlContent");

            migrationBuilder.RenameColumn(
                name: "Content",
                schema: "Game",
                table: "Scripts",
                newName: "HtmlContent");

            migrationBuilder.RenameColumn(
                name: "Content",
                schema: "Game",
                table: "Languages",
                newName: "HtmlContent");

            migrationBuilder.RenameColumn(
                name: "FeatureContent",
                schema: "Game",
                table: "Educations",
                newName: "HtmlContent");

            migrationBuilder.RenameColumn(
                name: "Content",
                schema: "Game",
                table: "Educations",
                newName: "FeatureHtmlContent");

            migrationBuilder.RenameColumn(
                name: "Content",
                schema: "Game",
                table: "Customizations",
                newName: "HtmlContent");

            migrationBuilder.RenameColumn(
                name: "FeatureContent",
                schema: "Game",
                table: "Castes",
                newName: "HtmlContent");

            migrationBuilder.RenameColumn(
                name: "Content",
                schema: "Game",
                table: "Castes",
                newName: "FeatureHtmlContent");
        }
    }
}
