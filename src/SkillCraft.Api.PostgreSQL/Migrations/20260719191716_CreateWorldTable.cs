using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SkillCraft.Api.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class CreateWorldTable : Migration
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
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Worlds", x => x.WorldId);
                });

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
                name: "Worlds",
                schema: "Game");
        }
    }
}
