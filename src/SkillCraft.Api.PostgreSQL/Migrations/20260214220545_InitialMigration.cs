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
                    OwnerId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
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
                });

            migrationBuilder.CreateTable(
                name: "Customizations",
                schema: "Game",
                columns: table => new
                {
                    CustomizationId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WorldId = table.Column<int>(type: "integer", nullable: false),
                    WorldUid = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Kind = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Summary = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
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
                        principalColumn: "WorldId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StorageSummary",
                schema: "Game",
                columns: table => new
                {
                    WorldId = table.Column<int>(type: "integer", nullable: false),
                    WorldUid = table.Column<Guid>(type: "uuid", nullable: false),
                    AllocatedBytes = table.Column<long>(type: "bigint", nullable: false),
                    UsedBytes = table.Column<long>(type: "bigint", nullable: false),
                    RemainingBytes = table.Column<long>(type: "bigint", nullable: false),
                    StreamId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StorageSummary", x => x.WorldId);
                    table.ForeignKey(
                        name: "FK_StorageSummary_Worlds_WorldId",
                        column: x => x.WorldId,
                        principalSchema: "Game",
                        principalTable: "Worlds",
                        principalColumn: "WorldId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StorageDetail",
                schema: "Game",
                columns: table => new
                {
                    StorageDetailId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Key = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    WorldId = table.Column<int>(type: "integer", nullable: false),
                    WorldUid = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityKind = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StorageDetail", x => x.StorageDetailId);
                    table.ForeignKey(
                        name: "FK_StorageDetail_StorageSummary_WorldId",
                        column: x => x.WorldId,
                        principalSchema: "Game",
                        principalTable: "StorageSummary",
                        principalColumn: "WorldId",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "IX_Customizations_Kind",
                schema: "Game",
                table: "Customizations",
                column: "Kind");

            migrationBuilder.CreateIndex(
                name: "IX_Customizations_Name",
                schema: "Game",
                table: "Customizations",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Customizations_StreamId",
                schema: "Game",
                table: "Customizations",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customizations_Summary",
                schema: "Game",
                table: "Customizations",
                column: "Summary");

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
                name: "IX_Customizations_WorldUid",
                schema: "Game",
                table: "Customizations",
                column: "WorldUid");

            migrationBuilder.CreateIndex(
                name: "IX_StorageDetail_Key",
                schema: "Game",
                table: "StorageDetail",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StorageDetail_Size",
                schema: "Game",
                table: "StorageDetail",
                column: "Size");

            migrationBuilder.CreateIndex(
                name: "IX_StorageDetail_WorldId_EntityKind_EntityId",
                schema: "Game",
                table: "StorageDetail",
                columns: new[] { "WorldId", "EntityKind", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_StorageDetail_WorldUid_EntityKind_EntityId",
                schema: "Game",
                table: "StorageDetail",
                columns: new[] { "WorldUid", "EntityKind", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_StorageSummary_AllocatedBytes",
                schema: "Game",
                table: "StorageSummary",
                column: "AllocatedBytes");

            migrationBuilder.CreateIndex(
                name: "IX_StorageSummary_CreatedBy",
                schema: "Game",
                table: "StorageSummary",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_StorageSummary_CreatedOn",
                schema: "Game",
                table: "StorageSummary",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_StorageSummary_RemainingBytes",
                schema: "Game",
                table: "StorageSummary",
                column: "RemainingBytes");

            migrationBuilder.CreateIndex(
                name: "IX_StorageSummary_StreamId",
                schema: "Game",
                table: "StorageSummary",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StorageSummary_UpdatedBy",
                schema: "Game",
                table: "StorageSummary",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_StorageSummary_UpdatedOn",
                schema: "Game",
                table: "StorageSummary",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_StorageSummary_UsedBytes",
                schema: "Game",
                table: "StorageSummary",
                column: "UsedBytes");

            migrationBuilder.CreateIndex(
                name: "IX_StorageSummary_Version",
                schema: "Game",
                table: "StorageSummary",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_StorageSummary_WorldUid",
                schema: "Game",
                table: "StorageSummary",
                column: "WorldUid",
                unique: true);

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
                name: "Customizations",
                schema: "Game");

            migrationBuilder.DropTable(
                name: "StorageDetail",
                schema: "Game");

            migrationBuilder.DropTable(
                name: "StorageSummary",
                schema: "Game");

            migrationBuilder.DropTable(
                name: "Worlds",
                schema: "Game");
        }
    }
}
