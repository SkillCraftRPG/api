using Logitar.EventSourcing;
using SkillCraft.Api.Core.Characters;

namespace SkillCraft.Api.Infrastructure.Repositories;

internal class CharacterRepository : Repository, ICharacterRepository
{
  public CharacterRepository(IEventStore eventStore) : base(eventStore)
  {
  }

  public async Task<Character?> LoadAsync(CharacterId id, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Character>(id.StreamId, cancellationToken);
  }

  public async Task<IReadOnlyCollection<Character>> LoadAsync(IEnumerable<CharacterId> ids, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Character>(ids.Select(id => id.StreamId), cancellationToken);
  }

  public async Task SaveAsync(Character character, CancellationToken cancellationToken)
  {
    await base.SaveAsync(character, cancellationToken);
  }

  public async Task SaveAsync(IEnumerable<Character> characters, CancellationToken cancellationToken)
  {
    await base.SaveAsync(characters, cancellationToken);
  }
}
