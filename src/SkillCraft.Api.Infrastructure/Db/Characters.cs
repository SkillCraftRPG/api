using Logitar.Data;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Db;

public static class Characters
{
  public static readonly TableId Table = new(Schemas.Game, nameof(GameContext.Characters), alias: null);

  public static readonly ColumnId Age = new(nameof(CharacterEntity.Age), Table);
  public static readonly ColumnId Alignment = new(nameof(CharacterEntity.Alignment), Table);
  public static readonly ColumnId Attributes = new(nameof(CharacterEntity.Attributes), Table);
  public static readonly ColumnId Background = new(nameof(CharacterEntity.Background), Table);
  public static readonly ColumnId CasteId = new(nameof(CharacterEntity.CasteId), Table);
  public static readonly ColumnId CharacterId = new(nameof(CharacterEntity.CharacterId), Table);
  public static readonly ColumnId CreatedBy = new(nameof(CharacterEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(CharacterEntity.CreatedOn), Table);
  public static readonly ColumnId DominantHand = new(nameof(CharacterEntity.DominantHand), Table);
  public static readonly ColumnId EducationId = new(nameof(CharacterEntity.EducationId), Table);
  public static readonly ColumnId Eyes = new(nameof(CharacterEntity.Eyes), Table);
  public static readonly ColumnId Flaws = new(nameof(CharacterEntity.Flaws), Table);
  public static readonly ColumnId Hair = new(nameof(CharacterEntity.Hair), Table);
  public static readonly ColumnId Height = new(nameof(CharacterEntity.Height), Table);
  public static readonly ColumnId Id = new(nameof(CharacterEntity.Id), Table);
  public static readonly ColumnId Ideals = new(nameof(CharacterEntity.Ideals), Table);
  public static readonly ColumnId LineageId = new(nameof(CharacterEntity.LineageId), Table);
  public static readonly ColumnId Name = new(nameof(CharacterEntity.Name), Table);
  public static readonly ColumnId Skills = new(nameof(CharacterEntity.Skills), Table);
  public static readonly ColumnId Skin = new(nameof(CharacterEntity.Skin), Table);
  public static readonly ColumnId Traits = new(nameof(CharacterEntity.Traits), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(CharacterEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(CharacterEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(CharacterEntity.Version), Table);
  public static readonly ColumnId Weight = new(nameof(CharacterEntity.Weight), Table);
  public static readonly ColumnId WorldId = new(nameof(CharacterEntity.WorldId), Table);
}
