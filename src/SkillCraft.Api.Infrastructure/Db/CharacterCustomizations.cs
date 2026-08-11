using Logitar.Data;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Db;

public static class CharacterCustomizations
{
  public static readonly TableId Table = new(Schemas.Game, nameof(GameContext.CharacterCustomizations), alias: null);

  public static readonly ColumnId CharacterId = new(nameof(CharacterCustomizationEntity.CharacterId), Table);
  public static readonly ColumnId CustomizationId = new(nameof(CharacterCustomizationEntity.CustomizationId), Table);
}
