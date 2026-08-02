using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Search;
using Logitar.Data;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Lineages;
using SkillCraft.Api.Core.Lineages.Events;
using SkillCraft.Api.Core.Lineages.Models;
using SkillCraft.Api.Core.Scripts.Models;
using SkillCraft.Api.Infrastructure.Actors;
using SkillCraft.Api.Infrastructure.Configurations;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Repositories;

internal class LineageRepository : Repository, ILineageRepository
{
  private readonly IActorService _actorService;
  private readonly IContext _context;
  private readonly ILanguageRepository _languageRepository;
  private readonly ISqlHelper _sqlHelper;

  public LineageRepository(
    IActorService actorService,
    IContext context,
    GameContext game,
    ILanguageRepository languageRepository,
    ISqlHelper sqlHelper) : base(game)
  {
    _actorService = actorService;
    _context = context;
    _languageRepository = languageRepository;
    _sqlHelper = sqlHelper;
  }

  public void Add(params Lineage[] lineages)
  {
    foreach (Lineage lineage in lineages)
    {
      Database.Lineages.Add(lineage);
      SyncLanguages(lineage);
      base.RecordChange(new LineageCreated(lineage));
    }
  }
  public void Add(LineageFeature feature)
  {
    Database.LineageFeatures.Add(feature);
    base.RecordChange(new LineageFeatureCreated(feature));
  }
  public void Remove(Lineage lineage)
  {
    Database.Lineages.Remove(lineage);
    base.RecordChange(new LineageDeleted(lineage, _context.UserUid));
  }
  public void Remove(LineageFeature feature)
  {
    Database.LineageFeatures.Remove(feature);
    base.RecordChange(new LineageFeatureDeleted(feature));
  }
  public void Update(Lineage lineage, LineageUpdated record)
  {
    Database.Lineages.Update(lineage);
    SyncLanguages(lineage);
    base.RecordChange(record);
  }
  public void Update(LineageFeature feature, LineageFeatureUpdated record)
  {
    Database.LineageFeatures.Update(feature);
    base.RecordChange(record);
  }

  public async Task<Lineage?> LoadAsync(Guid id, CancellationToken cancellationToken)
  {
    Lineage? lineage = await Database.Lineages
      .Include(x => x.Features)
      .Include(x => x.Parent)
      .SingleOrDefaultAsync(x => x.Id == id && x.WorldId == _context.WorldUid, cancellationToken);
    if (lineage is not null)
    {
      await HydrateLanguagesAsync([lineage], cancellationToken);
    }
    return lineage;
  }

  public async Task<LineageModel> ReadAsync(Lineage lineage, CancellationToken cancellationToken)
  {
    return await ReadAsync(lineage.Id, cancellationToken) ?? throw new InvalidOperationException($"The lineage 'Id={lineage.Id}' was not found.");
  }
  public async Task<LineageModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Lineage? lineage = await Database.Lineages.AsNoTracking().AsSplitQuery()
      .Where(x => x.Id == id && x.WorldId == _context.WorldUid)
      .Include(x => x.Features)
      .Include(x => x.Parent).ThenInclude(x => x!.Features)
      .SingleOrDefaultAsync(cancellationToken);

