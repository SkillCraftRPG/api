using Krakenar.Contracts.Search;
using Logitar.CQRS;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Contracts.Specializations;
using SkillCraft.Api.Core.Specializations.Commands;
using SkillCraft.Api.Core.Specializations.Queries;

namespace SkillCraft.Api.Core.Specializations;

internal class SpecializationService : ISpecializationService
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<ISpecializationService, SpecializationService>();
    services.AddTransient<ICommandHandler<CreateOrReplaceSpecializationCommand, CreateOrReplaceSpecializationResult>, CreateOrReplaceSpecializationCommandHandler>();
    services.AddTransient<ICommandHandler<DeleteSpecializationCommand, SpecializationModel?>, DeleteSpecializationCommandHandler>();
    services.AddTransient<ICommandHandler<UpdateSpecializationCommand, SpecializationModel?>, UpdateSpecializationCommandHandler>();
    services.AddTransient<IQueryHandler<ReadSpecializationQuery, SpecializationModel?>, ReadSpecializationQueryHandler>();
    services.AddTransient<IQueryHandler<SearchSpecializationsQuery, SearchResults<SpecializationModel>>, SearchSpecializationsQueryHandler>();
  }

  private readonly ICommandBus _commandBus;
  private readonly IQueryBus _queryBus;

  public SpecializationService(ICommandBus commandBus, IQueryBus queryBus)
  {
    _commandBus = commandBus;
    _queryBus = queryBus;
  }

  public async Task<CreateOrReplaceSpecializationResult> CreateOrReplaceAsync(CreateOrReplaceSpecializationPayload payload, Guid? id, CancellationToken cancellationToken)
  {
    CreateOrReplaceSpecializationCommand command = new(payload, id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<SpecializationModel?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    DeleteSpecializationCommand command = new(id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<SpecializationModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ReadSpecializationQuery query = new(id);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public async Task<SearchResults<SpecializationModel>> SearchAsync(SearchSpecializationsPayload payload, CancellationToken cancellationToken)
  {
    SearchSpecializationsQuery query = new(payload);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public async Task<SpecializationModel?> UpdateAsync(Guid id, UpdateSpecializationPayload payload, CancellationToken cancellationToken)
  {
    UpdateSpecializationCommand command = new(id, payload);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }
}
