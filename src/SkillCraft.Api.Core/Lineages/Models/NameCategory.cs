using FluentValidation;
using SkillCraft.Api.Core.Validation;

namespace SkillCraft.Api.Core.Lineages.Models;

public class NameCategory
{
  public string Category { get; set; } = string.Empty;
  public List<string> Values { get; set; } = [];

  public NameCategory() : this(string.Empty)
  {
  }

  public NameCategory(string category, IEnumerable<string>? values = null)
  {
    Category = category;
    if (values is not null)
    {
      Values.AddRange(values);
    }
  }

  public override bool Equals(object? obj) => obj is NameCategory category && category.Category == Category;
  public override int GetHashCode() => Category.GetHashCode();
  public override string ToString() => Category;
}

internal class NameCategoryValidator : AbstractValidator<NameCategory>
{
  public NameCategoryValidator()
  {
    RuleFor(x => x.Category).Name();
    RuleForEach(x => x.Values).Name();
  }
}
