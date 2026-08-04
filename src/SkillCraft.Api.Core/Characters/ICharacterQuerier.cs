using Krakenar.Contracts.Search;
using SkillCraft.Api.Core.Characters.Models;

namespace SkillCraft.Api.Core.Characters;

public interface ICharacterQuerier
{
  Task<CharacterModel> ReadAsync(Character character, CancellationToken cancellationToken = default);
  Task<CharacterModel?> ReadAsync(CharacterId id, CancellationToken cancellationToken = default);
  Task<CharacterModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);

  Task<SearchResults<CharacterModel>> SearchAsync(SearchCharactersPayload payload, CancellationToken cancellationToken = default);
}
