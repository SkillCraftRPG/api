using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillCraft.Api.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class WorldEventSourcing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Castes_Worlds_WorldId",
                schema: "Game",
                table: "Castes");

            migrationBuilder.DropForeignKey(
                name: "FK_Customizations_Worlds_WorldId",
                schema: "Game",
                table: "Customizations");

            migrationBuilder.DropForeignKey(
                name: "FK_Educations_Worlds_WorldId",
                schema: "Game",
                table: "Educations");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_Worlds_WorldId",
                schema: "Game",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Languages_Worlds_WorldId",
                schema: "Game",
                table: "Languages");

            migrationBuilder.DropForeignKey(
                name: "FK_Lineages_Worlds_WorldId",
                schema: "Game",
                table: "Lineages");

            migrationBuilder.DropForeignKey(
                name: "FK_Scripts_Worlds_WorldId",
                schema: "Game",
                table: "Scripts");

            migrationBuilder.DropForeignKey(
                name: "FK_Spells_Worlds_WorldId",
                schema: "Game",
                table: "Spells");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                schema: "Game",
                table: "Worlds",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "OwnerId",
                schema: "Game",
                table: "Worlds",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "Game",
                table: "Worlds",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "StreamId",
                schema: "Game",
                table: "Worlds",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "WorldEntityWorldId",
                schema: "Game",
                table: "Spells",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WorldEntityWorldId",
                schema: "Game",
                table: "Scripts",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WorldEntityWorldId",
                schema: "Game",
                table: "Lineages",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WorldEntityWorldId",
                schema: "Game",
                table: "Languages",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WorldEntityWorldId",
                schema: "Game",
                table: "Items",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WorldEntityWorldId",
                schema: "Game",
                table: "Educations",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WorldEntityWorldId",
                schema: "Game",
                table: "Customizations",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WorldEntityWorldId",
                schema: "Game",
                table: "Castes",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Worlds_StreamId",
                schema: "Game",
                table: "Worlds",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Spells_WorldEntityWorldId",
                schema: "Game",
                table: "Spells",
                column: "WorldEntityWorldId");

            migrationBuilder.CreateIndex(
                name: "IX_Scripts_WorldEntityWorldId",
                schema: "Game",
                table: "Scripts",
                column: "WorldEntityWorldId");

            migrationBuilder.CreateIndex(
                name: "IX_Lineages_WorldEntityWorldId",
                schema: "Game",
                table: "Lineages",
                column: "WorldEntityWorldId");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_WorldEntityWorldId",
                schema: "Game",
                table: "Languages",
                column: "WorldEntityWorldId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_WorldEntityWorldId",
                schema: "Game",
                table: "Items",
                column: "WorldEntityWorldId");

            migrationBuilder.CreateIndex(
                name: "IX_Educations_WorldEntityWorldId",
                schema: "Game",
                table: "Educations",
                column: "WorldEntityWorldId");

            migrationBuilder.CreateIndex(
                name: "IX_Customizations_WorldEntityWorldId",
                schema: "Game",
                table: "Customizations",
                column: "WorldEntityWorldId");

            migrationBuilder.CreateIndex(
                name: "IX_Castes_WorldEntityWorldId",
                schema: "Game",
                table: "Castes",
                column: "WorldEntityWorldId");

            migrationBuilder.AddForeignKey(
                name: "FK_Castes_Worlds_WorldEntityWorldId",
                schema: "Game",
                table: "Castes",
                column: "WorldEntityWorldId",
                principalSchema: "Game",
                principalTable: "Worlds",
                principalColumn: "WorldId");

            migrationBuilder.AddForeignKey(
                name: "FK_Customizations_Worlds_WorldEntityWorldId",
                schema: "Game",
                table: "Customizations",
                column: "WorldEntityWorldId",
                principalSchema: "Game",
                principalTable: "Worlds",
                principalColumn: "WorldId");

            migrationBuilder.AddForeignKey(
                name: "FK_Educations_Worlds_WorldEntityWorldId",
                schema: "Game",
                table: "Educations",
                column: "WorldEntityWorldId",
                principalSchema: "Game",
                principalTable: "Worlds",
                principalColumn: "WorldId");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Worlds_WorldEntityWorldId",
                schema: "Game",
                table: "Items",
                column: "WorldEntityWorldId",
                principalSchema: "Game",
                principalTable: "Worlds",
                principalColumn: "WorldId");

            migrationBuilder.AddForeignKey(
                name: "FK_Languages_Worlds_WorldEntityWorldId",
                schema: "Game",
                table: "Languages",
                column: "WorldEntityWorldId",
                principalSchema: "Game",
                principalTable: "Worlds",
                principalColumn: "WorldId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lineages_Worlds_WorldEntityWorldId",
                schema: "Game",
                table: "Lineages",
                column: "WorldEntityWorldId",
                principalSchema: "Game",
                principalTable: "Worlds",
                principalColumn: "WorldId");

            migrationBuilder.AddForeignKey(
                name: "FK_Scripts_Worlds_WorldEntityWorldId",
                schema: "Game",
                table: "Scripts",
                column: "WorldEntityWorldId",
                principalSchema: "Game",
                principalTable: "Worlds",
                principalColumn: "WorldId");

            migrationBuilder.AddForeignKey(
                name: "FK_Spells_Worlds_WorldEntityWorldId",
                schema: "Game",
                table: "Spells",
                column: "WorldEntityWorldId",
                principalSchema: "Game",
                principalTable: "Worlds",
                principalColumn: "WorldId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Castes_Worlds_WorldEntityWorldId",
                schema: "Game",
                table: "Castes");

            migrationBuilder.DropForeignKey(
                name: "FK_Customizations_Worlds_WorldEntityWorldId",
                schema: "Game",
                table: "Customizations");

            migrationBuilder.DropForeignKey(
                name: "FK_Educations_Worlds_WorldEntityWorldId",
                schema: "Game",
                table: "Educations");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_Worlds_WorldEntityWorldId",
                schema: "Game",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Languages_Worlds_WorldEntityWorldId",
                schema: "Game",
                table: "Languages");

            migrationBuilder.DropForeignKey(
                name: "FK_Lineages_Worlds_WorldEntityWorldId",
                schema: "Game",
                table: "Lineages");

            migrationBuilder.DropForeignKey(
                name: "FK_Scripts_Worlds_WorldEntityWorldId",
                schema: "Game",
                table: "Scripts");

            migrationBuilder.DropForeignKey(
                name: "FK_Spells_Worlds_WorldEntityWorldId",
                schema: "Game",
                table: "Spells");

            migrationBuilder.DropIndex(
                name: "IX_Worlds_StreamId",
                schema: "Game",
                table: "Worlds");

            migrationBuilder.DropIndex(
                name: "IX_Spells_WorldEntityWorldId",
                schema: "Game",
                table: "Spells");

            migrationBuilder.DropIndex(
                name: "IX_Scripts_WorldEntityWorldId",
                schema: "Game",
                table: "Scripts");

            migrationBuilder.DropIndex(
                name: "IX_Lineages_WorldEntityWorldId",
                schema: "Game",
                table: "Lineages");

            migrationBuilder.DropIndex(
                name: "IX_Languages_WorldEntityWorldId",
                schema: "Game",
                table: "Languages");

            migrationBuilder.DropIndex(
                name: "IX_Items_WorldEntityWorldId",
                schema: "Game",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Educations_WorldEntityWorldId",
                schema: "Game",
                table: "Educations");

            migrationBuilder.DropIndex(
                name: "IX_Customizations_WorldEntityWorldId",
                schema: "Game",
                table: "Customizations");

            migrationBuilder.DropIndex(
                name: "IX_Castes_WorldEntityWorldId",
                schema: "Game",
                table: "Castes");

            migrationBuilder.DropColumn(
                name: "StreamId",
                schema: "Game",
                table: "Worlds");

            migrationBuilder.DropColumn(
                name: "WorldEntityWorldId",
                schema: "Game",
                table: "Spells");

            migrationBuilder.DropColumn(
                name: "WorldEntityWorldId",
                schema: "Game",
                table: "Scripts");

            migrationBuilder.DropColumn(
                name: "WorldEntityWorldId",
                schema: "Game",
                table: "Lineages");

            migrationBuilder.DropColumn(
                name: "WorldEntityWorldId",
                schema: "Game",
                table: "Languages");

            migrationBuilder.DropColumn(
                name: "WorldEntityWorldId",
                schema: "Game",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "WorldEntityWorldId",
                schema: "Game",
                table: "Educations");

            migrationBuilder.DropColumn(
                name: "WorldEntityWorldId",
                schema: "Game",
                table: "Customizations");

            migrationBuilder.DropColumn(
                name: "WorldEntityWorldId",
                schema: "Game",
                table: "Castes");

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                schema: "Game",
                table: "Worlds",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "OwnerId",
                schema: "Game",
                table: "Worlds",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                schema: "Game",
                table: "Worlds",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Castes_Worlds_WorldId",
                schema: "Game",
                table: "Castes",
                column: "WorldId",
                principalSchema: "Game",
                principalTable: "Worlds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Customizations_Worlds_WorldId",
                schema: "Game",
                table: "Customizations",
                column: "WorldId",
                principalSchema: "Game",
                principalTable: "Worlds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Educations_Worlds_WorldId",
                schema: "Game",
                table: "Educations",
                column: "WorldId",
                principalSchema: "Game",
                principalTable: "Worlds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Worlds_WorldId",
                schema: "Game",
                table: "Items",
                column: "WorldId",
                principalSchema: "Game",
                principalTable: "Worlds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Languages_Worlds_WorldId",
                schema: "Game",
                table: "Languages",
                column: "WorldId",
                principalSchema: "Game",
                principalTable: "Worlds",
                principalColumn: "Id",
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

            migrationBuilder.AddForeignKey(
                name: "FK_Scripts_Worlds_WorldId",
                schema: "Game",
                table: "Scripts",
                column: "WorldId",
                principalSchema: "Game",
                principalTable: "Worlds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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
    }
}
