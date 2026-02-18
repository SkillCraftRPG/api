using FluentValidation;
using SkillCraft.Api.Contracts.Specializations;
using SkillCraft.Api.Core.Validators;

namespace SkillCraft.Api.Core.Specializations.Validators;

internal class DoctrineValidator : AbstractValidator<DoctrinePayload>
{
  public DoctrineValidator()
  {
    RuleFor(x => x.Name).Name();
    RuleForEach(x => x.Features).SetValidator(new FeatureValidator());
  }
}
