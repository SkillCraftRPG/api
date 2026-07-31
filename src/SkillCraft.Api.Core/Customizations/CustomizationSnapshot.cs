using SkillCraft.Api.Core.Customizations.Events;

namespace SkillCraft.Api.Core.Customizations;

public record CustomizationSnapshot
{
  public string Name { get; }
  public string? Summary { get; }
  public string? HtmlContent { get; }

  public CustomizationSnapshot(Customization customization)
  {
    Name = customization.Name;
    Summary = customization.Summary;
    HtmlContent = customization.HtmlContent;
  }

  public CustomizationUpdated? Compare(Customization customization)
  {
    int changes = 0;
    CustomizationUpdated record = new(customization);

    if (Name != customization.Name)
    {
      changes++;
      record.Name = new Change<string>(Name, customization.Name);
    }

    if (Summary != customization.Summary)
    {
      changes++;
      record.Summary = new Change<string>(Summary, customization.Summary);
    }

    if (HtmlContent != customization.HtmlContent)
    {
      changes++;
      record.HtmlContent = new Change<string>(HtmlContent, customization.HtmlContent);
    }

    return changes < 1 ? null : record;
  }
}
