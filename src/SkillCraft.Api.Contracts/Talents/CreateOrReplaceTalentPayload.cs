namespace SkillCraft.Api.Contracts.Talents;

public record CreateOrReplaceTalentPayload
{
  public int Tier { get; set; }

  public string Name { get; set; } = string.Empty;
  public string? Summary { get; set; }
  public string? Description { get; set; }

  public bool AllowMultiplePurchases { get; set; }
  public GameSkill? Skill { get; set; }
  public Guid? RequiredTalentId { get; set; }
}
