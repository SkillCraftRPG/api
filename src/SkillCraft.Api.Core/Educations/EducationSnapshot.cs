using SkillCraft.Api.Core.Educations.Events;
using SkillCraft.Api.Core.Features;

namespace SkillCraft.Api.Core.Educations;

public record EducationSnapshot
{
  public string Name { get; }
  public string? Summary { get; }
  public string? Content { get; }

  public Skill? Skill { get; }
  public int? WealthMultiplier { get; }
  public Feature? Feature { get; }

  public EducationSnapshot(Education education)
  {
    Name = education.Name;
    Summary = education.Summary;
    Content = education.Content;

    Skill = education.Skill;
    WealthMultiplier = education.WealthMultiplier;
    Feature = education.FeatureName is null ? null : new Feature(education.FeatureName, education.FeatureContent);
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

    if (Content != education.Content)
    {
      changes++;
      record.Content = new Change<string>(Content, education.Content);
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

    Feature? feature = education.FeatureName is null ? null : new Feature(education.FeatureName, education.FeatureContent);
    if (Feature != feature)
    {
      changes++;
      record.Feature = new Change<Feature>(Feature, feature);
    }

    return changes < 1 ? null : record;
  }
}
