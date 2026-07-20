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
                    WorldId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Tier = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Summary = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    HtmlContent = table.Column<string>(type: "text", nullable: true),
                    AllowMultiplePurchases = table.Column<bool>(type: "boolean", nullable: false),
                    Skill = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    RequiredTalentId = table.Column<int>(type: "integer", nullable: true),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: false),
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

            migrationBuilder.CreateIndex(
                name: "IX_Talents_RequiredTalentId",
                schema: "Game",
                table: "Talents",
                column: "RequiredTalentId");

            migrationBuilder.CreateIndex(
                name: "IX_Talents_WorldId_AllowMultiplePurchases",
                schema: "Game",
                table: "Talents",
                columns: new[] { "WorldId", "AllowMultiplePurchases" });

            migrationBuilder.CreateIndex(
                name: "IX_Talents_WorldId_CreatedBy",
                schema: "Game",
                table: "Talents",
                columns: new[] { "WorldId", "CreatedBy" });

            migrationBuilder.CreateIndex(
                name: "IX_Talents_WorldId_CreatedOn",
                schema: "Game",
                table: "Talents",
                columns: new[] { "WorldId", "CreatedOn" });

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
                name: "IX_Talents_WorldId_UpdatedBy",
                schema: "Game",
                table: "Talents",
                columns: new[] { "WorldId", "UpdatedBy" });

            migrationBuilder.CreateIndex(
                name: "IX_Talents_WorldId_UpdatedOn",
                schema: "Game",
                table: "Talents",
                columns: new[] { "WorldId", "UpdatedOn" });

            migrationBuilder.CreateIndex(
                name: "IX_Talents_WorldId_Version",
                schema: "Game",
                table: "Talents",
                columns: new[] { "WorldId", "Version" });
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
