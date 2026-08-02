using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillCraft.Api.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class LanguageEventSourcing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Languages_Worlds_WorldEntityWorldId",
                schema: "Game",
                table: "Languages");

            migrationBuilder.DropIndex(
                name: "IX_Languages_WorldEntityWorldId",
                schema: "Game",
                table: "Languages");

            migrationBuilder.DropIndex(
                name: "IX_Languages_WorldId_CreatedBy",
                schema: "Game",
                table: "Languages");

            migrationBuilder.DropIndex(
                name: "IX_Languages_WorldId_CreatedOn",
                schema: "Game",
                table: "Languages");

            migrationBuilder.DropIndex(
                name: "IX_Languages_WorldId_UpdatedBy",
                schema: "Game",
                table: "Languages");

            migrationBuilder.DropIndex(
                name: "IX_Languages_WorldId_UpdatedOn",
                schema: "Game",
                table: "Languages");

            migrationBuilder.DropIndex(
                name: "IX_Languages_WorldId_Version",
                schema: "Game",
                table: "Languages");

            migrationBuilder.DropColumn(
                name: "WorldEntityWorldId",
                schema: "Game",
                table: "Languages");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                schema: "Game",
                table: "Languages",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "Game",
                table: "Languages",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "StreamId",
                schema: "Game",
                table: "Languages",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

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
                name: "IX_Languages_StreamId",
                schema: "Game",
                table: "Languages",
                column: "StreamId",
                unique: true);

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

            migrationBuilder.AddForeignKey(
                name: "FK_Languages_Worlds_WorldId",
                schema: "Game",
                table: "Languages",
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
                name: "FK_Languages_Worlds_WorldId",
                schema: "Game",
                table: "Languages");

            migrationBuilder.DropIndex(
                name: "IX_Languages_CreatedBy",
                schema: "Game",
                table: "Languages");

            migrationBuilder.DropIndex(
                name: "IX_Languages_CreatedOn",
                schema: "Game",
                table: "Languages");

            migrationBuilder.DropIndex(
                name: "IX_Languages_StreamId",
                schema: "Game",
                table: "Languages");

            migrationBuilder.DropIndex(
                name: "IX_Languages_UpdatedBy",
                schema: "Game",
                table: "Languages");

            migrationBuilder.DropIndex(
                name: "IX_Languages_UpdatedOn",
                schema: "Game",
                table: "Languages");

            migrationBuilder.DropIndex(
                name: "IX_Languages_Version",
                schema: "Game",
                table: "Languages");

            migrationBuilder.DropColumn(
                name: "StreamId",
                schema: "Game",
                table: "Languages");

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                schema: "Game",
                table: "Languages",
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
                table: "Languages",
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
                table: "Languages",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Languages_WorldEntityWorldId",
                schema: "Game",
                table: "Languages",
                column: "WorldEntityWorldId");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_WorldId_CreatedBy",
                schema: "Game",
                table: "Languages",
                columns: new[] { "WorldId", "CreatedBy" });

            migrationBuilder.CreateIndex(
                name: "IX_Languages_WorldId_CreatedOn",
                schema: "Game",
                table: "Languages",
                columns: new[] { "WorldId", "CreatedOn" });

            migrationBuilder.CreateIndex(
                name: "IX_Languages_WorldId_UpdatedBy",
                schema: "Game",
                table: "Languages",
                columns: new[] { "WorldId", "UpdatedBy" });

            migrationBuilder.CreateIndex(
                name: "IX_Languages_WorldId_UpdatedOn",
                schema: "Game",
                table: "Languages",
                columns: new[] { "WorldId", "UpdatedOn" });

            migrationBuilder.CreateIndex(
                name: "IX_Languages_WorldId_Version",
                schema: "Game",
                table: "Languages",
                columns: new[] { "WorldId", "Version" });

            migrationBuilder.AddForeignKey(
                name: "FK_Languages_Worlds_WorldEntityWorldId",
                schema: "Game",
                table: "Languages",
                column: "WorldEntityWorldId",
                principalSchema: "Game",
                principalTable: "Worlds",
                principalColumn: "WorldId");
        }
    }
}
