using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SkillCraft.Api.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Game");

            migrationBuilder.CreateTable(
                name: "Worlds",
                schema: "Game",
                columns: table => new
                {
                    WorldId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerId = table.Column<string>(type: "text", nullable: false),
                    Key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Content = table.Column<string>(type: "text", nullable: true),
                    StreamId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Worlds", x => x.WorldId);
                    table.UniqueConstraint("AK_Worlds_Id", x => x.Id);
                });

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
                    Content = table.Column<string>(type: "text", nullable: true),
                    Skill = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    WealthRoll = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    FeatureName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    FeatureContent = table.Column<string>(type: "text", nullable: true),
                    StreamId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
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

            migrationBuilder.CreateTable(
                name: "Customizations",
                schema: "Game",
                columns: table => new
                {
                    CustomizationId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WorldId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Kind = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Summary = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Content = table.Column<string>(type: "text", nullable: true),
                    StreamId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customizations", x => x.CustomizationId);
                    table.ForeignKey(
                        name: "FK_Customizations_Worlds_WorldId",
                        column: x => x.WorldId,
                        principalSchema: "Game",
                        principalTable: "Worlds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Educations",
                schema: "Game",
                columns: table => new
                {
                    EducationId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WorldId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Summary = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Content = table.Column<string>(type: "text", nullable: true),
                    Skill = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    WealthMultiplier = table.Column<int>(type: "integer", nullable: true),
                    FeatureName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    FeatureContent = table.Column<string>(type: "text", nullable: true),
                    StreamId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Educations", x => x.EducationId);
                    table.ForeignKey(
                        name: "FK_Educations_Worlds_WorldId",
                        column: x => x.WorldId,
                        principalSchema: "Game",
                        principalTable: "Worlds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                schema: "Game",
                columns: table => new
                {
                    ItemId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WorldId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Category = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Summary = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Content = table.Column<string>(type: "text", nullable: true),
                    Price = table.Column<int>(type: "integer", nullable: true),
                    Weight = table.Column<int>(type: "integer", nullable: true),
                    Rarity = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    MaximumCharges = table.Column<int>(type: "integer", nullable: true),
                    ChargesDepletionBehavior = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: true),
                    ReplacementId = table.Column<int>(type: "integer", nullable: true),
                    IsMagic = table.Column<bool>(type: "boolean", nullable: false),
                    IsAttunementRequired = table.Column<bool>(type: "boolean", nullable: true),
                    AttunementRequirements = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Properties = table.Column<string>(type: "text", nullable: true),
                    StreamId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.ItemId);
                    table.ForeignKey(
                        name: "FK_Items_Items_ReplacementId",
                        column: x => x.ReplacementId,
                        principalSchema: "Game",
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Items_Worlds_WorldId",
                        column: x => x.WorldId,
                        principalSchema: "Game",
                        principalTable: "Worlds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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
                    Features = table.Column<string>(type: "text", nullable: true),
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
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Scripts",
                schema: "Game",
                columns: table => new
                {
                    ScriptId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WorldId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Summary = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Content = table.Column<string>(type: "text", nullable: true),
                    StreamId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scripts", x => x.ScriptId);
                    table.ForeignKey(
                        name: "FK_Scripts_Worlds_WorldId",
                        column: x => x.WorldId,
                        principalSchema: "Game",
                        principalTable: "Worlds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Spells",
                schema: "Game",
                columns: table => new
                {
                    SpellId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WorldId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Tier = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Summary = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Content = table.Column<string>(type: "text", nullable: true),
                    StreamId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spells", x => x.SpellId);
                    table.ForeignKey(
                        name: "FK_Spells_Worlds_WorldId",
                        column: x => x.WorldId,
                        principalSchema: "Game",
                        principalTable: "Worlds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Talents",
                schema: "Game",
                columns: table => new
                {
                    TalentId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WorldId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Tier = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Summary = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Content = table.Column<string>(type: "text", nullable: true),
                    AllowMultiplePurchases = table.Column<bool>(type: "boolean", nullable: false),
                    Skill = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    RequiredTalentId = table.Column<int>(type: "integer", nullable: true),
                    StreamId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Talents", x => x.TalentId);
                    table.ForeignKey(
                        name: "FK_Talents_Talents_RequiredTalentId",
                        column: x => x.RequiredTalentId,
                        principalSchema: "Game",
                        principalTable: "Talents",
                        principalColumn: "TalentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Talents_Worlds_WorldId",
                        column: x => x.WorldId,
                        principalSchema: "Game",
                        principalTable: "Worlds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Characters",
                schema: "Game",
                columns: table => new
                {
                    CharacterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WorldId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Tier = table.Column<int>(type: "integer", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    Experience = table.Column<int>(type: "integer", nullable: false),
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
                    CurrentVitality = table.Column<int>(type: "integer", nullable: false),
                    TemporaryVitality = table.Column<int>(type: "integer", nullable: false),
                    StunDamage = table.Column<int>(type: "integer", nullable: false),
                    Stamina = table.Column<int>(type: "integer", nullable: false),
                    CurrentHope = table.Column<int>(type: "integer", nullable: false),
                    MaximumHope = table.Column<int>(type: "integer", nullable: false),
                    BloodAlcoholContent = table.Column<int>(type: "integer", nullable: false),
                    Intoxication = table.Column<int>(type: "integer", nullable: false),
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
                name: "Languages",
                schema: "Game",
                columns: table => new
                {
                    LanguageId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WorldId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Summary = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Content = table.Column<string>(type: "text", nullable: true),
                    TypicalSpeakers = table.Column<string>(type: "text", nullable: true),
                    ScriptId = table.Column<int>(type: "integer", nullable: true),
                    StreamId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.LanguageId);
                    table.ForeignKey(
                        name: "FK_Languages_Scripts_ScriptId",
                        column: x => x.ScriptId,
                        principalSchema: "Game",
                        principalTable: "Scripts",
                        principalColumn: "ScriptId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Languages_Worlds_WorldId",
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
                name: "CharacterModifiers",
                schema: "Game",
                columns: table => new
                {
                    CharacterModifierId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CharacterId = table.Column<int>(type: "integer", nullable: false),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Kind = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Target = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Value = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterModifiers", x => x.CharacterModifierId);
                    table.ForeignKey(
                        name: "FK_CharacterModifiers_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalSchema: "Game",
                        principalTable: "Characters",
                        principalColumn: "CharacterId",
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
                name: "IX_Castes_CreatedBy",
                schema: "Game",
                table: "Castes",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Castes_CreatedOn",
                schema: "Game",
                table: "Castes",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Castes_StreamId",
                schema: "Game",
                table: "Castes",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Castes_UpdatedBy",
                schema: "Game",
                table: "Castes",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Castes_UpdatedOn",
                schema: "Game",
                table: "Castes",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Castes_Version",
                schema: "Game",
                table: "Castes",
                column: "Version");

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
                name: "IX_CharacterModifiers_CharacterId_Id",
                schema: "Game",
                table: "CharacterModifiers",
                columns: new[] { "CharacterId", "Id" },
                unique: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_Customizations_CreatedBy",
                schema: "Game",
                table: "Customizations",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Customizations_CreatedOn",
                schema: "Game",
                table: "Customizations",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Customizations_StreamId",
                schema: "Game",
                table: "Customizations",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customizations_UpdatedBy",
                schema: "Game",
                table: "Customizations",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Customizations_UpdatedOn",
                schema: "Game",
                table: "Customizations",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Customizations_Version",
                schema: "Game",
                table: "Customizations",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_Customizations_WorldId_Id",
                schema: "Game",
                table: "Customizations",
                columns: new[] { "WorldId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customizations_WorldId_Kind",
                schema: "Game",
                table: "Customizations",
                columns: new[] { "WorldId", "Kind" });

            migrationBuilder.CreateIndex(
                name: "IX_Customizations_WorldId_Name",
                schema: "Game",
                table: "Customizations",
                columns: new[] { "WorldId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_Customizations_WorldId_Summary",
                schema: "Game",
                table: "Customizations",
                columns: new[] { "WorldId", "Summary" });

            migrationBuilder.CreateIndex(
                name: "IX_Educations_CreatedBy",
                schema: "Game",
                table: "Educations",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Educations_CreatedOn",
                schema: "Game",
                table: "Educations",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Educations_StreamId",
                schema: "Game",
                table: "Educations",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Educations_UpdatedBy",
                schema: "Game",
                table: "Educations",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Educations_UpdatedOn",
                schema: "Game",
                table: "Educations",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Educations_Version",
                schema: "Game",
                table: "Educations",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_Educations_WorldId_FeatureName",
                schema: "Game",
                table: "Educations",
                columns: new[] { "WorldId", "FeatureName" });

            migrationBuilder.CreateIndex(
                name: "IX_Educations_WorldId_Id",
                schema: "Game",
                table: "Educations",
                columns: new[] { "WorldId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Educations_WorldId_Name",
                schema: "Game",
                table: "Educations",
                columns: new[] { "WorldId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_Educations_WorldId_Skill",
                schema: "Game",
                table: "Educations",
                columns: new[] { "WorldId", "Skill" });

            migrationBuilder.CreateIndex(
                name: "IX_Educations_WorldId_Summary",
                schema: "Game",
                table: "Educations",
                columns: new[] { "WorldId", "Summary" });

            migrationBuilder.CreateIndex(
                name: "IX_Items_CreatedBy",
                schema: "Game",
                table: "Items",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Items_CreatedOn",
                schema: "Game",
                table: "Items",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Items_ReplacementId",
                schema: "Game",
                table: "Items",
                column: "ReplacementId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_StreamId",
                schema: "Game",
                table: "Items",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_UpdatedBy",
                schema: "Game",
                table: "Items",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Items_UpdatedOn",
                schema: "Game",
                table: "Items",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Items_Version",
                schema: "Game",
                table: "Items",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_Items_WorldId_Category",
                schema: "Game",
                table: "Items",
                columns: new[] { "WorldId", "Category" });

            migrationBuilder.CreateIndex(
                name: "IX_Items_WorldId_Id",
                schema: "Game",
                table: "Items",
                columns: new[] { "WorldId", "Id" },
                unique: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_Items_WorldId_Name",
                schema: "Game",
                table: "Items",
                columns: new[] { "WorldId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_Items_WorldId_Price",
                schema: "Game",
                table: "Items",
                columns: new[] { "WorldId", "Price" });

            migrationBuilder.CreateIndex(
                name: "IX_Items_WorldId_Rarity",
                schema: "Game",
                table: "Items",
                columns: new[] { "WorldId", "Rarity" });

            migrationBuilder.CreateIndex(
                name: "IX_Items_WorldId_ReplacementId",
                schema: "Game",
                table: "Items",
                columns: new[] { "WorldId", "ReplacementId" });

            migrationBuilder.CreateIndex(
                name: "IX_Items_WorldId_Summary",
                schema: "Game",
                table: "Items",
                columns: new[] { "WorldId", "Summary" });

            migrationBuilder.CreateIndex(
                name: "IX_Items_WorldId_Weight",
                schema: "Game",
                table: "Items",
                columns: new[] { "WorldId", "Weight" });

            migrationBuilder.CreateIndex(
                name: "IX_Languages_CreatedBy",
                schema: "Game",
                table: "Languages",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_CreatedOn",
                schema: "Game",
                table: "Languages",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_ScriptId",
                schema: "Game",
                table: "Languages",
                column: "ScriptId");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_StreamId",
                schema: "Game",
                table: "Languages",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Languages_UpdatedBy",
                schema: "Game",
                table: "Languages",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_UpdatedOn",
                schema: "Game",
                table: "Languages",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_Version",
                schema: "Game",
                table: "Languages",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_WorldId_Id",
                schema: "Game",
                table: "Languages",
                columns: new[] { "WorldId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Languages_WorldId_Name",
                schema: "Game",
                table: "Languages",
                columns: new[] { "WorldId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_Languages_WorldId_ScriptId",
                schema: "Game",
                table: "Languages",
                columns: new[] { "WorldId", "ScriptId" });

            migrationBuilder.CreateIndex(
                name: "IX_Languages_WorldId_Summary",
                schema: "Game",
                table: "Languages",
                columns: new[] { "WorldId", "Summary" });

            migrationBuilder.CreateIndex(
                name: "IX_LineageLanguages_LanguageId",
                schema: "Game",
                table: "LineageLanguages",
                column: "LanguageId");

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
                name: "IX_Lineages_ParentId",
                schema: "Game",
                table: "Lineages",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Lineages_StreamId",
                schema: "Game",
                table: "Lineages",
                column: "StreamId",
                unique: true);

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
                name: "IX_Scripts_CreatedBy",
                schema: "Game",
                table: "Scripts",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Scripts_CreatedOn",
                schema: "Game",
                table: "Scripts",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Scripts_StreamId",
                schema: "Game",
                table: "Scripts",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Scripts_UpdatedBy",
                schema: "Game",
                table: "Scripts",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Scripts_UpdatedOn",
                schema: "Game",
                table: "Scripts",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Scripts_Version",
                schema: "Game",
                table: "Scripts",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_Scripts_WorldId_Id",
                schema: "Game",
                table: "Scripts",
                columns: new[] { "WorldId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Scripts_WorldId_Name",
                schema: "Game",
                table: "Scripts",
                columns: new[] { "WorldId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_Scripts_WorldId_Summary",
                schema: "Game",
                table: "Scripts",
                columns: new[] { "WorldId", "Summary" });

            migrationBuilder.CreateIndex(
                name: "IX_Spells_CreatedBy",
                schema: "Game",
                table: "Spells",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Spells_CreatedOn",
                schema: "Game",
                table: "Spells",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Spells_StreamId",
                schema: "Game",
                table: "Spells",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Spells_UpdatedBy",
                schema: "Game",
                table: "Spells",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Spells_UpdatedOn",
                schema: "Game",
                table: "Spells",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Spells_Version",
                schema: "Game",
                table: "Spells",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_Spells_WorldId_Id",
                schema: "Game",
                table: "Spells",
                columns: new[] { "WorldId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Spells_WorldId_Name",
                schema: "Game",
                table: "Spells",
                columns: new[] { "WorldId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_Spells_WorldId_Summary",
                schema: "Game",
                table: "Spells",
                columns: new[] { "WorldId", "Summary" });

            migrationBuilder.CreateIndex(
                name: "IX_Spells_WorldId_Tier",
                schema: "Game",
                table: "Spells",
                columns: new[] { "WorldId", "Tier" });

            migrationBuilder.CreateIndex(
                name: "IX_Talents_CreatedBy",
                schema: "Game",
                table: "Talents",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Talents_CreatedOn",
                schema: "Game",
                table: "Talents",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Talents_RequiredTalentId",
                schema: "Game",
                table: "Talents",
                column: "RequiredTalentId");

            migrationBuilder.CreateIndex(
                name: "IX_Talents_StreamId",
                schema: "Game",
                table: "Talents",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Talents_UpdatedBy",
                schema: "Game",
                table: "Talents",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Talents_UpdatedOn",
                schema: "Game",
                table: "Talents",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Talents_Version",
                schema: "Game",
                table: "Talents",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_Talents_WorldId_AllowMultiplePurchases",
                schema: "Game",
                table: "Talents",
                columns: new[] { "WorldId", "AllowMultiplePurchases" });

            migrationBuilder.CreateIndex(
                name: "IX_Talents_WorldId_Id",
                schema: "Game",
                table: "Talents",
                columns: new[] { "WorldId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Talents_WorldId_Name",
                schema: "Game",
                table: "Talents",
                columns: new[] { "WorldId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_Talents_WorldId_RequiredTalentId",
                schema: "Game",
                table: "Talents",
                columns: new[] { "WorldId", "RequiredTalentId" });

            migrationBuilder.CreateIndex(
                name: "IX_Talents_WorldId_Skill",
                schema: "Game",
                table: "Talents",
                columns: new[] { "WorldId", "Skill" });

            migrationBuilder.CreateIndex(
                name: "IX_Talents_WorldId_Summary",
                schema: "Game",
                table: "Talents",
                columns: new[] { "WorldId", "Summary" });

            migrationBuilder.CreateIndex(
                name: "IX_Talents_WorldId_Tier",
                schema: "Game",
                table: "Talents",
                columns: new[] { "WorldId", "Tier" });

            migrationBuilder.CreateIndex(
                name: "IX_Worlds_CreatedBy",
                schema: "Game",
                table: "Worlds",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Worlds_CreatedOn",
                schema: "Game",
                table: "Worlds",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Worlds_Id",
                schema: "Game",
                table: "Worlds",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Worlds_Key",
                schema: "Game",
                table: "Worlds",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Worlds_Name",
                schema: "Game",
                table: "Worlds",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Worlds_OwnerId",
                schema: "Game",
                table: "Worlds",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Worlds_StreamId",
                schema: "Game",
                table: "Worlds",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Worlds_UpdatedBy",
                schema: "Game",
                table: "Worlds",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Worlds_UpdatedOn",
                schema: "Game",
                table: "Worlds",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Worlds_Version",
                schema: "Game",
                table: "Worlds",
                column: "Version");
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
                name: "CharacterModifiers",
                schema: "Game");

            migrationBuilder.DropTable(
                name: "CharacterTalents",
                schema: "Game");

            migrationBuilder.DropTable(
                name: "Items",
                schema: "Game");

            migrationBuilder.DropTable(
                name: "LineageLanguages",
                schema: "Game");

            migrationBuilder.DropTable(
                name: "Spells",
                schema: "Game");

            migrationBuilder.DropTable(
                name: "Customizations",
                schema: "Game");

            migrationBuilder.DropTable(
                name: "Characters",
                schema: "Game");

            migrationBuilder.DropTable(
                name: "Talents",
                schema: "Game");

            migrationBuilder.DropTable(
                name: "Languages",
                schema: "Game");

            migrationBuilder.DropTable(
                name: "Castes",
                schema: "Game");

            migrationBuilder.DropTable(
                name: "Educations",
                schema: "Game");

            migrationBuilder.DropTable(
                name: "Lineages",
                schema: "Game");

            migrationBuilder.DropTable(
                name: "Scripts",
                schema: "Game");

            migrationBuilder.DropTable(
                name: "Worlds",
                schema: "Game");
        }
    }
}
