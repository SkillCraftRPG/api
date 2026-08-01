using Krakenar.Contracts.Search;
using Logitar.CQRS;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Core.Features;
using SkillCraft.Api.Core.Lineages.Commands;
using SkillCraft.Api.Core.Lineages.Models;
using SkillCraft.Api.Core.Lineages.Queries;

namespace SkillCraft.Api.Core.Lineages;

public interface ILineageService
{
  Task<CreateOrReplaceLineageResult> CreateOrReplaceAsync(CreateOrReplaceLineagePayload payload, Guid? id = null, CancellationToken cancellationToken = default);
  Task<CreateOrReplaceLineageFeatureResult> CreateOrReplaceFeatureAsync(Guid lineageId, FeatureModel payload, Guid? featureId = null, CancellationToken cancellationToken = default);
  Task<LineageModel?> DeleteFeatureAsync(Guid lineageId, Guid featureId, CancellationToken cancellationToken = default);
  Task<LineageModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<SearchResults<LineageModel>> SearchAsync(SearchLineagesPayload payload, CancellationToken cancellationToken = default);
  Task<LineageModel?> UpdateAsync(Guid id, UpdateLineagePayload payload, CancellationToken cancellationToken = default);
}

internal class LineageService : ILineageService
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<ILineageService, LineageService>();
    services.AddTransient<ICommandHandler<CreateOrReplaceLineageCommand, CreateOrReplaceLineageResult>, CreateOrReplaceLineageCommandHandler>();
    services.AddTransient<ICommandHandler<CreateOrReplaceLineageFeatureCommand, CreateOrReplaceLineageFeatureResult>, CreateOrReplaceLineageFeatureCommandHandler>();
    services.AddTransient<ICommandHandler<DeleteLineageFeatureCommand, LineageModel?>, DeleteLineageFeatureCommandHandler>();
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

  public async Task<CreateOrReplaceLineageFeatureResult> CreateOrReplaceFeatureAsync(Guid lineageId, FeatureModel payload, Guid? featureId, CancellationToken cancellationToken)
  {
    CreateOrReplaceLineageFeatureCommand command = new(lineageId, payload, featureId);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<LineageModel?> DeleteFeatureAsync(Guid lineageId, Guid featureId, CancellationToken cancellationToken)
  {
    DeleteLineageFeatureCommand command = new(lineageId, featureId);
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
