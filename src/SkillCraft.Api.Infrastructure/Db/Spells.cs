using Logitar.Data;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Db;

public static class Spells
{
  public static readonly TableId Table = new(Schemas.Game, nameof(GameContext.Spells), alias: null);

  public static readonly ColumnId Content = new(nameof(SpellEntity.Content), Table);
  public static readonly ColumnId CreatedBy = new(nameof(SpellEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(SpellEntity.CreatedOn), Table);
  public static readonly ColumnId Id = new(nameof(SpellEntity.Id), Table);
  public static readonly ColumnId Name = new(nameof(SpellEntity.Name), Table);
  public static readonly ColumnId SpellId = new(nameof(SpellEntity.SpellId), Table);
  public static readonly ColumnId Summary = new(nameof(SpellEntity.Summary), Table);
  public static readonly ColumnId Tier = new(nameof(SpellEntity.Tier), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(SpellEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(SpellEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(SpellEntity.Version), Table);
  public static readonly ColumnId WorldId = new(nameof(SpellEntity.WorldId), Table);
}
