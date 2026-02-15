namespace SkillCraft.Api.Contracts.Castes;

public record UpdateCastePayload
{
  public string? Name { get; set; }
  public Update<string>? Summary { get; set; }
  public Update<string>? Description { get; set; }

  public Update<GameSkill?>? Skill { get; set; }
  public Update<string>? WealthRoll { get; set; }
}
