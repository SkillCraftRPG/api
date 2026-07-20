using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SkillCraft.Api.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class CreateCustomizationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_Worlds_Id",
                schema: "Game",
                table: "Worlds",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Customizations",
                schema: "Game",
                columns: table => new
                {
                    CustomizationId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WorldId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Kind = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: false),
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

            migrationBuilder.CreateIndex(
                name: "IX_Customizations_WorldId_CreatedBy",
                schema: "Game",
                table: "Customizations",
                columns: new[] { "WorldId", "CreatedBy" });

            migrationBuilder.CreateIndex(
                name: "IX_Customizations_WorldId_CreatedOn",
                schema: "Game",
                table: "Customizations",
                columns: new[] { "WorldId", "CreatedOn" });

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
                name: "IX_Customizations_WorldId_UpdatedBy",
                schema: "Game",
                table: "Customizations",
                columns: new[] { "WorldId", "UpdatedBy" });

            migrationBuilder.CreateIndex(
                name: "IX_Customizations_WorldId_UpdatedOn",
                schema: "Game",
                table: "Customizations",
                columns: new[] { "WorldId", "UpdatedOn" });

            migrationBuilder.CreateIndex(
                name: "IX_Customizations_WorldId_Version",
                schema: "Game",
                table: "Customizations",
                columns: new[] { "WorldId", "Version" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Customizations",
                schema: "Game");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Worlds_Id",
                schema: "Game",
                table: "Worlds");
        }
    }
}
