using Logitar.EventSourcing;
using SkillCraft.Api.Core.Languages;

namespace SkillCraft.Api.Infrastructure.Repositories;

internal class LanguageRepository : Repository, ILanguageRepository
{
  public LanguageRepository(IEventStore eventStore) : base(eventStore)
  {
  }

  public async Task<Language?> LoadAsync(LanguageId id, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Language>(id.StreamId, cancellationToken);
  }

  public async Task<IReadOnlyCollection<Language>> LoadAsync(IEnumerable<LanguageId> ids, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Language>(ids.Select(id => id.StreamId), cancellationToken);
  }

  public async Task SaveAsync(Language language, CancellationToken cancellationToken)
  {
    await base.SaveAsync(language, cancellationToken);
  }

  public async Task SaveAsync(IEnumerable<Language> languages, CancellationToken cancellationToken)
  {
    await base.SaveAsync(languages, cancellationToken);
  }
}
