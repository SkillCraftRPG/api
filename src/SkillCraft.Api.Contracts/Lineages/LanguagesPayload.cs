namespace SkillCraft.Api.Contracts.Lineages;

public record LanguagesPayload
{
  public List<Guid> Ids { get; set; } = [];
  public int Extra { get; set; }
  public string? Text { get; set; }
}
