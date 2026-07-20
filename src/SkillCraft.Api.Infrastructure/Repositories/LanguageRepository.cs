using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Search;
using Logitar.Data;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Languages.Events;
using SkillCraft.Api.Core.Languages.Models;
using SkillCraft.Api.Infrastructure.Actors;

namespace SkillCraft.Api.Infrastructure.Repositories;

internal class LanguageRepository : Repository, ILanguageRepository
{
  private readonly IActorService _actorService;
  private readonly IContext _context;
  private readonly ISqlHelper _sqlHelper;

  public LanguageRepository(IActorService actorService, IContext context, GameContext game, ISqlHelper sqlHelper) : base(game)
  {
    _actorService = actorService;
    _context = context;
    _sqlHelper = sqlHelper;
  }

  public void Add(params Language[] languages)
  {
    foreach (Language language in languages)
    {
      Database.Languages.Add(language);
      base.RecordChange(new LanguageCreated(language));
    }
  }
  public void Remove(Language language)
  {
    Database.Languages.Remove(language);
    base.RecordChange(new LanguageDeleted(language, _context.UserId));
  }
  public void Update(Language language, LanguageUpdated record)
  {
    Database.Languages.Update(language);
    base.RecordChange(record);
  }

  public async Task<Language?> LoadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await Database.Languages
      .Include(x => x.Script)
      .SingleOrDefaultAsync(x => x.Id == id && x.WorldId == _context.WorldId, cancellationToken);
  }

  public async Task<LanguageModel> ReadAsync(Language language, CancellationToken cancellationToken)
  {
    return await ReadAsync(language.Id, cancellationToken) ?? throw new InvalidOperationException($"The language 'Id={language.Id}' was not found.");
  }
  public async Task<LanguageModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Language? language = await Database.Languages.AsNoTracking()
      .Where(x => x.Id == id && x.WorldId == _context.WorldId)
      .Include(x => x.Script)
      .SingleOrDefaultAsync(cancellationToken);

    return language is null ? null : await MapAsync(language, cancellationToken);
  }

  public virtual async Task<SearchResults<LanguageModel>> SearchAsync(SearchLanguagesPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.Query(Db.Languages.Table).SelectAll(Db.Languages.Table)
      .Where(Db.Languages.WorldId, Operators.IsEqualTo(_context.WorldId))
      .ApplyIdFilter(Db.Languages.Id, payload.Ids);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, Db.Languages.Name, Db.Languages.Summary);

    if (payload.ScriptId.HasValue)
    {
      OperatorCondition condition = new(Db.Scripts.Id, Operators.IsEqualTo(payload.ScriptId.Value));
      builder.Join(Db.Scripts.ScriptId, Db.Languages.ScriptId, condition);
    }

    IQueryable<Language> query = Database.Languages.FromQuery(builder).AsNoTracking()
      .Include(x => x.Script);

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<Language>? ordered = null;
    foreach (LanguageSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case LanguageSort.CreatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case LanguageSort.Name:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Name) : ordered.ThenBy(x => x.Name));
          break;
        case LanguageSort.UpdatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    Language[] languages = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<LanguageModel> items = await MapAsync(languages, cancellationToken);

    return new SearchResults<LanguageModel>(items, total);
  }

  private async Task<LanguageModel> MapAsync(Language language, CancellationToken cancellationToken)
  {
    return (await MapAsync([language], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<LanguageModel>> MapAsync(IEnumerable<Language> languages, CancellationToken cancellationToken)
  {
    IEnumerable<Guid> userIds = languages.SelectMany(language => language.GetUserIds());
    IReadOnlyDictionary<Guid, Actor> actors = await _actorService.FindAsync(userIds, cancellationToken);
    Mapper mapper = new(actors);

    return languages.Select(mapper.ToLanguage).ToList().AsReadOnly();
  }
}
