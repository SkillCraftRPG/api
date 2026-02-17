namespace SkillCraft.Api.Contracts.Lineages;

public record NameCategory
{
  public string Category { get; set; }
  public List<string> Names { get; set; } = [];

  public NameCategory() : this(string.Empty)
  {
  }

  public NameCategory(string category, IEnumerable<string>? names = null)
  {
    Category = category;
    if (names is not null)
    {
      Names.AddRange(names);
    }
  }
}
