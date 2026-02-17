namespace SkillCraft.Api.Contracts.Lineages;

public record LanguagesPayload
{
  public List<Guid> Ids { get; set; }
  public int Extra { get; set; }
  public string? Text { get; set; }

  public LanguagesPayload(IEnumerable<Guid>? ids = null, int extra = 0, string? text = null)
  {
    Ids = ids?.ToList() ?? [];
    Extra = extra;
    Text = text;
  }
}
