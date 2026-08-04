using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Api.Core.Lineages;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Repositories;

internal class LineageRepository : Repository, ILineageRepository
{
  private readonly GameContext _database;

  public LineageRepository(GameContext database, IEventStore eventStore) : base(eventStore)
  {
    _database = database;
  }

  public async Task<Lineage?> LoadAsync(LineageId id, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Lineage>(id.StreamId, isDeleted: false, cancellationToken);
  }
  public async Task<IReadOnlyCollection<Lineage>> LoadAsync(IEnumerable<LineageId> ids, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Lineage>(ids.Select(id => id.StreamId), isDeleted: false, cancellationToken);
  }

  public async Task SaveAsync(Lineage lineage, CancellationToken cancellationToken)
  {
    await base.SaveAsync(lineage, cancellationToken);

    await SynchronizeAsync(lineage, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<Lineage> lineages, CancellationToken cancellationToken)
  {
    await base.SaveAsync(lineages, cancellationToken);

    foreach (Lineage lineage in lineages)
    {
      await SynchronizeAsync(lineage, cancellationToken);
    }
  }

  private async Task SynchronizeAsync(Lineage lineage, CancellationToken cancellationToken)
  {
    int? parentId = null;
    if (lineage.ParentId.HasValue)
    {
      parentId = await _database.Lineages
        .Where(x => x.StreamId == lineage.ParentId.Value.Value)
        .Select(x => (int?)x.LineageId)
        .SingleOrDefaultAsync(cancellationToken)
        ?? throw new InvalidOperationException($"The lineage entity 'StreamId={lineage.ParentId}' was not found.");
    }

    HashSet<string> languageIds = lineage.Languages.Ids.Select(id => id.StreamId.Value).ToHashSet();
    LanguageEntity[] languages = await _database.Languages
      .Where(x => languageIds.Contains(x.StreamId))
      .ToArrayAsync(cancellationToken);

    LineageEntity? entity = await _database.Lineages
      .Include(x => x.Languages)
      .SingleOrDefaultAsync(x => x.StreamId == lineage.Id.Value, cancellationToken);
    if (entity is null)
    {
      entity = new LineageEntity(lineage, parentId, languages);
      _database.Lineages.Add(entity);
    }
    else
    {
      entity.Update(lineage, parentId, languages);
    }
    await _database.SaveChangesAsync(cancellationToken);
  }
}
