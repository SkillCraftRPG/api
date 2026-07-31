using SkillCraft.Api.Core.Educations.Events;
using SkillCraft.Api.Core.Features;

namespace SkillCraft.Api.Core.Educations;

public record EducationSnapshot
{
  public string Name { get; }
  public string? Summary { get; }
  public string? HtmlContent { get; }

  public Skill? Skill { get; }
  public int? WealthMultiplier { get; }
  public Feature? Feature { get; }

  public EducationSnapshot(Education education)
  {
    Name = education.Name;
    Summary = education.Summary;
    HtmlContent = education.HtmlContent;

    Skill = education.Skill;
    WealthMultiplier = education.WealthMultiplier;
    Feature = education.FeatureName is null ? null : new Feature(education.FeatureName, education.FeatureHtmlContent);
  }

  public EducationUpdated? Compare(Education education)
  {
    int changes = 0;
    EducationUpdated record = new(education);

    if (Name != education.Name)
    {
      changes++;
      record.Name = new Change<string>(Name, education.Name);
    }

    if (Summary != education.Summary)
    {
      changes++;
      record.Summary = new Change<string>(Summary, education.Summary);
    }

    if (HtmlContent != education.HtmlContent)
    {
      changes++;
      record.HtmlContent = new Change<string>(HtmlContent, education.HtmlContent);
    }

    if (Skill != education.Skill)
    {
      changes++;
      record.Skill = new Change<Skill?>(Skill, education.Skill);
    }

    if (WealthMultiplier != education.WealthMultiplier)
    {
      changes++;
      record.WealthMultiplier = new Change<int?>(WealthMultiplier, education.WealthMultiplier);
    }

    Feature? feature = education.FeatureName is null ? null : new Feature(education.FeatureName, education.FeatureHtmlContent);
    if (Feature != feature)
    {
      changes++;
      record.Feature = new Change<Feature>(Feature, feature);
    }

    return changes < 1 ? null : record;
  }
}
