using Logitar.Data;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.GameDb;

internal static class Languages
{
  public static readonly TableId Table = new(GameContext.Schema, nameof(GameContext.Languages), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(LanguageEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(LanguageEntity.CreatedOn), Table);
  public static readonly ColumnId StreamId = new(nameof(LanguageEntity.StreamId), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(LanguageEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(LanguageEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(LanguageEntity.Version), Table);

  public static readonly ColumnId Description = new(nameof(LanguageEntity.Description), Table);
  public static readonly ColumnId Id = new(nameof(LanguageEntity.Id), Table);
  public static readonly ColumnId LanguageId = new(nameof(LanguageEntity.LanguageId), Table);
  public static readonly ColumnId Name = new(nameof(LanguageEntity.Name), Table);
  public static readonly ColumnId Summary = new(nameof(LanguageEntity.Summary), Table);
  public static readonly ColumnId WorldId = new(nameof(LanguageEntity.WorldId), Table);
  public static readonly ColumnId WorldUid = new(nameof(LanguageEntity.WorldUid), Table);
}
