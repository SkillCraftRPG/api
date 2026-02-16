using Logitar.EventSourcing;
using SkillCraft.Api.Core.Talents;

namespace SkillCraft.Api.Infrastructure.Repositories;

internal class TalentRepository : Repository, ITalentRepository
{
  public TalentRepository(IEventStore eventStore) : base(eventStore)
  {
  }

  public async Task<Talent?> LoadAsync(TalentId id, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Talent>(id.StreamId, cancellationToken);
  }

  public async Task<IReadOnlyCollection<Talent>> LoadAsync(IEnumerable<TalentId> ids, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Talent>(ids.Select(id => id.StreamId), cancellationToken);
  }

  public async Task SaveAsync(Talent talent, CancellationToken cancellationToken)
  {
    await base.SaveAsync(talent, cancellationToken);
  }

  public async Task SaveAsync(IEnumerable<Talent> talents, CancellationToken cancellationToken)
  {
    await base.SaveAsync(talents, cancellationToken);
  }
}
