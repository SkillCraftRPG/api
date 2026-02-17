using Krakenar.Contracts.Search;
using Logitar.CQRS;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Contracts.Lineages;
using SkillCraft.Api.Core.Lineages.Commands;
using SkillCraft.Api.Core.Lineages.Queries;

namespace SkillCraft.Api.Core.Lineages;

internal class LineageService : ILineageService
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<ILineageService, LineageService>();
    services.AddTransient<ICommandHandler<CreateOrReplaceLineageCommand, CreateOrReplaceLineageResult>, CreateOrReplaceLineageCommandHandler>();
    services.AddTransient<ICommandHandler<DeleteLineageCommand, LineageModel?>, DeleteLineageCommandHandler>();
    services.AddTransient<ICommandHandler<UpdateLineageCommand, LineageModel?>, UpdateLineageCommandHandler>();
    services.AddTransient<IQueryHandler<ReadLineageQuery, LineageModel?>, ReadLineageQueryHandler>();
    services.AddTransient<IQueryHandler<SearchLineagesQuery, SearchResults<LineageModel>>, SearchLineagesQueryHandler>();
  }

  private readonly ICommandBus _commandBus;
  private readonly IQueryBus _queryBus;

  public LineageService(ICommandBus commandBus, IQueryBus queryBus)
  {
    _commandBus = commandBus;
    _queryBus = queryBus;
  }

  public async Task<CreateOrReplaceLineageResult> CreateOrReplaceAsync(CreateOrReplaceLineagePayload payload, Guid? id, CancellationToken cancellationToken)
  {
    CreateOrReplaceLineageCommand command = new(payload, id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<LineageModel?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    DeleteLineageCommand command = new(id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<LineageModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ReadLineageQuery query = new(id);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public async Task<SearchResults<LineageModel>> SearchAsync(SearchLineagesPayload payload, CancellationToken cancellationToken)
  {
    SearchLineagesQuery query = new(payload);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public async Task<LineageModel?> UpdateAsync(Guid id, UpdateLineagePayload payload, CancellationToken cancellationToken)
  {
    UpdateLineageCommand command = new(id, payload);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }
}
