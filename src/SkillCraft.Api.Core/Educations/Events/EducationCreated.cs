using SkillCraft.Api.Core.Features;

namespace SkillCraft.Api.Core.Educations.Events;

public class EducationCreated : CreateEvent
{
  public string Name { get; set; } = string.Empty;
  public string? Summary { get; set; }
  public string? HtmlContent { get; set; }

  public Skill? Skill { get; set; }
  public int? WealthMultiplier { get; set; }
  public Feature? Feature { get; set; }

  public EducationCreated() : base()
  {
  }

  public EducationCreated(Education education) : base(education)
  {
    Name = education.Name;
    Summary = education.Summary;
    HtmlContent = education.HtmlContent;

    Skill = education.Skill;
    WealthMultiplier = education.WealthMultiplier;
    if (education.FeatureName is not null)
    {
      Feature = new Feature(education.FeatureName, education.FeatureHtmlContent);
    }
  }
}
