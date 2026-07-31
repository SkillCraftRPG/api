using SkillCraft.Api.Core.Castes.Events;
using SkillCraft.Api.Core.Features;

namespace SkillCraft.Api.Core.Castes;

public record CasteSnapshot
{
  public string Name { get; }
  public string? Summary { get; }
  public string? HtmlContent { get; }

  public Skill? Skill { get; }
  public string? WealthRoll { get; }
  public Feature? Feature { get; }

  public CasteSnapshot(Caste caste)
  {
    Name = caste.Name;
    Summary = caste.Summary;
    HtmlContent = caste.HtmlContent;

    Skill = caste.Skill;
    WealthRoll = caste.WealthRoll;
    Feature = caste.FeatureName is null ? null : new Feature(caste.FeatureName, caste.FeatureHtmlContent);
  }

  public CasteUpdated? Compare(Caste caste)
  {
    int changes = 0;
    CasteUpdated record = new(caste);

    if (Name != caste.Name)
    {
      changes++;
      record.Name = new Change<string>(Name, caste.Name);
    }

    if (Summary != caste.Summary)
    {
      changes++;
      record.Summary = new Change<string>(Summary, caste.Summary);
    }

    if (HtmlContent != caste.HtmlContent)
    {
      changes++;
      record.HtmlContent = new Change<string>(HtmlContent, caste.HtmlContent);
    }

    if (Skill != caste.Skill)
    {
      changes++;
      record.Skill = new Change<Skill?>(Skill, caste.Skill);
    }

    if (WealthRoll != caste.WealthRoll)
    {
      changes++;
      record.WealthRoll = new Change<string>(WealthRoll, caste.WealthRoll);
    }

    Feature? feature = caste.FeatureName is null ? null : new Feature(caste.FeatureName, caste.FeatureHtmlContent);
    if (Feature != feature)
    {
      changes++;
      record.Feature = new Change<Feature>(Feature, feature);
    }

    return changes < 1 ? null : record;
  }
}
