using Logitar.CQRS;
using SkillCraft.Api.Contracts.Languages;

namespace SkillCraft.Api.Core.Languages.Queries;

internal record ReadLanguageQuery(Guid Id) : IQuery<LanguageModel?>;

internal class ReadLanguageQueryHandler : IQueryHandler<ReadLanguageQuery, LanguageModel?>
{
  private readonly ILanguageQuerier _languageQuerier;

  public ReadLanguageQueryHandler(ILanguageQuerier languageQuerier)
  {
    _languageQuerier = languageQuerier;
  }

  public async Task<LanguageModel?> HandleAsync(ReadLanguageQuery query, CancellationToken cancellationToken)
  {
    return await _languageQuerier.ReadAsync(query.Id, cancellationToken);
  }
}
