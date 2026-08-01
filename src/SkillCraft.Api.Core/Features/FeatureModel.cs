using FluentValidation;

namespace SkillCraft.Api.Core.Features;

public record FeatureModel : IFeature
{
  public string Name { get; set; }
  public string? Content { get; set; }

  public FeatureModel() : this(string.Empty)
  {
  }

  public FeatureModel(string name, string? content = null)
  {
    Name = name;
    Content = content;
  }

  public FeatureModel(IFeature feature) : this(feature.Name, feature.Content)
  {
  }

  public void Validate() => new FeatureValidator().ValidateAndThrow(this);
}
