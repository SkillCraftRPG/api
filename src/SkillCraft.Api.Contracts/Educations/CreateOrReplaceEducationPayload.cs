namespace SkillCraft.Api.Contracts.Educations;

public record CreateOrReplaceEducationPayload
{
  public string Name { get; set; } = string.Empty;
  public string? Summary { get; set; }
  public string? Description { get; set; }

  public GameSkill? Skill { get; set; }
  public int? WealthMultiplier { get; set; }
  public FeatureModel? Feature { get; set; }
}
