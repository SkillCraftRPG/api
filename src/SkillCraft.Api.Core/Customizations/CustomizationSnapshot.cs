using SkillCraft.Api.Core.Customizations.Events;

namespace SkillCraft.Api.Core.Customizations;

public record CustomizationSnapshot
{
  public string Name { get; }
  public string? Summary { get; }
  public string? Content { get; }

  public CustomizationSnapshot(Customization customization)
  {
    Name = customization.Name;
    Summary = customization.Summary;
    Content = customization.Content;
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

    if (Content != customization.Content)
    {
      changes++;
      record.Content = new Change<string>(Content, customization.Content);
    }

    return changes < 1 ? null : record;
  }
}
