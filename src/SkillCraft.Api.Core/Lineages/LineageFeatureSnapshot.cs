using SkillCraft.Api.Core.Lineages.Events;

namespace SkillCraft.Api.Core.Lineages;

public class LineageFeatureSnapshot
{
  public string Name { get; }
  public string? Content { get; }

  public LineageFeatureSnapshot(LineageFeature feature)
  {
    Name = feature.Name;
    Content = feature.Content;
  }

  public LineageFeatureUpdated? Compare(LineageFeature feature)
  {
    int changes = 0;
    LineageFeatureUpdated record = new(feature);

    if (Name != feature.Name)
    {
      changes++;
      record.Name = new Change<string>(Name, feature.Name);
    }

    if (Content != feature.Content)
    {
      changes++;
      record.Content = new Change<string>(Content, feature.Content);
    }

    return changes < 1 ? null : record;
  }
}
