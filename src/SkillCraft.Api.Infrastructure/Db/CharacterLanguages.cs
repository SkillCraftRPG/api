using Logitar.Data;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Db;

public static class CharacterLanguages
{
  public static readonly TableId Table = new(Schemas.Game, nameof(GameContext.CharacterLanguages), alias: null);

  public static readonly ColumnId CharacterId = new(nameof(CharacterLanguageEntity.CharacterId), Table);
  public static readonly ColumnId CreatedBy = new(nameof(CharacterLanguageEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(CharacterLanguageEntity.CreatedOn), Table);
  public static readonly ColumnId LanguageId = new(nameof(CharacterLanguageEntity.LanguageId), Table);
  public static readonly ColumnId Notes = new(nameof(CharacterLanguageEntity.Notes), Table);
  public static readonly ColumnId Source = new(nameof(CharacterLanguageEntity.Source), Table);
  public static readonly ColumnId Target = new(nameof(CharacterLanguageEntity.Target), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(CharacterLanguageEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(CharacterLanguageEntity.UpdatedOn), Table);
}
