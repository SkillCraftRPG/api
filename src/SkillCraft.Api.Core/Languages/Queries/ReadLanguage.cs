using Logitar.CQRS;
using SkillCraft.Api.Core.Languages.Models;

namespace SkillCraft.Api.Core.Languages.Queries;

internal record ReadLanguageQuery(Guid Id) : IQuery<LanguageModel?>;

internal class ReadLanguageQueryHandler : IQueryHandler<ReadLanguageQuery, LanguageModel?>
{
  private readonly ILanguageRepository _languageRepository;

  public ReadLanguageQueryHandler(ILanguageRepository languageRepository)
  {
    _languageRepository = languageRepository;
  }

  public async Task<LanguageModel?> HandleAsync(ReadLanguageQuery query, CancellationToken cancellationToken)
  {
    return await _languageRepository.ReadAsync(query.Id, cancellationToken);
  }
}
