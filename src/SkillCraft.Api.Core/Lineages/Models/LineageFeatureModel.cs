using Krakenar.Contracts.Actors;
using SkillCraft.Api.Core.Features;

namespace SkillCraft.Api.Core.Lineages.Models;

public class LineageFeatureModel : IFeature
{
  public Guid Id { get; set; }

  public string Name { get; set; } = string.Empty;
  public string? HtmlContent { get; set; }

  public Actor CreatedBy { get; set; } = new();
  public DateTime CreatedOn { get; set; }
  public Actor UpdatedBy { get; set; } = new();
  public DateTime UpdatedOn { get; set; }

  public override bool Equals(object? obj) => obj is LineageFeatureModel feature && feature.Id == Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString() => $"{Name} | {base.ToString()} (Id={Id})";
}
