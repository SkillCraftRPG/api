using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillCraft.Api.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class CasteEventSourcing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Castes_Worlds_WorldEntityWorldId",
                schema: "Game",
                table: "Castes");

            migrationBuilder.DropIndex(
                name: "IX_Castes_WorldEntityWorldId",
                schema: "Game",
                table: "Castes");

            migrationBuilder.DropIndex(
                name: "IX_Castes_WorldId_CreatedBy",
                schema: "Game",
                table: "Castes");

            migrationBuilder.DropIndex(
                name: "IX_Castes_WorldId_CreatedOn",
                schema: "Game",
                table: "Castes");

            migrationBuilder.DropIndex(
                name: "IX_Castes_WorldId_UpdatedBy",
                schema: "Game",
                table: "Castes");

            migrationBuilder.DropIndex(
                name: "IX_Castes_WorldId_UpdatedOn",
                schema: "Game",
                table: "Castes");

            migrationBuilder.DropIndex(
                name: "IX_Castes_WorldId_Version",
                schema: "Game",
                table: "Castes");

            migrationBuilder.DropColumn(
                name: "WorldEntityWorldId",
                schema: "Game",
                table: "Castes");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                schema: "Game",
                table: "Castes",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "Game",
                table: "Castes",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "StreamId",
                schema: "Game",
                table: "Castes",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Castes_CreatedBy",
                schema: "Game",
                table: "Castes",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Castes_CreatedOn",
                schema: "Game",
                table: "Castes",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Castes_StreamId",
                schema: "Game",
                table: "Castes",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Castes_UpdatedBy",
                schema: "Game",
                table: "Castes",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Castes_UpdatedOn",
                schema: "Game",
                table: "Castes",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Castes_Version",
                schema: "Game",
                table: "Castes",
                column: "Version");

            migrationBuilder.AddForeignKey(
                name: "FK_Castes_Worlds_WorldId",
                schema: "Game",
                table: "Castes",
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
                name: "FK_Castes_Worlds_WorldId",
                schema: "Game",
                table: "Castes");

            migrationBuilder.DropIndex(
                name: "IX_Castes_CreatedBy",
                schema: "Game",
                table: "Castes");

            migrationBuilder.DropIndex(
                name: "IX_Castes_CreatedOn",
                schema: "Game",
                table: "Castes");

            migrationBuilder.DropIndex(
                name: "IX_Castes_StreamId",
                schema: "Game",
                table: "Castes");

            migrationBuilder.DropIndex(
                name: "IX_Castes_UpdatedBy",
                schema: "Game",
                table: "Castes");

            migrationBuilder.DropIndex(
                name: "IX_Castes_UpdatedOn",
                schema: "Game",
                table: "Castes");

            migrationBuilder.DropIndex(
                name: "IX_Castes_Version",
                schema: "Game",
                table: "Castes");

            migrationBuilder.DropColumn(
                name: "StreamId",
                schema: "Game",
                table: "Castes");

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                schema: "Game",
                table: "Castes",
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
                table: "Castes",
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
                table: "Castes",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Castes_WorldEntityWorldId",
                schema: "Game",
                table: "Castes",
                column: "WorldEntityWorldId");

            migrationBuilder.CreateIndex(
                name: "IX_Castes_WorldId_CreatedBy",
                schema: "Game",
                table: "Castes",
                columns: new[] { "WorldId", "CreatedBy" });

            migrationBuilder.CreateIndex(
                name: "IX_Castes_WorldId_CreatedOn",
                schema: "Game",
                table: "Castes",
                columns: new[] { "WorldId", "CreatedOn" });

            migrationBuilder.CreateIndex(
                name: "IX_Castes_WorldId_UpdatedBy",
                schema: "Game",
                table: "Castes",
                columns: new[] { "WorldId", "UpdatedBy" });

            migrationBuilder.CreateIndex(
                name: "IX_Castes_WorldId_UpdatedOn",
                schema: "Game",
                table: "Castes",
                columns: new[] { "WorldId", "UpdatedOn" });

            migrationBuilder.CreateIndex(
                name: "IX_Castes_WorldId_Version",
                schema: "Game",
                table: "Castes",
                columns: new[] { "WorldId", "Version" });

            migrationBuilder.AddForeignKey(
                name: "FK_Castes_Worlds_WorldEntityWorldId",
                schema: "Game",
                table: "Castes",
                column: "WorldEntityWorldId",
                principalSchema: "Game",
                principalTable: "Worlds",
                principalColumn: "WorldId");
        }
    }
}
