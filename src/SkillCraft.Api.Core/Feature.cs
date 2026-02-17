namespace SkillCraft.Api.Core;

public record Feature(Name Name, Description? Description)
{
  [JsonIgnore]
  public long Size => Name.Size + (Description?.Size ?? 0);

  public static Feature Create(string name, string? description = null) => new(new Name(name), Description.TryCreate(description));
}
