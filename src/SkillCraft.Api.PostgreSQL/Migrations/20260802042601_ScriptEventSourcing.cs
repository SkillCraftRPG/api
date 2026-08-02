using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillCraft.Api.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class ScriptEventSourcing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Scripts_Worlds_WorldEntityWorldId",
                schema: "Game",
                table: "Scripts");

            migrationBuilder.DropIndex(
                name: "IX_Scripts_WorldEntityWorldId",
                schema: "Game",
                table: "Scripts");

            migrationBuilder.DropIndex(
                name: "IX_Scripts_WorldId_CreatedBy",
                schema: "Game",
                table: "Scripts");

            migrationBuilder.DropIndex(
                name: "IX_Scripts_WorldId_CreatedOn",
                schema: "Game",
                table: "Scripts");

            migrationBuilder.DropIndex(
                name: "IX_Scripts_WorldId_UpdatedBy",
                schema: "Game",
                table: "Scripts");

            migrationBuilder.DropIndex(
                name: "IX_Scripts_WorldId_UpdatedOn",
                schema: "Game",
                table: "Scripts");

            migrationBuilder.DropIndex(
                name: "IX_Scripts_WorldId_Version",
                schema: "Game",
                table: "Scripts");

            migrationBuilder.DropColumn(
                name: "WorldEntityWorldId",
                schema: "Game",
                table: "Scripts");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                schema: "Game",
                table: "Scripts",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "Game",
                table: "Scripts",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "StreamId",
                schema: "Game",
                table: "Scripts",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Scripts_CreatedBy",
                schema: "Game",
                table: "Scripts",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Scripts_CreatedOn",
                schema: "Game",
                table: "Scripts",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Scripts_StreamId",
                schema: "Game",
                table: "Scripts",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Scripts_UpdatedBy",
                schema: "Game",
                table: "Scripts",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Scripts_UpdatedOn",
                schema: "Game",
                table: "Scripts",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Scripts_Version",
                schema: "Game",
                table: "Scripts",
                column: "Version");

            migrationBuilder.AddForeignKey(
                name: "FK_Scripts_Worlds_WorldId",
                schema: "Game",
                table: "Scripts",
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
                name: "FK_Scripts_Worlds_WorldId",
                schema: "Game",
                table: "Scripts");

            migrationBuilder.DropIndex(
                name: "IX_Scripts_CreatedBy",
                schema: "Game",
                table: "Scripts");

            migrationBuilder.DropIndex(
                name: "IX_Scripts_CreatedOn",
                schema: "Game",
                table: "Scripts");

            migrationBuilder.DropIndex(
                name: "IX_Scripts_StreamId",
                schema: "Game",
                table: "Scripts");

            migrationBuilder.DropIndex(
                name: "IX_Scripts_UpdatedBy",
                schema: "Game",
                table: "Scripts");

            migrationBuilder.DropIndex(
                name: "IX_Scripts_UpdatedOn",
                schema: "Game",
                table: "Scripts");

            migrationBuilder.DropIndex(
                name: "IX_Scripts_Version",
                schema: "Game",
                table: "Scripts");

            migrationBuilder.DropColumn(
                name: "StreamId",
                schema: "Game",
                table: "Scripts");

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                schema: "Game",
                table: "Scripts",
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
                table: "Scripts",
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
                table: "Scripts",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Scripts_WorldEntityWorldId",
                schema: "Game",
                table: "Scripts",
                column: "WorldEntityWorldId");

            migrationBuilder.CreateIndex(
                name: "IX_Scripts_WorldId_CreatedBy",
                schema: "Game",
                table: "Scripts",
                columns: new[] { "WorldId", "CreatedBy" });

            migrationBuilder.CreateIndex(
                name: "IX_Scripts_WorldId_CreatedOn",
                schema: "Game",
                table: "Scripts",
                columns: new[] { "WorldId", "CreatedOn" });

            migrationBuilder.CreateIndex(
                name: "IX_Scripts_WorldId_UpdatedBy",
                schema: "Game",
                table: "Scripts",
                columns: new[] { "WorldId", "UpdatedBy" });

            migrationBuilder.CreateIndex(
                name: "IX_Scripts_WorldId_UpdatedOn",
                schema: "Game",
                table: "Scripts",
                columns: new[] { "WorldId", "UpdatedOn" });

            migrationBuilder.CreateIndex(
                name: "IX_Scripts_WorldId_Version",
                schema: "Game",
                table: "Scripts",
                columns: new[] { "WorldId", "Version" });

            migrationBuilder.AddForeignKey(
                name: "FK_Scripts_Worlds_WorldEntityWorldId",
                schema: "Game",
                table: "Scripts",
                column: "WorldEntityWorldId",
                principalSchema: "Game",
                principalTable: "Worlds",
                principalColumn: "WorldId");
        }
    }
}
