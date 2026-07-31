namespace SkillCraft.Api.Core.Features;

[method: JsonConstructor]
public record Feature(string Name, string? Content = null) : IFeature
{
  public Feature(IFeature feature) : this(feature.Name, feature.Content)
  {
  }
}
