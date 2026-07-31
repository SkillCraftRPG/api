namespace SkillCraft.Api.Core.Lineages.Models;

public record LineageSizeModel : ILineageSize
{
  public SizeCategory Category { get; set; }
  public string? Height { get; set; }
}
