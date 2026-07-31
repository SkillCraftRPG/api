namespace SkillCraft.Api.Core.Features;

public record Feature : IFeature
{
  public string Name { get; }
  public string? Content { get; }

  [JsonConstructor]
  public Feature(string name, string? content = null)
  {
    Name = name;
    Content = content;
  }

  public Feature(IFeature feature) : this(feature.Name, feature.Content)
  {
  }
}
