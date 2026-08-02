using FluentValidation;

namespace SkillCraft.Api.Core.Lineages;

public record LineageSize
{
  public SizeCategory Category { get; }
  public Roll? Height { get; }

  public LineageSize(SizeCategory category = default, Roll? height = null)
  {
    Category = category;
    Height = height;
    new Validator().ValidateAndThrow(this);
  }

  private class Validator : AbstractValidator<LineageSize>
  {
    public Validator()
    {
      RuleFor(x => x.Category).IsInEnum();
    }
  }
}
