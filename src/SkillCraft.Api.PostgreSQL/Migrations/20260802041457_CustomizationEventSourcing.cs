using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillCraft.Api.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class CustomizationEventSourcing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customizations_Worlds_WorldEntityWorldId",
                schema: "Game",
                table: "Customizations");

            migrationBuilder.DropIndex(
                name: "IX_Customizations_WorldEntityWorldId",
                schema: "Game",
                table: "Customizations");

            migrationBuilder.DropIndex(
                name: "IX_Customizations_WorldId_CreatedBy",
                schema: "Game",
                table: "Customizations");

            migrationBuilder.DropIndex(
                name: "IX_Customizations_WorldId_CreatedOn",
                schema: "Game",
                table: "Customizations");

            migrationBuilder.DropIndex(
                name: "IX_Customizations_WorldId_UpdatedBy",
                schema: "Game",
                table: "Customizations");

            migrationBuilder.DropIndex(
                name: "IX_Customizations_WorldId_UpdatedOn",
                schema: "Game",
                table: "Customizations");

            migrationBuilder.DropIndex(
                name: "IX_Customizations_WorldId_Version",
                schema: "Game",
                table: "Customizations");

            migrationBuilder.DropColumn(
                name: "WorldEntityWorldId",
                schema: "Game",
                table: "Customizations");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                schema: "Game",
                table: "Customizations",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "Kind",
                schema: "Game",
                table: "Customizations",
                type: "character varying(16)",
                maxLength: 16,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "Game",
                table: "Customizations",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "StreamId",
                schema: "Game",
                table: "Customizations",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

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
                name: "IX_Customizations_StreamId",
                schema: "Game",
                table: "Customizations",
                column: "StreamId",
                unique: true);

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

            migrationBuilder.AddForeignKey(
                name: "FK_Customizations_Worlds_WorldId",
                schema: "Game",
                table: "Customizations",
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
                name: "FK_Customizations_Worlds_WorldId",
                schema: "Game",
                table: "Customizations");

            migrationBuilder.DropIndex(
                name: "IX_Customizations_CreatedBy",
                schema: "Game",
                table: "Customizations");

            migrationBuilder.DropIndex(
                name: "IX_Customizations_CreatedOn",
                schema: "Game",
                table: "Customizations");

            migrationBuilder.DropIndex(
                name: "IX_Customizations_StreamId",
                schema: "Game",
                table: "Customizations");

            migrationBuilder.DropIndex(
                name: "IX_Customizations_UpdatedBy",
                schema: "Game",
                table: "Customizations");

            migrationBuilder.DropIndex(
                name: "IX_Customizations_UpdatedOn",
                schema: "Game",
                table: "Customizations");

            migrationBuilder.DropIndex(
                name: "IX_Customizations_Version",
                schema: "Game",
                table: "Customizations");

            migrationBuilder.DropColumn(
                name: "StreamId",
                schema: "Game",
                table: "Customizations");

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                schema: "Game",
                table: "Customizations",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Kind",
                schema: "Game",
                table: "Customizations",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(16)",
                oldMaxLength: 16);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                schema: "Game",
                table: "Customizations",
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
                table: "Customizations",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customizations_WorldEntityWorldId",
                schema: "Game",
                table: "Customizations",
                column: "WorldEntityWorldId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Customizations_Worlds_WorldEntityWorldId",
                schema: "Game",
                table: "Customizations",
                column: "WorldEntityWorldId",
                principalSchema: "Game",
                principalTable: "Worlds",
                principalColumn: "WorldId");
        }
    }
}
