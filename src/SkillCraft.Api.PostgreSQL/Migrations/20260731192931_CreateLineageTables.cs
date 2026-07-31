using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SkillCraft.Api.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class CreateLineageTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Lineages",
                schema: "Game",
                columns: table => new
                {
                    LineageId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WorldId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentId = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Summary = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Content = table.Column<string>(type: "text", nullable: true),
                    ExtraLanguages = table.Column<int>(type: "integer", nullable: false),
                    LanguagesContent = table.Column<string>(type: "text", nullable: true),
                    FamilyNames = table.Column<string>(type: "text", nullable: true),
                    FemaleNames = table.Column<string>(type: "text", nullable: true),
                    MaleNames = table.Column<string>(type: "text", nullable: true),
                    UnisexNames = table.Column<string>(type: "text", nullable: true),
                    CustomNames = table.Column<string>(type: "text", nullable: true),
                    NamesContent = table.Column<string>(type: "text", nullable: true),
                    Walk = table.Column<int>(type: "integer", nullable: true),
                    Climb = table.Column<int>(type: "integer", nullable: true),
                    Swim = table.Column<int>(type: "integer", nullable: true),
                    Fly = table.Column<int>(type: "integer", nullable: true),
                    Hover = table.Column<bool>(type: "boolean", nullable: false),
                    Burrow = table.Column<int>(type: "integer", nullable: true),
                    SizeCategory = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    HeightRoll = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Malnutrition = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Skinny = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    NormalWeight = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Overweight = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Obese = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Teenager = table.Column<int>(type: "integer", nullable: true),
                    Adult = table.Column<int>(type: "integer", nullable: true),
                    Mature = table.Column<int>(type: "integer", nullable: true),
                    Venerable = table.Column<int>(type: "integer", nullable: true),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lineages", x => x.LineageId);
                    table.ForeignKey(
                        name: "FK_Lineages_Lineages_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "Game",
                        principalTable: "Lineages",
                        principalColumn: "LineageId");
                    table.ForeignKey(
                        name: "FK_Lineages_Worlds_WorldId",
                        column: x => x.WorldId,
                        principalSchema: "Game",
                        principalTable: "Worlds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LineageFeatures",
                schema: "Game",
                columns: table => new
                {
                    LineageFeatureId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LineageId = table.Column<int>(type: "integer", nullable: false),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Content = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineageFeatures", x => x.LineageFeatureId);
                    table.ForeignKey(
                        name: "FK_LineageFeatures_Lineages_LineageId",
                        column: x => x.LineageId,
                        principalSchema: "Game",
                        principalTable: "Lineages",
                        principalColumn: "LineageId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LineageLanguages",
                schema: "Game",
                columns: table => new
                {
                    LineageId = table.Column<int>(type: "integer", nullable: false),
                    LanguageId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineageLanguages", x => new { x.LineageId, x.LanguageId });
                    table.ForeignKey(
                        name: "FK_LineageLanguages_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalSchema: "Game",
                        principalTable: "Languages",
                        principalColumn: "LanguageId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LineageLanguages_Lineages_LineageId",
                        column: x => x.LineageId,
                        principalSchema: "Game",
                        principalTable: "Lineages",
                        principalColumn: "LineageId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LineageFeatures_LineageId_CreatedBy",
                schema: "Game",
                table: "LineageFeatures",
                columns: new[] { "LineageId", "CreatedBy" });

            migrationBuilder.CreateIndex(
                name: "IX_LineageFeatures_LineageId_CreatedOn",
                schema: "Game",
                table: "LineageFeatures",
                columns: new[] { "LineageId", "CreatedOn" });

            migrationBuilder.CreateIndex(
                name: "IX_LineageFeatures_LineageId_Id",
                schema: "Game",
                table: "LineageFeatures",
                columns: new[] { "LineageId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LineageFeatures_LineageId_Name",
                schema: "Game",
                table: "LineageFeatures",
                columns: new[] { "LineageId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_LineageFeatures_LineageId_UpdatedBy",
                schema: "Game",
                table: "LineageFeatures",
                columns: new[] { "LineageId", "UpdatedBy" });

            migrationBuilder.CreateIndex(
                name: "IX_LineageFeatures_LineageId_UpdatedOn",
                schema: "Game",
                table: "LineageFeatures",
                columns: new[] { "LineageId", "UpdatedOn" });

            migrationBuilder.CreateIndex(
                name: "IX_LineageLanguages_LanguageId",
                schema: "Game",
                table: "LineageLanguages",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_Lineages_ParentId",
                schema: "Game",
                table: "Lineages",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Lineages_WorldId_CreatedBy",
                schema: "Game",
                table: "Lineages",
                columns: new[] { "WorldId", "CreatedBy" });

            migrationBuilder.CreateIndex(
                name: "IX_Lineages_WorldId_CreatedOn",
                schema: "Game",
                table: "Lineages",
                columns: new[] { "WorldId", "CreatedOn" });

            migrationBuilder.CreateIndex(
                name: "IX_Lineages_WorldId_Id",
                schema: "Game",
                table: "Lineages",
                columns: new[] { "WorldId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lineages_WorldId_Name",
                schema: "Game",
                table: "Lineages",
                columns: new[] { "WorldId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_Lineages_WorldId_ParentId",
                schema: "Game",
                table: "Lineages",
                columns: new[] { "WorldId", "ParentId" });

            migrationBuilder.CreateIndex(
                name: "IX_Lineages_WorldId_SizeCategory",
                schema: "Game",
                table: "Lineages",
                columns: new[] { "WorldId", "SizeCategory" });

            migrationBuilder.CreateIndex(
                name: "IX_Lineages_WorldId_Summary",
                schema: "Game",
                table: "Lineages",
                columns: new[] { "WorldId", "Summary" });

            migrationBuilder.CreateIndex(
                name: "IX_Lineages_WorldId_UpdatedBy",
                schema: "Game",
                table: "Lineages",
                columns: new[] { "WorldId", "UpdatedBy" });

            migrationBuilder.CreateIndex(
                name: "IX_Lineages_WorldId_UpdatedOn",
                schema: "Game",
                table: "Lineages",
                columns: new[] { "WorldId", "UpdatedOn" });

            migrationBuilder.CreateIndex(
                name: "IX_Lineages_WorldId_Version",
                schema: "Game",
                table: "Lineages",
                columns: new[] { "WorldId", "Version" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LineageFeatures",
                schema: "Game");

            migrationBuilder.DropTable(
                name: "LineageLanguages",
                schema: "Game");

            migrationBuilder.DropTable(
                name: "Lineages",
                schema: "Game");
        }
    }
}
