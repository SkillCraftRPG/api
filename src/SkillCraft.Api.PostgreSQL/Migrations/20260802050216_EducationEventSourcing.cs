using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillCraft.Api.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class EducationEventSourcing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Educations_Worlds_WorldEntityWorldId",
                schema: "Game",
                table: "Educations");

            migrationBuilder.DropIndex(
                name: "IX_Educations_WorldEntityWorldId",
                schema: "Game",
                table: "Educations");

            migrationBuilder.DropIndex(
                name: "IX_Educations_WorldId_CreatedBy",
                schema: "Game",
                table: "Educations");

            migrationBuilder.DropIndex(
                name: "IX_Educations_WorldId_CreatedOn",
                schema: "Game",
                table: "Educations");

            migrationBuilder.DropIndex(
                name: "IX_Educations_WorldId_UpdatedBy",
                schema: "Game",
                table: "Educations");

            migrationBuilder.DropIndex(
                name: "IX_Educations_WorldId_UpdatedOn",
                schema: "Game",
                table: "Educations");

            migrationBuilder.DropIndex(
                name: "IX_Educations_WorldId_Version",
                schema: "Game",
                table: "Educations");

            migrationBuilder.DropColumn(
                name: "WorldEntityWorldId",
                schema: "Game",
                table: "Educations");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                schema: "Game",
                table: "Educations",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "Game",
                table: "Educations",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "StreamId",
                schema: "Game",
                table: "Educations",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Educations_CreatedBy",
                schema: "Game",
                table: "Educations",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Educations_CreatedOn",
                schema: "Game",
                table: "Educations",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Educations_StreamId",
                schema: "Game",
                table: "Educations",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Educations_UpdatedBy",
                schema: "Game",
                table: "Educations",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Educations_UpdatedOn",
                schema: "Game",
                table: "Educations",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Educations_Version",
                schema: "Game",
                table: "Educations",
                column: "Version");

            migrationBuilder.AddForeignKey(
                name: "FK_Educations_Worlds_WorldId",
                schema: "Game",
                table: "Educations",
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
                name: "FK_Educations_Worlds_WorldId",
                schema: "Game",
                table: "Educations");

            migrationBuilder.DropIndex(
                name: "IX_Educations_CreatedBy",
                schema: "Game",
                table: "Educations");

            migrationBuilder.DropIndex(
                name: "IX_Educations_CreatedOn",
                schema: "Game",
                table: "Educations");

            migrationBuilder.DropIndex(
                name: "IX_Educations_StreamId",
                schema: "Game",
                table: "Educations");

            migrationBuilder.DropIndex(
                name: "IX_Educations_UpdatedBy",
                schema: "Game",
                table: "Educations");

            migrationBuilder.DropIndex(
                name: "IX_Educations_UpdatedOn",
                schema: "Game",
                table: "Educations");

            migrationBuilder.DropIndex(
                name: "IX_Educations_Version",
                schema: "Game",
                table: "Educations");

            migrationBuilder.DropColumn(
                name: "StreamId",
                schema: "Game",
                table: "Educations");

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                schema: "Game",
                table: "Educations",
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
                table: "Educations",
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
                table: "Educations",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Educations_WorldEntityWorldId",
                schema: "Game",
                table: "Educations",
                column: "WorldEntityWorldId");

            migrationBuilder.CreateIndex(
                name: "IX_Educations_WorldId_CreatedBy",
                schema: "Game",
                table: "Educations",
                columns: new[] { "WorldId", "CreatedBy" });

            migrationBuilder.CreateIndex(
                name: "IX_Educations_WorldId_CreatedOn",
                schema: "Game",
                table: "Educations",
                columns: new[] { "WorldId", "CreatedOn" });

            migrationBuilder.CreateIndex(
                name: "IX_Educations_WorldId_UpdatedBy",
                schema: "Game",
                table: "Educations",
                columns: new[] { "WorldId", "UpdatedBy" });

            migrationBuilder.CreateIndex(
                name: "IX_Educations_WorldId_UpdatedOn",
                schema: "Game",
                table: "Educations",
                columns: new[] { "WorldId", "UpdatedOn" });

            migrationBuilder.CreateIndex(
                name: "IX_Educations_WorldId_Version",
                schema: "Game",
                table: "Educations",
                columns: new[] { "WorldId", "Version" });

            migrationBuilder.AddForeignKey(
                name: "FK_Educations_Worlds_WorldEntityWorldId",
                schema: "Game",
                table: "Educations",
                column: "WorldEntityWorldId",
                principalSchema: "Game",
                principalTable: "Worlds",
                principalColumn: "WorldId");
        }
    }
}
