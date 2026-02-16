using FluentValidation;
using SkillCraft.Api.Contracts.Lineages;

namespace SkillCraft.Api.Core.Lineages;

public record Size
{
  public SizeCategory Category { get; }
  public Roll? Height { get; }

  public Size()
  {
  }

  [JsonConstructor]
  public Size(SizeCategory category, Roll? height = null)
  {
    Category = category;
    Height = height;
    new Validator().ValidateAndThrow(this);
  }

  private class Validator : AbstractValidator<Size>
  {
    public Validator()
    {
      RuleFor(x => x.Category).IsInEnum();
    }
  }
}
