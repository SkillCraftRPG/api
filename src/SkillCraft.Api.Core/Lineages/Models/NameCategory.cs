namespace SkillCraft.Api.Core.Lineages.Models;

public class NameCategory
{
  public string Category { get; set; } = string.Empty;
  public List<string> Values { get; set; } = [];

  public override bool Equals(object? obj) => obj is NameCategory category && category.Category == Category;
  public override int GetHashCode() => Category.GetHashCode();
  public override string ToString() => Category;
}
