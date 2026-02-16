namespace SkillCraft.Api.Contracts.Lineages;

public record SizeModel
{
  public SizeCategory Category { get; set; }
  public string Height { get; set; }

  public SizeModel() : this(SizeCategory.Medium, string.Empty)
  {
  }

  public SizeModel(SizeCategory category, string height)
  {
    Category = category;
    Height = height;
  }
}
