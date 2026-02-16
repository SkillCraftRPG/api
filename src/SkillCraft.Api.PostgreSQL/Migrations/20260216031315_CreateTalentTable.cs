using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SkillCraft.Api.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class CreateTalentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Talents",
                schema: "Game",
                columns: table => new
                {
                    TalentId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WorldId = table.Column<int>(type: "integer", nullable: false),
                    WorldUid = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Tier = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Summary = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    AllowMultiplePurchases = table.Column<bool>(type: "boolean", nullable: false),
                    Skill = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    RequiredTalentId = table.Column<int>(type: "integer", nullable: true),
                    RequiredTalentUid = table.Column<Guid>(type: "uuid", nullable: true),
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
                        principalColumn: "WorldId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Talents_AllowMultiplePurchases",
                schema: "Game",
                table: "Talents",
                column: "AllowMultiplePurchases");

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
                name: "IX_Talents_Name",
                schema: "Game",
                table: "Talents",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Talents_RequiredTalentId",
                schema: "Game",
                table: "Talents",
                column: "RequiredTalentId");

            migrationBuilder.CreateIndex(
                name: "IX_Talents_RequiredTalentUid",
                schema: "Game",
                table: "Talents",
                column: "RequiredTalentUid");

            migrationBuilder.CreateIndex(
                name: "IX_Talents_Skill",
                schema: "Game",
                table: "Talents",
                column: "Skill");

            migrationBuilder.CreateIndex(
                name: "IX_Talents_StreamId",
                schema: "Game",
                table: "Talents",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Talents_Summary",
                schema: "Game",
                table: "Talents",
                column: "Summary");

            migrationBuilder.CreateIndex(
                name: "IX_Talents_Tier",
                schema: "Game",
                table: "Talents",
                column: "Tier");

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
                name: "IX_Talents_WorldId_Id",
                schema: "Game",
                table: "Talents",
                columns: new[] { "WorldId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Talents_WorldUid",
                schema: "Game",
                table: "Talents",
                column: "WorldUid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Talents",
                schema: "Game");
        }
    }
}
