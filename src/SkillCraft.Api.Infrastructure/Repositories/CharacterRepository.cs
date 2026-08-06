using Logitar.EventSourcing;
using SkillCraft.Api.Core.Characters;

namespace SkillCraft.Api.Infrastructure.Repositories;

internal class CharacterRepository : Repository, ICharacterRepository
{
  private readonly GameContext _database;

  public CharacterRepository(GameContext database, IEventStore eventStore) : base(eventStore)
  {
    _database = database;
  }

  public async Task<Character?> LoadAsync(CharacterId id, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Character>(id.StreamId, isDeleted: false, cancellationToken);
  }
  public async Task<IReadOnlyCollection<Character>> LoadAsync(IEnumerable<CharacterId> ids, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Character>(ids.Select(id => id.StreamId), isDeleted: false, cancellationToken);
  }

  public async Task SaveAsync(Character character, CancellationToken cancellationToken)
  {
    await base.SaveAsync(character, cancellationToken);

    await SynchronizeAsync(character, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<Character> characters, CancellationToken cancellationToken)
  {
    await base.SaveAsync(characters, cancellationToken);

    foreach (Character character in characters)
    {
      await SynchronizeAsync(character, cancellationToken);
    }
  }

  private async Task SynchronizeAsync(Character character, CancellationToken cancellationToken)
  {
    // TOOD(fpion): implement
  }
}
