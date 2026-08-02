using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Api.Core.Spells;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Repositories;

internal class SpellRepository : Logitar.EventSourcing.Repository, ISpellRepository
{
  private readonly GameContext _database;

  public SpellRepository(GameContext database, IEventStore eventStore) : base(eventStore)
  {
    _database = database;
  }

  public async Task<Spell?> LoadAsync(SpellId id, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Spell>(id.StreamId, isDeleted: false, cancellationToken);
  }
  public async Task<IReadOnlyCollection<Spell>> LoadAsync(IEnumerable<SpellId> ids, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Spell>(ids.Select(id => id.StreamId), isDeleted: false, cancellationToken);
  }

  public async Task SaveAsync(Spell spell, CancellationToken cancellationToken)
  {
    await base.SaveAsync(spell, cancellationToken);

    await SynchronizeAsync(spell, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<Spell> spells, CancellationToken cancellationToken)
  {
    await base.SaveAsync(spells, cancellationToken);

    foreach (Spell spell in spells)
    {
      await SynchronizeAsync(spell, cancellationToken);
    }
  }

  private async Task SynchronizeAsync(Spell spell, CancellationToken cancellationToken)
  {
    SpellEntity? entity = await _database.Spells.SingleOrDefaultAsync(x => x.StreamId == spell.Id.Value, cancellationToken);
    if (entity is null)
    {
      entity = new SpellEntity(spell);
      _database.Spells.Add(entity);
    }
    else
    {
      entity.Update(spell);
    }
    await _database.SaveChangesAsync(cancellationToken);
  }
}