    return lineage is null ? null : await MapAsync(lineage, cancellationToken);
  }

  public virtual async Task<SearchResults<LineageModel>> SearchAsync(SearchLineagesPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.Query(Db.Lineages.Table).SelectAll(Db.Lineages.Table)
      .Where(Db.Lineages.WorldId, Operators.IsEqualTo(_context.WorldUid))
      .ApplyIdFilter(Db.Lineages.Id, payload.Ids);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, Db.Lineages.Name, Db.Lineages.Summary);

    if (payload.ParentId.HasValue)
    {
      TableId parentLineages = new(Db.Lineages.Table.Schema, Db.Lineages.Table.Table!, "ParentLineage");
      ColumnId parentLineageId = new(nameof(Lineage.LineageId), parentLineages);
      ColumnId parentLineageUid = new(nameof(Lineage.Id), parentLineages);
      OperatorCondition condition = new(parentLineageUid, Operators.IsEqualTo(payload.ParentId.Value));
      builder.Join(parentLineageId, Db.Lineages.ParentId, condition);
    }
    else
    {
      builder.Where(Db.Lineages.ParentId, Operators.IsNull());
    }

    if (payload.SizeCategory.HasValue)
    {
      builder.Where(Db.Lineages.SizeCategory, Operators.IsEqualTo(payload.SizeCategory.Value.ToString()));
    }

    IQueryable<Lineage> query = Database.Lineages.FromQuery(builder).AsNoTracking();

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<Lineage>? ordered = null;
    foreach (LineageSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case LineageSort.CreatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case LineageSort.Name:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Name) : ordered.ThenBy(x => x.Name));
          break;
        case LineageSort.UpdatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    Lineage[] lineages = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<LineageModel> items = await MapAsync(lineages, cancellationToken);

    return new SearchResults<LineageModel>(items, total);
  }

  private void SyncLanguages(Lineage lineage)
  {
    List<LineageLanguage> existing = lineage.LineageId > 0
      ? Database.LineageLanguages.Where(x => x.LineageId == lineage.LineageId).ToList()
      : Database.LineageLanguages.Local.Where(x => ReferenceEquals(x.Lineage, lineage)).ToList();
    Database.LineageLanguages.RemoveRange(existing);

    foreach (Language language in lineage.Languages)
    {
      LanguageEntity entity = Database.Languages.Local.FirstOrDefault(x => x.StreamId == language.Id.Value)
        ?? Database.Languages.SingleOrDefault(x => x.StreamId == language.Id.Value)
        ?? throw new InvalidOperationException($"The language entity 'StreamId={language.Id}' was not found.");

      Database.LineageLanguages.Add(new LineageLanguage
      {
        Lineage = lineage,
        Language = entity
      });
    }
  }

  private async Task HydrateLanguagesAsync(IEnumerable<Lineage> lineages, CancellationToken cancellationToken)
  {
    Lineage[] lineageList = [.. lineages];
    int[] lineageIds = [.. lineageList.Select(lineage => lineage.LineageId).Where(id => id > 0)];
    if (lineageIds.Length < 1)
    {
      return;
    }

    var links = await (
      from link in Database.LineageLanguages.AsNoTracking()
      join language in Database.Languages.AsNoTracking() on link.LanguageId equals language.LanguageId
      where lineageIds.Contains(link.LineageId)
      select new { link.LineageId, language.StreamId }).ToListAsync(cancellationToken);

    Dictionary<int, List<LanguageId>> languageIdsByLineage = links
      .GroupBy(x => x.LineageId)
      .ToDictionary(group => group.Key, group => group.Select(x => new LanguageId(x.StreamId)).ToList());

    LanguageId[] allLanguageIds = [.. languageIdsByLineage.Values.SelectMany(ids => ids).Distinct()];
    Dictionary<string, Language> languages = allLanguageIds.Length < 1
      ? []
      : (await _languageRepository.LoadAsync(allLanguageIds, cancellationToken)).ToDictionary(language => language.Id.Value);

    foreach (Lineage lineage in lineageList)
    {
      lineage.Languages.Clear();
      if (languageIdsByLineage.TryGetValue(lineage.LineageId, out List<LanguageId>? ids))
      {
        foreach (LanguageId id in ids)
        {
          if (languages.TryGetValue(id.Value, out Language? language))
          {
            lineage.Languages.Add(language);
          }
        }
      }
    }
  }

  private async Task<LineageModel> MapAsync(Lineage lineage, CancellationToken cancellationToken)
  {
    return (await MapAsync([lineage], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<LineageModel>> MapAsync(IEnumerable<Lineage> lineages, CancellationToken cancellationToken)
  {
    Lineage[] lineageList = [.. lineages];
    Dictionary<int, List<LanguageEntity>> languagesByLineageId = await LoadLanguageEntitiesAsync(lineageList, cancellationToken);
    Dictionary<int, ScriptModel> scripts = await LoadScriptsAsync(languagesByLineageId.Values.SelectMany(x => x), cancellationToken);

    IEnumerable<Guid> userIds = lineageList.SelectMany(lineage => lineage.GetUserIds());
    IReadOnlyDictionary<Guid, Actor> actors = await _actorService.FindAsync(userIds, cancellationToken);
    MapperOld mapper = new(actors);

    IEnumerable<ActorId> languageActorIds = languagesByLineageId.Values.SelectMany(languages => languages.SelectMany(language => language.GetActorIds()));
    IReadOnlyDictionary<ActorId, Actor> languageActors = await _actorService.FindAsync(languageActorIds, cancellationToken);
    Mapper languageMapper = new(languageActors);

    return lineageList.Select(lineage =>
    {
      IReadOnlyList<LanguageEntity> languages = languagesByLineageId.GetValueOrDefault(lineage.LineageId) ?? [];
      IReadOnlyList<LanguageEntity>? parentLanguages = lineage.Parent is null
        ? null
        : languagesByLineageId.GetValueOrDefault(lineage.Parent.LineageId);
      return mapper.ToLineage(lineage, scripts, languages, parentLanguages, languageMapper);
    }).ToList().AsReadOnly();
  }

  private async Task<Dictionary<int, List<LanguageEntity>>> LoadLanguageEntitiesAsync(IEnumerable<Lineage> lineages, CancellationToken cancellationToken)
  {
    Lineage[] lineageList = [.. lineages];
    HashSet<int> lineageIds = [.. lineageList.Select(lineage => lineage.LineageId)];
    foreach (Lineage lineage in lineageList)
    {
      if (lineage.Parent is not null)
      {
        lineageIds.Add(lineage.Parent.LineageId);
      }
    }
    lineageIds.Remove(0);
    if (lineageIds.Count < 1)
    {
      return [];
    }

    var links = await (
      from link in Database.LineageLanguages.AsNoTracking()
      join language in Database.Languages.AsNoTracking().Include(x => x.Script) on link.LanguageId equals language.LanguageId
      where lineageIds.Contains(link.LineageId)
      select new { link.LineageId, Language = language }).ToListAsync(cancellationToken);

    return links
      .GroupBy(x => x.LineageId)
      .ToDictionary(group => group.Key, group => group.Select(x => x.Language).ToList());
  }

  private async Task<Dictionary<int, ScriptModel>> LoadScriptsAsync(IEnumerable<LanguageEntity> languages, CancellationToken cancellationToken)
  {
    int[] keys = [.. languages.Where(language => language.ScriptId.HasValue).Select(language => language.ScriptId!.Value).Distinct()];
    if (keys.Length < 1)
    {
      return [];
    }

    ScriptEntity[] entities = await Database.Scripts.AsNoTracking()
      .Where(script => keys.Contains(script.ScriptId))
      .ToArrayAsync(cancellationToken);

    IEnumerable<ActorId> actorIds = entities.SelectMany(script => script.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return entities.ToDictionary(script => script.ScriptId, mapper.ToScript);
  }
}
