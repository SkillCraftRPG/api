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
                    WorldId = table.Column<int>(type: "integer", nullable: false),
                    WorldUid = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Summary = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Skill = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    WealthRoll = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
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
                    table.PrimaryKey("PK_Castes", x => x.CasteId);
                    table.ForeignKey(
                        name: "FK_Castes_Worlds_WorldId",
                        column: x => x.WorldId,
                        principalSchema: "Game",
                        principalTable: "Worlds",
                        principalColumn: "WorldId",
                        onDelete: ReferentialAction.Restrict);
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
                name: "IX_Castes_FeatureName",
                schema: "Game",
                table: "Castes",
                column: "FeatureName");

            migrationBuilder.CreateIndex(
                name: "IX_Castes_Name",
                schema: "Game",
                table: "Castes",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Castes_Skill",
                schema: "Game",
                table: "Castes",
                column: "Skill");

            migrationBuilder.CreateIndex(
                name: "IX_Castes_StreamId",
                schema: "Game",
                table: "Castes",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Castes_Summary",
                schema: "Game",
                table: "Castes",
                column: "Summary");

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
                name: "IX_Castes_WorldId_Id",
                schema: "Game",
                table: "Castes",
                columns: new[] { "WorldId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Castes_WorldUid",
                schema: "Game",
                table: "Castes",
                column: "WorldUid");
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
