using Logitar;

namespace SkillCraft.Api.Core.Features;

public record FeatureOld : IFeature
{
  public string Name { get; }
  public string? Content { get; }

  [JsonConstructor]
  public FeatureOld(string name, string? content = null)
  {
    Name = name.Trim();
    Content = content?.CleanTrim();
  }

  public FeatureOld(IFeature feature) : this(feature.Name, feature.Content)
  {
  }
}
