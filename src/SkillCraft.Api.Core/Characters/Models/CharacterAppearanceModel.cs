namespace SkillCraft.Api.Core.Characters.Models;

public record CharacterAppearanceModel : ICharacterAppearance
{
  public int? Height { get; set; }
  public int? Weight { get; set; }
  public int? Age { get; set; }

  public string? Skin { get; set; }
  public string? Eyes { get; set; }
  public string? Hair { get; set; }
}
