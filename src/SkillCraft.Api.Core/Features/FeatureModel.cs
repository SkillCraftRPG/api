namespace SkillCraft.Api.Core.Features;

public record FeatureModel : IFeature
{
  public string Name { get; set; }
  public string? HtmlContent { get; set; }

  public FeatureModel() : this(string.Empty)
  {
  }

  public FeatureModel(string name, string? htmlContent = null)
  {
    Name = name;
    HtmlContent = htmlContent;
  }

  public FeatureModel(IFeature feature) : this(feature.Name, feature.HtmlContent)
  {
  }
}
