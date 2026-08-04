using Krakenar.Contracts.Search;

namespace SkillCraft.Api.Core.Characters.Models;

public record SearchCharactersPayload : SearchPayload
{
  public new List<CharacterSortOption> Sort { get; set; } = [];
}
