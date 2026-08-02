using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillCraft.Api.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class TalentEventSourcing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Talents_WorldId_CreatedBy",
                schema: "Game",
                table: "Talents");

            migrationBuilder.DropIndex(
                name: "IX_Talents_WorldId_CreatedOn",
                schema: "Game",
                table: "Talents");

            migrationBuilder.DropIndex(
                name: "IX_Talents_WorldId_UpdatedBy",
                schema: "Game",
                table: "Talents");

            migrationBuilder.DropIndex(
                name: "IX_Talents_WorldId_UpdatedOn",
                schema: "Game",
                table: "Talents");

            migrationBuilder.DropIndex(
                name: "IX_Talents_WorldId_Version",
                schema: "Game",
                table: "Talents");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                schema: "Game",
                table: "Talents",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "Game",
                table: "Talents",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "StreamId",
                schema: "Game",
                table: "Talents",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

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
                name: "IX_Talents_StreamId",
                schema: "Game",
                table: "Talents",
                column: "StreamId",
                unique: true);

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Talents_CreatedBy",
                schema: "Game",
                table: "Talents");

            migrationBuilder.DropIndex(
                name: "IX_Talents_CreatedOn",
                schema: "Game",
                table: "Talents");

            migrationBuilder.DropIndex(
                name: "IX_Talents_StreamId",
                schema: "Game",
                table: "Talents");

            migrationBuilder.DropIndex(
                name: "IX_Talents_UpdatedBy",
                schema: "Game",
                table: "Talents");

            migrationBuilder.DropIndex(
                name: "IX_Talents_UpdatedOn",
                schema: "Game",
                table: "Talents");

            migrationBuilder.DropIndex(
                name: "IX_Talents_Version",
                schema: "Game",
                table: "Talents");

            migrationBuilder.DropColumn(
                name: "StreamId",
                schema: "Game",
                table: "Talents");

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                schema: "Game",
                table: "Talents",
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
                table: "Talents",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);

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
    }
}
