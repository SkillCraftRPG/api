using SkillCraft.Api.Core.Features;

namespace SkillCraft.Api.Core.Castes.Events;

public class CasteCreated : CreateEvent
{
  public string Name { get; set; } = string.Empty;
  public string? Summary { get; set; }
  public string? HtmlContent { get; set; }

  public Skill? Skill { get; set; }
  public string? WealthRoll { get; set; }
  public Feature? Feature { get; set; }

  public CasteCreated() : base()
  {
  }

  public CasteCreated(Caste caste) : base(caste)
  {
    Name = caste.Name;
    Summary = caste.Summary;
    HtmlContent = caste.HtmlContent;

    Skill = caste.Skill;
    WealthRoll = caste.WealthRoll;
    if (caste.FeatureName is not null)
    {
      Feature = new Feature(caste.FeatureName, caste.FeatureHtmlContent);
    }
  }
}
