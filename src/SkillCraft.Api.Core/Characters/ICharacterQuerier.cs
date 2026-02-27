using SkillCraft.Api.Contracts.Characters;

namespace SkillCraft.Api.Core.Characters;

public interface ICharacterQuerier
{
  Task<CharacterModel> ReadAsync(Character character, CancellationToken cancellationToken = default);
  Task<CharacterModel?> ReadAsync(CharacterId id, CancellationToken cancellationToken = default);
  Task<CharacterModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
}
