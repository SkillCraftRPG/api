using Logitar.Data;
using SkillCraft.Api.Core.Talents;

namespace SkillCraft.Api.Infrastructure.Db;

public static class Talents
{
  public static readonly TableId Table = new(Schemas.Game, nameof(GameContext.Talents), alias: null);

  public static readonly ColumnId AllowMultiplePurchases = new(nameof(Talent.AllowMultiplePurchases), Table);
  public static readonly ColumnId CreatedBy = new(nameof(Talent.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(Talent.CreatedOn), Table);
  public static readonly ColumnId HtmlContent = new(nameof(Talent.HtmlContent), Table);
  public static readonly ColumnId Id = new(nameof(Talent.Id), Table);
  public static readonly ColumnId Name = new(nameof(Talent.Name), Table);
  public static readonly ColumnId RequiredTalentId = new(nameof(Talent.RequiredTalentId), Table);
  public static readonly ColumnId Skill = new(nameof(Talent.Skill), Table);
  public static readonly ColumnId Summary = new(nameof(Talent.Summary), Table);
  public static readonly ColumnId TalentId = new(nameof(Talent.TalentId), Table);
  public static readonly ColumnId Tier = new(nameof(Talent.Tier), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(Talent.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(Talent.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(Talent.Version), Table);
  public static readonly ColumnId WorldId = new(nameof(Talent.WorldId), Table);
}
