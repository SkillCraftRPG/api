namespace SkillCraft.Api.Contracts.Specializations;

public record DoctrinePayload
{
  public string Name { get; set; } = string.Empty;
  public List<string> Description { get; set; } = [];
  public List<Guid> DiscountedTalentIds { get; set; } = [];
  public List<FeatureModel> Features { get; set; } = [];
}
