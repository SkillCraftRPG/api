namespace SkillCraft.Api.Contracts.Castes;

public record CreateOrReplaceCastePayload
{
  public string Name { get; set; } = string.Empty;
  public string? Summary { get; set; }
  public string? Description { get; set; }

  public GameSkill? Skill { get; set; }
  public string? WealthRoll { get; set; }
}
