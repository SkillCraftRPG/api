using Logitar.Data;
using SkillCraft.Api.Core.Spells;

namespace SkillCraft.Api.Infrastructure.Db;

public static class Spells
{
  public static readonly TableId Table = new(Schemas.Game, nameof(GameContext.Spells), alias: null);

  public static readonly ColumnId Content = new(nameof(Spell.Content), Table);
  public static readonly ColumnId CreatedBy = new(nameof(Spell.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(Spell.CreatedOn), Table);
  public static readonly ColumnId Id = new(nameof(Spell.Id), Table);
  public static readonly ColumnId Name = new(nameof(Spell.Name), Table);
  public static readonly ColumnId SpellId = new(nameof(Spell.SpellId), Table);
  public static readonly ColumnId Summary = new(nameof(Spell.Summary), Table);
  public static readonly ColumnId Tier = new(nameof(Spell.Tier), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(Spell.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(Spell.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(Spell.Version), Table);
  public static readonly ColumnId WorldId = new(nameof(Spell.WorldId), Table);
}
