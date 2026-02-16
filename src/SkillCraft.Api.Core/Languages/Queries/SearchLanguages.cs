using Krakenar.Contracts.Search;
using Logitar.CQRS;
using SkillCraft.Api.Contracts.Languages;

namespace SkillCraft.Api.Core.Languages.Queries;

internal record SearchLanguagesQuery(SearchLanguagesPayload Payload) : IQuery<SearchResults<LanguageModel>>;

internal class SearchLanguagesQueryHandler : IQueryHandler<SearchLanguagesQuery, SearchResults<LanguageModel>>
{
  private readonly ILanguageQuerier _languageQuerier;

  public SearchLanguagesQueryHandler(ILanguageQuerier languageQuerier)
  {
    _languageQuerier = languageQuerier;
  }

  public async Task<SearchResults<LanguageModel>> HandleAsync(SearchLanguagesQuery query, CancellationToken cancellationToken)
  {
    return await _languageQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
