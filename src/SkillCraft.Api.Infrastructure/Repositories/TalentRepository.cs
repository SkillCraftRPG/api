using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Api.Core.Talents;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Repositories;

internal class TalentRepository : Repository, ITalentRepository
{
  private readonly GameContext _database;

  public TalentRepository(GameContext database, IEventStore eventStore) : base(eventStore)
  {
    _database = database;
  }

  public async Task<Talent?> LoadAsync(TalentId id, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Talent>(id.StreamId, isDeleted: false, cancellationToken);
  }
  public async Task<IReadOnlyCollection<Talent>> LoadAsync(IEnumerable<TalentId> ids, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Talent>(ids.Select(id => id.StreamId), isDeleted: false, cancellationToken);
  }

  public async Task SaveAsync(Talent talent, CancellationToken cancellationToken)
  {
    await base.SaveAsync(talent, cancellationToken);

    await SynchronizeAsync(talent, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<Talent> talents, CancellationToken cancellationToken)
  {
    await base.SaveAsync(talents, cancellationToken);

    foreach (Talent talent in talents)
    {
      await SynchronizeAsync(talent, cancellationToken);
    }
  }

  private async Task SynchronizeAsync(Talent talent, CancellationToken cancellationToken)
  {
    int? requiredTalentId = null;
    if (talent.RequiredTalentId.HasValue)
    {
      requiredTalentId = await _database.Talents
        .Where(x => x.StreamId == talent.RequiredTalentId.Value.Value)
        .Select(x => (int?)x.TalentId)
        .SingleOrDefaultAsync(cancellationToken)
        ?? throw new InvalidOperationException($"The talent 'StreamId={talent.RequiredTalentId}' was not found.");
    }

    TalentEntity? entity = await _database.Talents.SingleOrDefaultAsync(x => x.StreamId == talent.Id.Value, cancellationToken);
    if (entity is null)
    {
      entity = new TalentEntity(talent, requiredTalentId);
      _database.Talents.Add(entity);
    }
    else
    {
      entity.Update(talent, requiredTalentId);
    }
    await _database.SaveChangesAsync(cancellationToken);
  }
}
