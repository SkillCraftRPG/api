namespace SkillCraft.Api.Contracts.Talents;

public record UpdateTalentPayload
{
  public string? Name { get; set; }
  public Update<string>? Summary { get; set; }
  public Update<string>? Description { get; set; }

  public bool? AllowMultiplePurchases { get; set; }
  public Update<GameSkill?>? Skill { get; set; }
  public Update<Guid?>? RequiredTalentId { get; set; }
}
