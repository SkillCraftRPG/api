namespace SkillCraft.Api.Contracts.Educations;

public record UpdateEducationPayload
{
  public string? Name { get; set; }
  public Update<string>? Summary { get; set; }
  public Update<string>? Description { get; set; }

  public Update<GameSkill?>? Skill { get; set; }
  public Update<int?>? WealthMultiplier { get; set; }
  public Update<FeatureModel>? Feature { get; set; }
}
