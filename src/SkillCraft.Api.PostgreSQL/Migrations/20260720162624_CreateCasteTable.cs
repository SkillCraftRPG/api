using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SkillCraft.Api.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class CreateCasteTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Castes",
                schema: "Game",
                columns: table => new
                {
                    CasteId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WorldId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Summary = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    HtmlContent = table.Column<string>(type: "text", nullable: true),
                    Skill = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    WealthRoll = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    FeatureName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    FeatureHtmlContent = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Castes", x => x.CasteId);
                    table.ForeignKey(
                        name: "FK_Castes_Worlds_WorldId",
                        column: x => x.WorldId,
                        principalSchema: "Game",
                        principalTable: "Worlds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Castes_WorldId_CreatedBy",
                schema: "Game",
                table: "Castes",
                columns: new[] { "WorldId", "CreatedBy" });

            migrationBuilder.CreateIndex(
                name: "IX_Castes_WorldId_CreatedOn",
                schema: "Game",
                table: "Castes",
                columns: new[] { "WorldId", "CreatedOn" });

            migrationBuilder.CreateIndex(
                name: "IX_Castes_WorldId_FeatureName",
                schema: "Game",
                table: "Castes",
                columns: new[] { "WorldId", "FeatureName" });

            migrationBuilder.CreateIndex(
                name: "IX_Castes_WorldId_Id",
                schema: "Game",
                table: "Castes",
                columns: new[] { "WorldId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Castes_WorldId_Name",
                schema: "Game",
                table: "Castes",
                columns: new[] { "WorldId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_Castes_WorldId_Skill",
                schema: "Game",
                table: "Castes",
                columns: new[] { "WorldId", "Skill" });

            migrationBuilder.CreateIndex(
                name: "IX_Castes_WorldId_Summary",
                schema: "Game",
                table: "Castes",
                columns: new[] { "WorldId", "Summary" });

            migrationBuilder.CreateIndex(
                name: "IX_Castes_WorldId_UpdatedBy",
                schema: "Game",
                table: "Castes",
                columns: new[] { "WorldId", "UpdatedBy" });

            migrationBuilder.CreateIndex(
                name: "IX_Castes_WorldId_UpdatedOn",
                schema: "Game",
                table: "Castes",
                columns: new[] { "WorldId", "UpdatedOn" });

            migrationBuilder.CreateIndex(
                name: "IX_Castes_WorldId_Version",
                schema: "Game",
                table: "Castes",
                columns: new[] { "WorldId", "Version" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Castes",
                schema: "Game");
        }
    }
}
