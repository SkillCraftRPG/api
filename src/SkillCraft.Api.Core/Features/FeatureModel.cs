using FluentValidation;
using SkillCraft.Api.Core.Validation;

namespace SkillCraft.Api.Core.Features;

public record FeatureModel
{
  public string Name { get; set; }
  public string? Content { get; set; }

  public FeatureModel() : this(string.Empty)
  {
  }

  public FeatureModel(string name, string? content = null)
  {
    Name = name;
    Content = content;
  }
}

internal class FeatureValidator : AbstractValidator<FeatureModel>
{
  public FeatureValidator()
  {
    RuleFor(x => x.Name).Name();
    When(x => !string.IsNullOrWhiteSpace(x.Content), () => RuleFor(x => x.Content!).Content());
  }
}
