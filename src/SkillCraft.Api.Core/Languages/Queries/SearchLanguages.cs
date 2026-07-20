using Krakenar.Contracts.Search;
using Logitar.CQRS;
using SkillCraft.Api.Core.Languages.Models;

namespace SkillCraft.Api.Core.Languages.Queries;

internal record SearchLanguagesQuery(SearchLanguagesPayload Payload) : IQuery<SearchResults<LanguageModel>>;

internal class SearchLanguagesQueryHandler : IQueryHandler<SearchLanguagesQuery, SearchResults<LanguageModel>>
{
  private readonly ILanguageRepository _languageRepository;

  public SearchLanguagesQueryHandler(ILanguageRepository languageRepository)
  {
    _languageRepository = languageRepository;
  }

  public async Task<SearchResults<LanguageModel>> HandleAsync(SearchLanguagesQuery query, CancellationToken cancellationToken)
  {
    return await _languageRepository.SearchAsync(query.Payload, cancellationToken);
  }
}
