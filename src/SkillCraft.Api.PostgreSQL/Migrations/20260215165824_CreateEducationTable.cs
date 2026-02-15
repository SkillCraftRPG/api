using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SkillCraft.Api.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class CreateEducationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Educations",
                schema: "Game",
                columns: table => new
                {
                    EducationId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WorldId = table.Column<int>(type: "integer", nullable: false),
                    WorldUid = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Summary = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Skill = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    WealthMultiplier = table.Column<int>(type: "integer", nullable: true),
                    FeatureName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    FeatureDescription = table.Column<string>(type: "text", nullable: true),
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
                        principalColumn: "WorldId",
                        onDelete: ReferentialAction.Restrict);
                });

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
                name: "IX_Educations_FeatureName",
                schema: "Game",
                table: "Educations",
                column: "FeatureName");

            migrationBuilder.CreateIndex(
                name: "IX_Educations_Name",
                schema: "Game",
                table: "Educations",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Educations_Skill",
                schema: "Game",
                table: "Educations",
                column: "Skill");

            migrationBuilder.CreateIndex(
                name: "IX_Educations_StreamId",
                schema: "Game",
                table: "Educations",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Educations_Summary",
                schema: "Game",
                table: "Educations",
                column: "Summary");

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
                name: "IX_Educations_WorldId_Id",
                schema: "Game",
                table: "Educations",
                columns: new[] { "WorldId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Educations_WorldUid",
                schema: "Game",
                table: "Educations",
                column: "WorldUid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Educations",
                schema: "Game");
        }
    }
}
