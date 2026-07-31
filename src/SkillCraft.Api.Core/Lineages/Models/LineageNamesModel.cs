using FluentValidation;
using SkillCraft.Api.Core.Validation;

namespace SkillCraft.Api.Core.Lineages.Models;

public record LineageNamesModel
{
  public List<string> Family { get; set; } = [];
  public List<string> Female { get; set; } = [];
  public List<string> Male { get; set; } = [];
  public List<string> Unisex { get; set; } = [];
  public List<NameCategory> Custom { get; set; } = [];
  public string? Content { get; set; }
}

internal class LineageNamesValidator : AbstractValidator<LineageNamesModel>
{
  public LineageNamesValidator()
  {
    RuleForEach(x => x.Custom).SetValidator(new NameCategoryValidator());
    When(x => !string.IsNullOrWhiteSpace(x.Content), () => RuleFor(x => x.Content!).Content());
  }
}
