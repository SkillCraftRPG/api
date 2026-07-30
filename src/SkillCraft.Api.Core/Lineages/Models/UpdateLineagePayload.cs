using FluentValidation;
using SkillCraft.Api.Core.Validation;

namespace SkillCraft.Api.Core.Lineages.Models;

public record UpdateLineagePayload
{

  public string? Name { get; set; }
  public Optional<string>? Summary { get; set; }
  public Optional<string>? HtmlContent { get; set; }

  public LineageLanguagesPayload? Languages { get; set; }
  public LineageNamesModel? Names { get; set; }
  public LineageSpeedsModel? Speeds { get; set; }
  public LineageSizeModel? Size { get; set; }
  public LineageWeightModel? Weight { get; set; }
  public LineageAgeModel? Age { get; set; }

  public void Validate() => new Validator().ValidateAndThrow(this);

  private class Validator : AbstractValidator<UpdateLineagePayload>
  {
    public Validator()
    {
      When(x => !string.IsNullOrWhiteSpace(x.Name), () => RuleFor(x => x.Name!).Name());
      When(x => !string.IsNullOrWhiteSpace(x.Summary?.Value), () => RuleFor(x => x.Summary!.Value!).Summary());
      When(x => !string.IsNullOrWhiteSpace(x.HtmlContent?.Value), () => RuleFor(x => x.HtmlContent!.Value!).HtmlContent());

      // TODO(fpion): Languages
      // TODO(fpion): Names
      // TODO(fpion): Speeds
      // TODO(fpion): Size
      // TODO(fpion): Weight
      // TODO(fpion): Age
    }
  }
}
