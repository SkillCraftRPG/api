using FluentValidation;
using Logitar;

namespace SkillCraft.Api.Core.Features;

public interface IFeature
{
  string Name { get; }
  string? Content { get; }
}

public record Feature : IFeature
{
  public string Name { get; }
  public string? Content { get; }

  public Feature(string name, string? content = null)
  {
    Name = name.Trim();
    Content = content?.CleanTrim();
    new FeatureValidator().ValidateAndThrow(this);
  }

  public Feature(IFeature feature) : this(feature.Name, feature.Content)
  {
  }
}
