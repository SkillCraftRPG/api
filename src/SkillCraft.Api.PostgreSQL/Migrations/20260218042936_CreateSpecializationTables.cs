using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SkillCraft.Api.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class CreateSpecializationTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Specializations",
                schema: "Game",
                columns: table => new
                {
                    SpecializationId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WorldId = table.Column<int>(type: "integer", nullable: false),
                    WorldUid = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Tier = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Summary = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    RequiredTalentId = table.Column<int>(type: "integer", nullable: true),
                    RequiredTalentUid = table.Column<Guid>(type: "uuid", nullable: true),
                    OtherRequirements = table.Column<string>(type: "text", nullable: true),
                    OtherOptions = table.Column<string>(type: "text", nullable: true),
                    DoctrineName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DoctrineDescription = table.Column<string>(type: "text", nullable: true),
                    DoctrineFeatures = table.Column<string>(type: "text", nullable: true),
                    StreamId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Specializations", x => x.SpecializationId);
                    table.ForeignKey(
                        name: "FK_Specializations_Talents_RequiredTalentId",
                        column: x => x.RequiredTalentId,
                        principalSchema: "Game",
                        principalTable: "Talents",
                        principalColumn: "TalentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Specializations_Worlds_WorldId",
                        column: x => x.WorldId,
                        principalSchema: "Game",
                        principalTable: "Worlds",
                        principalColumn: "WorldId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SpecializationDiscountedTalents",
                schema: "Game",
                columns: table => new
                {
                    SpecializationId = table.Column<int>(type: "integer", nullable: false),
                    TalentId = table.Column<int>(type: "integer", nullable: false),
                    SpecializationUid = table.Column<Guid>(type: "uuid", nullable: false),
                    TalentUid = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecializationDiscountedTalents", x => new { x.SpecializationId, x.TalentId });
                    table.ForeignKey(
                        name: "FK_SpecializationDiscountedTalents_Specializations_Specializat~",
                        column: x => x.SpecializationId,
                        principalSchema: "Game",
                        principalTable: "Specializations",
                        principalColumn: "SpecializationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SpecializationDiscountedTalents_Talents_TalentId",
                        column: x => x.TalentId,
                        principalSchema: "Game",
                        principalTable: "Talents",
                        principalColumn: "TalentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SpecializationOptionalTalents",
                schema: "Game",
                columns: table => new
                {
                    SpecializationId = table.Column<int>(type: "integer", nullable: false),
                    TalentId = table.Column<int>(type: "integer", nullable: false),
                    SpecializationUid = table.Column<Guid>(type: "uuid", nullable: false),
                    TalentUid = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecializationOptionalTalents", x => new { x.SpecializationId, x.TalentId });
                    table.ForeignKey(
                        name: "FK_SpecializationOptionalTalents_Specializations_Specializatio~",
                        column: x => x.SpecializationId,
                        principalSchema: "Game",
                        principalTable: "Specializations",
                        principalColumn: "SpecializationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SpecializationOptionalTalents_Talents_TalentId",
                        column: x => x.TalentId,
                        principalSchema: "Game",
                        principalTable: "Talents",
                        principalColumn: "TalentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SpecializationDiscountedTalents_SpecializationUid",
                schema: "Game",
                table: "SpecializationDiscountedTalents",
                column: "SpecializationUid");

            migrationBuilder.CreateIndex(
                name: "IX_SpecializationDiscountedTalents_TalentId",
                schema: "Game",
                table: "SpecializationDiscountedTalents",
                column: "TalentId");

            migrationBuilder.CreateIndex(
                name: "IX_SpecializationDiscountedTalents_TalentUid",
                schema: "Game",
                table: "SpecializationDiscountedTalents",
                column: "TalentUid");

            migrationBuilder.CreateIndex(
                name: "IX_SpecializationOptionalTalents_SpecializationUid",
                schema: "Game",
                table: "SpecializationOptionalTalents",
                column: "SpecializationUid");

            migrationBuilder.CreateIndex(
                name: "IX_SpecializationOptionalTalents_TalentId",
                schema: "Game",
                table: "SpecializationOptionalTalents",
                column: "TalentId");

            migrationBuilder.CreateIndex(
                name: "IX_SpecializationOptionalTalents_TalentUid",
                schema: "Game",
                table: "SpecializationOptionalTalents",
                column: "TalentUid");

            migrationBuilder.CreateIndex(
                name: "IX_Specializations_CreatedBy",
                schema: "Game",
                table: "Specializations",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Specializations_CreatedOn",
                schema: "Game",
                table: "Specializations",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Specializations_DoctrineName",
                schema: "Game",
                table: "Specializations",
                column: "DoctrineName");

            migrationBuilder.CreateIndex(
                name: "IX_Specializations_Name",
                schema: "Game",
                table: "Specializations",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Specializations_RequiredTalentId",
                schema: "Game",
                table: "Specializations",
                column: "RequiredTalentId");

            migrationBuilder.CreateIndex(
                name: "IX_Specializations_RequiredTalentUid",
                schema: "Game",
                table: "Specializations",
                column: "RequiredTalentUid");

            migrationBuilder.CreateIndex(
                name: "IX_Specializations_StreamId",
                schema: "Game",
                table: "Specializations",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Specializations_Summary",
                schema: "Game",
                table: "Specializations",
                column: "Summary");

            migrationBuilder.CreateIndex(
                name: "IX_Specializations_Tier",
                schema: "Game",
                table: "Specializations",
                column: "Tier");

            migrationBuilder.CreateIndex(
                name: "IX_Specializations_UpdatedBy",
                schema: "Game",
                table: "Specializations",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Specializations_UpdatedOn",
                schema: "Game",
                table: "Specializations",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Specializations_Version",
                schema: "Game",
                table: "Specializations",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_Specializations_WorldId_Id",
                schema: "Game",
                table: "Specializations",
                columns: new[] { "WorldId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Specializations_WorldUid",
                schema: "Game",
                table: "Specializations",
                column: "WorldUid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SpecializationDiscountedTalents",
                schema: "Game");

            migrationBuilder.DropTable(
                name: "SpecializationOptionalTalents",
                schema: "Game");

            migrationBuilder.DropTable(
                name: "Specializations",
                schema: "Game");
        }
    }
}
