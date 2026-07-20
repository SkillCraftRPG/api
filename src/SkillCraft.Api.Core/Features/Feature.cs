using FluentValidation;
using Logitar;

namespace SkillCraft.Api.Core.Features;

public interface IFeature
{
  string Name { get; }
  string? HtmlContent { get; }
}

public record Feature : IFeature
{
  public string Name { get; }
  public string? HtmlContent { get; }

  public Feature(string name, string? htmlContent = null)
  {
    Name = name.Trim();
    HtmlContent = htmlContent?.CleanTrim();
    new FeatureValidator().ValidateAndThrow(this);
  }

  public Feature(IFeature feature) : this(feature.Name, feature.HtmlContent)
  {
  }
}
