using FluentValidation;
using SkillCraft.Api.Core.Validation;

namespace SkillCraft.Api.Core.Lineages.Models;

public record CreateOrReplaceLineagePayload
{
  public Guid? ParentId { get; set; }

  public string Name { get; set; } = string.Empty;
  public string? Summary { get; set; }
  public string? HtmlContent { get; set; }

  public LineageLanguagesPayload Languages { get; set; } = new();
  public LineageNamesModel Names { get; set; } = new();
  public LineageSpeedsModel Speeds { get; set; } = new();
  public LineageSizeModel Size { get; set; } = new();
  public LineageWeightModel Weight { get; set; } = new();
  public LineageAgeModel Age { get; set; } = new();

  public void Validate() => new Validator().ValidateAndThrow(this);

  private class Validator : AbstractValidator<CreateOrReplaceLineagePayload>
  {
    public Validator()
    {
      RuleFor(x => x.Name).Name();
      When(x => !string.IsNullOrWhiteSpace(x.Summary), () => RuleFor(x => x.Summary!).Summary());
      When(x => !string.IsNullOrWhiteSpace(x.HtmlContent), () => RuleFor(x => x.HtmlContent!).HtmlContent());

      // TODO(fpion): Languages
      // TODO(fpion): Names
      // TODO(fpion): Speeds
      // TODO(fpion): Size
      // TODO(fpion): Weight
      // TODO(fpion): Age
    }
  }
}
