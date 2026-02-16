using Krakenar.Contracts.Search;
using Logitar.CQRS;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Contracts.Scripts;
using SkillCraft.Api.Core.Scripts.Commands;
using SkillCraft.Api.Core.Scripts.Queries;

namespace SkillCraft.Api.Core.Scripts;

internal class ScriptService : IScriptService
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IScriptService, ScriptService>();
    services.AddTransient<ICommandHandler<CreateOrReplaceScriptCommand, CreateOrReplaceScriptResult>, CreateOrReplaceScriptCommandHandler>();
    services.AddTransient<ICommandHandler<DeleteScriptCommand, ScriptModel?>, DeleteScriptCommandHandler>();
    services.AddTransient<ICommandHandler<UpdateScriptCommand, ScriptModel?>, UpdateScriptCommandHandler>();
    services.AddTransient<IQueryHandler<ReadScriptQuery, ScriptModel?>, ReadScriptQueryHandler>();
    services.AddTransient<IQueryHandler<SearchScriptsQuery, SearchResults<ScriptModel>>, SearchScriptsQueryHandler>();
  }

  private readonly ICommandBus _commandBus;
  private readonly IQueryBus _queryBus;

  public ScriptService(ICommandBus commandBus, IQueryBus queryBus)
  {
    _commandBus = commandBus;
    _queryBus = queryBus;
  }

  public async Task<CreateOrReplaceScriptResult> CreateOrReplaceAsync(CreateOrReplaceScriptPayload payload, Guid? id, CancellationToken cancellationToken)
  {
    CreateOrReplaceScriptCommand command = new(payload, id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<ScriptModel?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    DeleteScriptCommand command = new(id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<ScriptModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ReadScriptQuery query = new(id);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public async Task<SearchResults<ScriptModel>> SearchAsync(SearchScriptsPayload payload, CancellationToken cancellationToken)
  {
    SearchScriptsQuery query = new(payload);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public async Task<ScriptModel?> UpdateAsync(Guid id, UpdateScriptPayload payload, CancellationToken cancellationToken)
  {
    UpdateScriptCommand command = new(id, payload);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }
}
