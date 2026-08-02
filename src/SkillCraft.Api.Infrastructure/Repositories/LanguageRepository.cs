using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Repositories;

internal class LanguageRepository : Logitar.EventSourcing.Repository, ILanguageRepository
{
  private readonly GameContext _database;

  public LanguageRepository(GameContext database, IEventStore eventStore) : base(eventStore)
  {
    _database = database;
  }

  public async Task<Language?> LoadAsync(LanguageId id, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Language>(id.StreamId, isDeleted: false, cancellationToken);
  }
  public async Task<IReadOnlyCollection<Language>> LoadAsync(IEnumerable<LanguageId> ids, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Language>(ids.Select(id => id.StreamId), isDeleted: false, cancellationToken);
  }

  public async Task SaveAsync(Language language, CancellationToken cancellationToken)
  {
    await base.SaveAsync(language, cancellationToken);

    await SynchronizeAsync(language, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<Language> languages, CancellationToken cancellationToken)
  {
    await base.SaveAsync(languages, cancellationToken);

    foreach (Language language in languages)
    {
      await SynchronizeAsync(language, cancellationToken);
    }
  }

  private async Task SynchronizeAsync(Language language, CancellationToken cancellationToken)
  {
    int? scriptId = null;
    if (language.ScriptId.HasValue)
    {
      scriptId = await _database.Scripts
        .Where(x => x.StreamId == language.ScriptId.Value.Value)
        .Select(x => (int?)x.ScriptId)
        .SingleOrDefaultAsync(cancellationToken)
        ?? throw new InvalidOperationException($"The script entity 'StreamId={language.ScriptId}' was not found.");
    }

    LanguageEntity? entity = await _database.Languages.SingleOrDefaultAsync(x => x.StreamId == language.Id.Value, cancellationToken);
    if (entity is null)
    {
      entity = new LanguageEntity(language, scriptId);
      _database.Languages.Add(entity);
    }
    else
    {
      entity.Update(language, scriptId);
    }
    await _database.SaveChangesAsync(cancellationToken);
  }
}
