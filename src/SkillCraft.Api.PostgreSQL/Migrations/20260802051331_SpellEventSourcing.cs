using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillCraft.Api.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class SpellEventSourcing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Spells_Worlds_WorldEntityWorldId",
                schema: "Game",
                table: "Spells");

            migrationBuilder.DropIndex(
                name: "IX_Spells_WorldEntityWorldId",
                schema: "Game",
                table: "Spells");

            migrationBuilder.DropIndex(
                name: "IX_Spells_WorldId_CreatedBy",
                schema: "Game",
                table: "Spells");

            migrationBuilder.DropIndex(
                name: "IX_Spells_WorldId_CreatedOn",
                schema: "Game",
                table: "Spells");

            migrationBuilder.DropIndex(
                name: "IX_Spells_WorldId_UpdatedBy",
                schema: "Game",
                table: "Spells");

            migrationBuilder.DropIndex(
                name: "IX_Spells_WorldId_UpdatedOn",
                schema: "Game",
                table: "Spells");

            migrationBuilder.DropIndex(
                name: "IX_Spells_WorldId_Version",
                schema: "Game",
                table: "Spells");

            migrationBuilder.DropColumn(
                name: "WorldEntityWorldId",
                schema: "Game",
                table: "Spells");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                schema: "Game",
                table: "Spells",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "Game",
                table: "Spells",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "StreamId",
                schema: "Game",
                table: "Spells",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Spells_CreatedBy",
                schema: "Game",
                table: "Spells",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Spells_CreatedOn",
                schema: "Game",
                table: "Spells",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Spells_StreamId",
                schema: "Game",
                table: "Spells",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Spells_UpdatedBy",
                schema: "Game",
                table: "Spells",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Spells_UpdatedOn",
                schema: "Game",
                table: "Spells",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Spells_Version",
                schema: "Game",
                table: "Spells",
                column: "Version");

            migrationBuilder.AddForeignKey(
                name: "FK_Spells_Worlds_WorldId",
                schema: "Game",
                table: "Spells",
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
                name: "FK_Spells_Worlds_WorldId",
                schema: "Game",
                table: "Spells");

            migrationBuilder.DropIndex(
                name: "IX_Spells_CreatedBy",
                schema: "Game",
                table: "Spells");

            migrationBuilder.DropIndex(
                name: "IX_Spells_CreatedOn",
                schema: "Game",
                table: "Spells");

            migrationBuilder.DropIndex(
                name: "IX_Spells_StreamId",
                schema: "Game",
                table: "Spells");

            migrationBuilder.DropIndex(
                name: "IX_Spells_UpdatedBy",
                schema: "Game",
                table: "Spells");

            migrationBuilder.DropIndex(
                name: "IX_Spells_UpdatedOn",
                schema: "Game",
                table: "Spells");

            migrationBuilder.DropIndex(
                name: "IX_Spells_Version",
                schema: "Game",
                table: "Spells");

            migrationBuilder.DropColumn(
                name: "StreamId",
                schema: "Game",
                table: "Spells");

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                schema: "Game",
                table: "Spells",
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
                table: "Spells",
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
                table: "Spells",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Spells_WorldEntityWorldId",
                schema: "Game",
                table: "Spells",
                column: "WorldEntityWorldId");

            migrationBuilder.CreateIndex(
                name: "IX_Spells_WorldId_CreatedBy",
                schema: "Game",
                table: "Spells",
                columns: new[] { "WorldId", "CreatedBy" });

            migrationBuilder.CreateIndex(
                name: "IX_Spells_WorldId_CreatedOn",
                schema: "Game",
                table: "Spells",
                columns: new[] { "WorldId", "CreatedOn" });

            migrationBuilder.CreateIndex(
                name: "IX_Spells_WorldId_UpdatedBy",
                schema: "Game",
                table: "Spells",
                columns: new[] { "WorldId", "UpdatedBy" });

            migrationBuilder.CreateIndex(
                name: "IX_Spells_WorldId_UpdatedOn",
                schema: "Game",
                table: "Spells",
                columns: new[] { "WorldId", "UpdatedOn" });

            migrationBuilder.CreateIndex(
                name: "IX_Spells_WorldId_Version",
                schema: "Game",
                table: "Spells",
                columns: new[] { "WorldId", "Version" });

            migrationBuilder.AddForeignKey(
                name: "FK_Spells_Worlds_WorldEntityWorldId",
                schema: "Game",
                table: "Spells",
                column: "WorldEntityWorldId",
                principalSchema: "Game",
                principalTable: "Worlds",
                principalColumn: "WorldId");
        }
    }
}
