using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillCraft.Api.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class LineageEventSourcingFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Languages_Lineages_LineageEntityLineageId",
                schema: "Game",
                table: "Languages");

            migrationBuilder.DropIndex(
                name: "IX_Languages_LineageEntityLineageId",
                schema: "Game",
                table: "Languages");

            migrationBuilder.DropColumn(
                name: "LineageEntityLineageId",
                schema: "Game",
                table: "Languages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LineageEntityLineageId",
                schema: "Game",
                table: "Languages",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Languages_LineageEntityLineageId",
                schema: "Game",
                table: "Languages",
                column: "LineageEntityLineageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Languages_Lineages_LineageEntityLineageId",
                schema: "Game",
                table: "Languages",
                column: "LineageEntityLineageId",
                principalSchema: "Game",
                principalTable: "Lineages",
                principalColumn: "LineageId");
        }
    }
}
