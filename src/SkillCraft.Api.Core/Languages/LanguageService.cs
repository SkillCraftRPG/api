using Krakenar.Contracts.Search;
using Logitar.CQRS;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Core.Languages.Commands;
using SkillCraft.Api.Core.Languages.Models;
using SkillCraft.Api.Core.Languages.Queries;

namespace SkillCraft.Api.Core.Languages;

public interface ILanguageService
{
  Task<CreateOrReplaceLanguageResult> CreateOrReplaceAsync(CreateOrReplaceLanguagePayload payload, Guid? id = null, CancellationToken cancellationToken = default);
  Task<LanguageModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<SearchResults<LanguageModel>> SearchAsync(SearchLanguagesPayload payload, CancellationToken cancellationToken = default);
  Task<LanguageModel?> UpdateAsync(Guid id, UpdateLanguagePayload payload, CancellationToken cancellationToken = default);
}

internal class LanguageService : ILanguageService
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<ILanguageService, LanguageService>();
    services.AddTransient<ICommandHandler<CreateOrReplaceLanguageCommand, CreateOrReplaceLanguageResult>, CreateOrReplaceLanguageCommandHandler>();
    services.AddTransient<ICommandHandler<UpdateLanguageCommand, LanguageModel?>, UpdateLanguageCommandHandler>();
    services.AddTransient<IQueryHandler<ReadLanguageQuery, LanguageModel?>, ReadLanguageQueryHandler>();
    services.AddTransient<IQueryHandler<SearchLanguagesQuery, SearchResults<LanguageModel>>, SearchLanguagesQueryHandler>();
  }

  private readonly ICommandBus _commandBus;
  private readonly IQueryBus _queryBus;

  public LanguageService(ICommandBus commandBus, IQueryBus queryBus)
  {
    _commandBus = commandBus;
    _queryBus = queryBus;
  }

  public async Task<CreateOrReplaceLanguageResult> CreateOrReplaceAsync(CreateOrReplaceLanguagePayload payload, Guid? id, CancellationToken cancellationToken)
  {
    CreateOrReplaceLanguageCommand command = new(payload, id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<LanguageModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ReadLanguageQuery query = new(id);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public async Task<SearchResults<LanguageModel>> SearchAsync(SearchLanguagesPayload payload, CancellationToken cancellationToken)
  {
    SearchLanguagesQuery query = new(payload);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public async Task<LanguageModel?> UpdateAsync(Guid id, UpdateLanguagePayload payload, CancellationToken cancellationToken)
  {
    UpdateLanguageCommand command = new(id, payload);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }
}
