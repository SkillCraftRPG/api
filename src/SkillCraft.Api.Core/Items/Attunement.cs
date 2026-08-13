using FluentValidation;
using Logitar;

namespace SkillCraft.Api.Core.Items;

public interface IAttunement
{
  bool IsRequired { get; }
  string? Requirements { get; }
}

public record Attunement : IAttunement
{
  public const int MaximumLength = 100;

  public bool IsRequired { get; }
  public string? Requirements { get; }

  public Attunement() : this(isRequired: false)
  {
  }

  [JsonConstructor]
  public Attunement(bool isRequired, string? requirements = null)
  {
    IsRequired = isRequired;
    Requirements = requirements?.CleanTrim();
    new AttunementValidator().ValidateAndThrow(this);
  }

  public Attunement(IAttunement attunement) : this(attunement.IsRequired, attunement.Requirements)
  {
  }
}

internal class AttunementValidator : AbstractValidator<IAttunement>
{
  public AttunementValidator()
  {
    When(x => !x.IsRequired, () => RuleFor(x => x.Requirements).Empty());
    When(x => !string.IsNullOrWhiteSpace(x.Requirements), () => RuleFor(x => x.Requirements).NotEmpty().MaximumLength(Attunement.MaximumLength));
  }
}
