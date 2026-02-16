namespace SkillCraft.Api.Contracts.Lineages;

public record SizeModel
{
  public SizeCategory Category { get; set; }
  public string? Height { get; set; }

  public SizeModel() : this(SizeCategory.Medium)
  {
  }

  public SizeModel(SizeCategory category, string? height = null)
  {
    Category = category;
    Height = height;
  }
}
