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
                    WorldId = table.Column<int>(type: "integer", nullable: false),
                    WorldUid = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentId = table.Column<int>(type: "integer", nullable: true),
                    ParentUid = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Summary = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Features = table.Column<string>(type: "text", nullable: true),
                    ExtraLanguages = table.Column<int>(type: "integer", nullable: false),
                    LanguagesText = table.Column<string>(type: "text", nullable: true),
                    FamilyNames = table.Column<string>(type: "text", nullable: true),
                    FemaleNames = table.Column<string>(type: "text", nullable: true),
                    MaleNames = table.Column<string>(type: "text", nullable: true),
                    UnisexNames = table.Column<string>(type: "text", nullable: true),
                    CustomNames = table.Column<string>(type: "text", nullable: true),
                    NamesText = table.Column<string>(type: "text", nullable: true),
                    Walk = table.Column<int>(type: "integer", nullable: true),
                    Climb = table.Column<int>(type: "integer", nullable: true),
                    Swim = table.Column<int>(type: "integer", nullable: true),
                    Fly = table.Column<int>(type: "integer", nullable: true),
                    Hover = table.Column<bool>(type: "boolean", nullable: false),
                    Burrow = table.Column<int>(type: "integer", nullable: true),
                    SizeCategory = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Height = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Malnutrition = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Skinny = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Normal = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Overweight = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Obese = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Teenager = table.Column<int>(type: "integer", nullable: true),
                    Adult = table.Column<int>(type: "integer", nullable: true),
                    Mature = table.Column<int>(type: "integer", nullable: true),
                    Venerable = table.Column<int>(type: "integer", nullable: true),
                    StreamId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
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
                        principalColumn: "LineageId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Lineages_Worlds_WorldId",
                        column: x => x.WorldId,
                        principalSchema: "Game",
                        principalTable: "Worlds",
                        principalColumn: "WorldId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LineageLanguages",
                schema: "Game",
                columns: table => new
                {
                    LineageId = table.Column<int>(type: "integer", nullable: false),
                    LanguageId = table.Column<int>(type: "integer", nullable: false),
                    LineageUid = table.Column<Guid>(type: "uuid", nullable: false),
                    LanguageUid = table.Column<Guid>(type: "uuid", nullable: false)
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
                name: "IX_LineageLanguages_LanguageId",
                schema: "Game",
                table: "LineageLanguages",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_LineageLanguages_LanguageUid",
                schema: "Game",
                table: "LineageLanguages",
                column: "LanguageUid");

            migrationBuilder.CreateIndex(
                name: "IX_LineageLanguages_LineageUid",
                schema: "Game",
                table: "LineageLanguages",
                column: "LineageUid");

            migrationBuilder.CreateIndex(
                name: "IX_Lineages_CreatedBy",
                schema: "Game",
                table: "Lineages",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Lineages_CreatedOn",
                schema: "Game",
                table: "Lineages",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Lineages_Name",
                schema: "Game",
                table: "Lineages",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Lineages_ParentId",
                schema: "Game",
                table: "Lineages",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Lineages_ParentUid",
                schema: "Game",
                table: "Lineages",
                column: "ParentUid");

            migrationBuilder.CreateIndex(
                name: "IX_Lineages_SizeCategory",
                schema: "Game",
                table: "Lineages",
                column: "SizeCategory");

            migrationBuilder.CreateIndex(
                name: "IX_Lineages_StreamId",
                schema: "Game",
                table: "Lineages",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lineages_Summary",
                schema: "Game",
                table: "Lineages",
                column: "Summary");

            migrationBuilder.CreateIndex(
                name: "IX_Lineages_UpdatedBy",
                schema: "Game",
                table: "Lineages",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Lineages_UpdatedOn",
                schema: "Game",
                table: "Lineages",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Lineages_Version",
                schema: "Game",
                table: "Lineages",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_Lineages_WorldId_Id",
                schema: "Game",
                table: "Lineages",
                columns: new[] { "WorldId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lineages_WorldUid",
                schema: "Game",
                table: "Lineages",
                column: "WorldUid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LineageLanguages",
                schema: "Game");

            migrationBuilder.DropTable(
                name: "Lineages",
                schema: "Game");
        }
    }
}
