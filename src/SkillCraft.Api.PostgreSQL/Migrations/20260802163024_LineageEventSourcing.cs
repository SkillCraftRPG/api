using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SkillCraft.Api.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class LineageEventSourcing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lineages_Lineages_ParentId",
                schema: "Game",
                table: "Lineages");

            migrationBuilder.DropForeignKey(
                name: "FK_Lineages_Worlds_WorldEntityWorldId",
                schema: "Game",
                table: "Lineages");

            migrationBuilder.DropTable(
                name: "History");

            migrationBuilder.DropIndex(
                name: "IX_Lineages_WorldEntityWorldId",
                schema: "Game",
                table: "Lineages");

            migrationBuilder.DropIndex(
                name: "IX_Lineages_WorldId_CreatedBy",
                schema: "Game",
                table: "Lineages");

            migrationBuilder.DropIndex(
                name: "IX_Lineages_WorldId_CreatedOn",
                schema: "Game",
                table: "Lineages");

            migrationBuilder.DropIndex(
                name: "IX_Lineages_WorldId_UpdatedBy",
                schema: "Game",
                table: "Lineages");

            migrationBuilder.DropIndex(
                name: "IX_Lineages_WorldId_UpdatedOn",
                schema: "Game",
                table: "Lineages");

            migrationBuilder.DropIndex(
                name: "IX_Lineages_WorldId_Version",
                schema: "Game",
                table: "Lineages");

            migrationBuilder.DropColumn(
                name: "WorldEntityWorldId",
                schema: "Game",
                table: "Lineages");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                schema: "Game",
                table: "Lineages",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "Game",
                table: "Lineages",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "StreamId",
                schema: "Game",
                table: "Lineages",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                schema: "Game",
                table: "LineageFeatures",
                type: "text",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "Game",
                table: "LineageFeatures",
                type: "text",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<int>(
                name: "LineageEntityLineageId",
                schema: "Game",
                table: "Languages",
                type: "integer",
                nullable: true);

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
                name: "IX_Languages_LineageEntityLineageId",
                schema: "Game",
                table: "Languages",
                column: "LineageEntityLineageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Languages_Lineages_LineageEntityLineageId",
                schema: "Game",
                table: "Languages",
                column: "LineageEntityLineageId",
                principalSchema: "Game",
                principalTable: "Lineages",
                principalColumn: "LineageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lineages_Lineages_ParentId",
                schema: "Game",
                table: "Lineages",
                column: "ParentId",
                principalSchema: "Game",
                principalTable: "Lineages",
                principalColumn: "LineageId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Lineages_Worlds_WorldId",
                schema: "Game",
                table: "Lineages",
                column: "WorldId",
                principalSchema: "Game",
                principalTable: "Worlds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Languages_Lineages_LineageEntityLineageId",
                schema: "Game",
                table: "Languages");

            migrationBuilder.DropForeignKey(
                name: "FK_Lineages_Lineages_ParentId",
                schema: "Game",
                table: "Lineages");

            migrationBuilder.DropForeignKey(
                name: "FK_Lineages_Worlds_WorldId",
                schema: "Game",
                table: "Lineages");

            migrationBuilder.DropIndex(
                name: "IX_Lineages_CreatedBy",
                schema: "Game",
                table: "Lineages");

            migrationBuilder.DropIndex(
                name: "IX_Lineages_CreatedOn",
                schema: "Game",
                table: "Lineages");

            migrationBuilder.DropIndex(
                name: "IX_Lineages_StreamId",
                schema: "Game",
                table: "Lineages");

            migrationBuilder.DropIndex(
                name: "IX_Lineages_UpdatedBy",
                schema: "Game",
                table: "Lineages");

            migrationBuilder.DropIndex(
                name: "IX_Lineages_UpdatedOn",
                schema: "Game",
                table: "Lineages");

            migrationBuilder.DropIndex(
                name: "IX_Lineages_Version",
                schema: "Game",
                table: "Lineages");

            migrationBuilder.DropIndex(
                name: "IX_Languages_LineageEntityLineageId",
                schema: "Game",
                table: "Languages");

            migrationBuilder.DropColumn(
                name: "StreamId",
                schema: "Game",
                table: "Lineages");

            migrationBuilder.DropColumn(
                name: "LineageEntityLineageId",
                schema: "Game",
                table: "Languages");

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                schema: "Game",
                table: "Lineages",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                schema: "Game",
                table: "Lineages",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WorldEntityWorldId",
                schema: "Game",
                table: "Lineages",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                schema: "Game",
                table: "LineageFeatures",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                schema: "Game",
                table: "LineageFeatures",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "History",
                columns: table => new
                {
                    HistoryRecordId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventData = table.Column<string>(type: "text", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventType = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    OccurredOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ResourceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResourceKind = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    WorldId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_History", x => x.HistoryRecordId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Lineages_WorldEntityWorldId",
                schema: "Game",
                table: "Lineages",
                column: "WorldEntityWorldId");

            migrationBuilder.CreateIndex(
                name: "IX_Lineages_WorldId_CreatedBy",
                schema: "Game",
                table: "Lineages",
                columns: new[] { "WorldId", "CreatedBy" });

            migrationBuilder.CreateIndex(
                name: "IX_Lineages_WorldId_CreatedOn",
                schema: "Game",
                table: "Lineages",
                columns: new[] { "WorldId", "CreatedOn" });

            migrationBuilder.CreateIndex(
                name: "IX_Lineages_WorldId_UpdatedBy",
                schema: "Game",
                table: "Lineages",
                columns: new[] { "WorldId", "UpdatedBy" });

            migrationBuilder.CreateIndex(
                name: "IX_Lineages_WorldId_UpdatedOn",
                schema: "Game",
                table: "Lineages",
                columns: new[] { "WorldId", "UpdatedOn" });

            migrationBuilder.CreateIndex(
                name: "IX_Lineages_WorldId_Version",
                schema: "Game",
                table: "Lineages",
                columns: new[] { "WorldId", "Version" });

            migrationBuilder.CreateIndex(
                name: "IX_History_EventId",
                table: "History",
                column: "EventId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_History_EventType",
                table: "History",
                column: "EventType");

            migrationBuilder.CreateIndex(
                name: "IX_History_OccurredOn",
                table: "History",
                column: "OccurredOn");

            migrationBuilder.CreateIndex(
                name: "IX_History_UserId",
                table: "History",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_History_Version",
                table: "History",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_History_WorldId_ResourceKind_ResourceId",
                table: "History",
                columns: new[] { "WorldId", "ResourceKind", "ResourceId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Lineages_Lineages_ParentId",
                schema: "Game",
                table: "Lineages",
                column: "ParentId",
                principalSchema: "Game",
                principalTable: "Lineages",
                principalColumn: "LineageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lineages_Worlds_WorldEntityWorldId",
                schema: "Game",
                table: "Lineages",
                column: "WorldEntityWorldId",
                principalSchema: "Game",
                principalTable: "Worlds",
                principalColumn: "WorldId");
        }
    }
}
