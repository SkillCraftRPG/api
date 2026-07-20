using Logitar.Data;
using SkillCraft.Api.Core.Customizations;

namespace SkillCraft.Api.Infrastructure.Db;

public static class Customizations
{
  public static readonly TableId Table = new(Schemas.Game, nameof(GameContext.Customizations), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(Customization.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(Customization.CreatedOn), Table);
  public static readonly ColumnId CustomizationId = new(nameof(Customization.CustomizationId), Table);
  public static readonly ColumnId HtmlContent = new(nameof(Customization.HtmlContent), Table);
  public static readonly ColumnId Id = new(nameof(Customization.Id), Table);
  public static readonly ColumnId Kind = new(nameof(Customization.Kind), Table);
  public static readonly ColumnId Name = new(nameof(Customization.Name), Table);
  public static readonly ColumnId Summary = new(nameof(Customization.Summary), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(Customization.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(Customization.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(Customization.Version), Table);
  public static readonly ColumnId WorldId = new(nameof(Customization.WorldId), Table);
}
