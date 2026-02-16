using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SkillCraft.Api.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class CreateLanguageTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Languages",
                schema: "Game",
                columns: table => new
                {
                    LanguageId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WorldId = table.Column<int>(type: "integer", nullable: false),
                    WorldUid = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Summary = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ScriptId = table.Column<int>(type: "integer", nullable: true),
                    ScriptUid = table.Column<Guid>(type: "uuid", nullable: true),
                    TypicalSpeakers = table.Column<string>(type: "text", nullable: true),
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
                        principalColumn: "WorldId",
                        onDelete: ReferentialAction.Restrict);
                });

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
                name: "IX_Languages_Name",
                schema: "Game",
                table: "Languages",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_ScriptId",
                schema: "Game",
                table: "Languages",
                column: "ScriptId");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_ScriptUid",
                schema: "Game",
                table: "Languages",
                column: "ScriptUid");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_StreamId",
                schema: "Game",
                table: "Languages",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Languages_Summary",
                schema: "Game",
                table: "Languages",
                column: "Summary");

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
                name: "IX_Languages_WorldUid",
                schema: "Game",
                table: "Languages",
                column: "WorldUid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Languages",
                schema: "Game");
        }
    }
}
