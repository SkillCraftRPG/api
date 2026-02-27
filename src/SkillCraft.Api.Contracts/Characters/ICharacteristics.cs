namespace SkillCraft.Api.Contracts.Characters;

public interface ICharacteristics
{
  int? Height { get; }
  int? Weight { get; }
  int? Age { get; }
  string? Skin { get; }
  string? Eyes { get; }
  string? Hair { get; }
  Handedness? Handedness { get; }
}
