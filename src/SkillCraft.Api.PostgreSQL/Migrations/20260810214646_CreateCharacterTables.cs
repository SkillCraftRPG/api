using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SkillCraft.Api.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class CreateCharacterTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Characters",
                schema: "Game",
                columns: table => new
                {
                    CharacterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WorldId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DominantHand = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: true),
                    LineageId = table.Column<int>(type: "integer", nullable: false),
                    CasteId = table.Column<int>(type: "integer", nullable: false),
                    EducationId = table.Column<int>(type: "integer", nullable: false),
                    Height = table.Column<int>(type: "integer", nullable: true),
                    Weight = table.Column<int>(type: "integer", nullable: true),
                    Age = table.Column<int>(type: "integer", nullable: true),
                    Skin = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Eyes = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Hair = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Alignment = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    Traits = table.Column<string>(type: "text", nullable: true),
                    Ideals = table.Column<string>(type: "text", nullable: true),
                    Flaws = table.Column<string>(type: "text", nullable: true),
                    Background = table.Column<string>(type: "text", nullable: true),
                    Attributes = table.Column<string>(type: "text", nullable: true),
                    Skills = table.Column<string>(type: "text", nullable: true),
                    StreamId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characters", x => x.CharacterId);
                    table.ForeignKey(
                        name: "FK_Characters_Castes_CasteId",
                        column: x => x.CasteId,
                        principalSchema: "Game",
                        principalTable: "Castes",
                        principalColumn: "CasteId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Characters_Educations_EducationId",
                        column: x => x.EducationId,
                        principalSchema: "Game",
                        principalTable: "Educations",
                        principalColumn: "EducationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Characters_Lineages_LineageId",
                        column: x => x.LineageId,
                        principalSchema: "Game",
                        principalTable: "Lineages",
                        principalColumn: "LineageId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Characters_Worlds_WorldId",
                        column: x => x.WorldId,
                        principalSchema: "Game",
                        principalTable: "Worlds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CharacterCustomizations",
                schema: "Game",
                columns: table => new
                {
                    CharacterId = table.Column<int>(type: "integer", nullable: false),
                    CustomizationId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterCustomizations", x => new { x.CharacterId, x.CustomizationId });
                    table.ForeignKey(
                        name: "FK_CharacterCustomizations_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalSchema: "Game",
                        principalTable: "Characters",
                        principalColumn: "CharacterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterCustomizations_Customizations_CustomizationId",
                        column: x => x.CustomizationId,
                        principalSchema: "Game",
                        principalTable: "Customizations",
                        principalColumn: "CustomizationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharacterLanguages",
                schema: "Game",
                columns: table => new
                {
                    CharacterId = table.Column<int>(type: "integer", nullable: false),
                    LanguageId = table.Column<int>(type: "integer", nullable: false),
                    Source = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Target = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterLanguages", x => new { x.CharacterId, x.LanguageId });
                    table.ForeignKey(
                        name: "FK_CharacterLanguages_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalSchema: "Game",
                        principalTable: "Characters",
                        principalColumn: "CharacterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterLanguages_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalSchema: "Game",
                        principalTable: "Languages",
                        principalColumn: "LanguageId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharacterTalents",
                schema: "Game",
                columns: table => new
                {
                    CharacterTalentId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CharacterId = table.Column<int>(type: "integer", nullable: false),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TalentId = table.Column<int>(type: "integer", nullable: false),
                    Qualifier = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    Discounts = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterTalents", x => x.CharacterTalentId);
                    table.ForeignKey(
                        name: "FK_CharacterTalents_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalSchema: "Game",
                        principalTable: "Characters",
                        principalColumn: "CharacterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterTalents_Talents_TalentId",
                        column: x => x.TalentId,
                        principalSchema: "Game",
                        principalTable: "Talents",
                        principalColumn: "TalentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CharacterCustomizations_CustomizationId",
                schema: "Game",
                table: "CharacterCustomizations",
                column: "CustomizationId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterLanguages_LanguageId",
                schema: "Game",
                table: "CharacterLanguages",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_CasteId",
                schema: "Game",
                table: "Characters",
                column: "CasteId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_CreatedBy",
                schema: "Game",
                table: "Characters",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_CreatedOn",
                schema: "Game",
                table: "Characters",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_EducationId",
                schema: "Game",
                table: "Characters",
                column: "EducationId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_LineageId",
                schema: "Game",
                table: "Characters",
                column: "LineageId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_StreamId",
                schema: "Game",
                table: "Characters",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Characters_UpdatedBy",
                schema: "Game",
                table: "Characters",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_UpdatedOn",
                schema: "Game",
                table: "Characters",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_Version",
                schema: "Game",
                table: "Characters",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_WorldId_CasteId",
                schema: "Game",
                table: "Characters",
                columns: new[] { "WorldId", "CasteId" });

            migrationBuilder.CreateIndex(
                name: "IX_Characters_WorldId_EducationId",
                schema: "Game",
                table: "Characters",
                columns: new[] { "WorldId", "EducationId" });

            migrationBuilder.CreateIndex(
                name: "IX_Characters_WorldId_Id",
                schema: "Game",
                table: "Characters",
                columns: new[] { "WorldId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Characters_WorldId_LineageId",
                schema: "Game",
                table: "Characters",
                columns: new[] { "WorldId", "LineageId" });

            migrationBuilder.CreateIndex(
                name: "IX_Characters_WorldId_Name",
                schema: "Game",
                table: "Characters",
                columns: new[] { "WorldId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_CharacterTalents_CharacterId_Id",
                schema: "Game",
                table: "CharacterTalents",
                columns: new[] { "CharacterId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharacterTalents_TalentId",
                schema: "Game",
                table: "CharacterTalents",
                column: "TalentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CharacterCustomizations",
                schema: "Game");

            migrationBuilder.DropTable(
                name: "CharacterLanguages",
                schema: "Game");

            migrationBuilder.DropTable(
                name: "CharacterTalents",
                schema: "Game");

            migrationBuilder.DropTable(
                name: "Characters",
                schema: "Game");
        }
    }
}
