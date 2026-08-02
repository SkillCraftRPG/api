using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillCraft.Api.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class ItemEventSourcing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Worlds_WorldEntityWorldId",
                schema: "Game",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_WorldEntityWorldId",
                schema: "Game",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_WorldId_CreatedBy",
                schema: "Game",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_WorldId_CreatedOn",
                schema: "Game",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_WorldId_UpdatedBy",
                schema: "Game",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_WorldId_UpdatedOn",
                schema: "Game",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_WorldId_Version",
                schema: "Game",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "WorldEntityWorldId",
                schema: "Game",
                table: "Items");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                schema: "Game",
                table: "Items",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "Game",
                table: "Items",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "StreamId",
                schema: "Game",
                table: "Items",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Items_CreatedBy",
                schema: "Game",
                table: "Items",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Items_CreatedOn",
                schema: "Game",
                table: "Items",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Items_StreamId",
                schema: "Game",
                table: "Items",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_UpdatedBy",
                schema: "Game",
                table: "Items",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Items_UpdatedOn",
                schema: "Game",
                table: "Items",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Items_Version",
                schema: "Game",
                table: "Items",
                column: "Version");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Worlds_WorldId",
                schema: "Game",
                table: "Items",
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
                name: "FK_Items_Worlds_WorldId",
                schema: "Game",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_CreatedBy",
                schema: "Game",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_CreatedOn",
                schema: "Game",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_StreamId",
                schema: "Game",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_UpdatedBy",
                schema: "Game",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_UpdatedOn",
                schema: "Game",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_Version",
                schema: "Game",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "StreamId",
                schema: "Game",
                table: "Items");

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                schema: "Game",
                table: "Items",
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
                table: "Items",
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
                table: "Items",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_WorldEntityWorldId",
                schema: "Game",
                table: "Items",
                column: "WorldEntityWorldId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_WorldId_CreatedBy",
                schema: "Game",
                table: "Items",
                columns: new[] { "WorldId", "CreatedBy" });

            migrationBuilder.CreateIndex(
                name: "IX_Items_WorldId_CreatedOn",
                schema: "Game",
                table: "Items",
                columns: new[] { "WorldId", "CreatedOn" });

            migrationBuilder.CreateIndex(
                name: "IX_Items_WorldId_UpdatedBy",
                schema: "Game",
                table: "Items",
                columns: new[] { "WorldId", "UpdatedBy" });

            migrationBuilder.CreateIndex(
                name: "IX_Items_WorldId_UpdatedOn",
                schema: "Game",
                table: "Items",
                columns: new[] { "WorldId", "UpdatedOn" });

            migrationBuilder.CreateIndex(
                name: "IX_Items_WorldId_Version",
                schema: "Game",
                table: "Items",
                columns: new[] { "WorldId", "Version" });

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Worlds_WorldEntityWorldId",
                schema: "Game",
                table: "Items",
                column: "WorldEntityWorldId",
                principalSchema: "Game",
                principalTable: "Worlds",
                principalColumn: "WorldId");
        }
    }
}
