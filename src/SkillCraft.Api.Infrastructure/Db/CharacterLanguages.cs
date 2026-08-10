using Logitar.Data;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Db;

public static class CharacterLanguages
{
  public static readonly TableId Table = new(Schemas.Game, nameof(GameContext.CharacterLanguages), alias: null);

  public static readonly ColumnId CharacterId = new(nameof(CharacterLanguageEntity.CharacterId), Table);
  public static readonly ColumnId LanguageId = new(nameof(CharacterLanguageEntity.LanguageId), Table);
}
