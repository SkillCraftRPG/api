namespace SkillCraft.Api.Core.Characters;

public interface ICharacterRepository
{
  Task<Character?> LoadAsync(CharacterId id, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<Character>> LoadAsync(IEnumerable<CharacterId> ids, CancellationToken cancellationToken = default);

  Task SaveAsync(Character character, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Character> characters, CancellationToken cancellationToken = default);
}
