namespace SkillCraft.Api.Core;

public record Feature(Name Name, Description? Description)
{
  [JsonIgnore]
  public long Size => Name.Size + (Description?.Size ?? 0);
}
