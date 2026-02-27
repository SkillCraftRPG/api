namespace SkillCraft.Api.Contracts.Characters;

public interface ICharacterService
{
  Task<CharacterModel> CreateOrReplaceAsync(CreateCharacterPayload payload, CancellationToken cancellationToken = default);
  Task<CharacterModel?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<CharacterModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
}
