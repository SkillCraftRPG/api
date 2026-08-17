using Logitar.Data;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Db;

public static class CharacterModifiers
{
  public static readonly TableId Table = new(Schemas.Game, nameof(GameContext.CharacterModifiers), alias: null);

  public static readonly ColumnId CharacterId = new(nameof(CharacterModifierEntity.CharacterId), Table);
  public static readonly ColumnId CharacterModifierId = new(nameof(CharacterModifierEntity.CharacterModifierId), Table);
  public static readonly ColumnId CreatedBy = new(nameof(CharacterModifierEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(CharacterModifierEntity.CreatedOn), Table);
  public static readonly ColumnId Id = new(nameof(CharacterModifierEntity.Id), Table);
  public static readonly ColumnId Kind = new(nameof(CharacterModifierEntity.Kind), Table);
  public static readonly ColumnId Name = new(nameof(CharacterModifierEntity.Name), Table);
  public static readonly ColumnId Notes = new(nameof(CharacterModifierEntity.Notes), Table);
  public static readonly ColumnId Target = new(nameof(CharacterModifierEntity.Target), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(CharacterModifierEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(CharacterModifierEntity.UpdatedOn), Table);
  public static readonly ColumnId Value = new(nameof(CharacterModifierEntity.Value), Table);
}
