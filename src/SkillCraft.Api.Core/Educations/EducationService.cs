using Krakenar.Contracts.Search;
using Logitar.CQRS;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Contracts.Educations;
using SkillCraft.Api.Core.Educations.Commands;
using SkillCraft.Api.Core.Educations.Queries;

namespace SkillCraft.Api.Core.Educations;

internal class EducationService : IEducationService
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IEducationService, EducationService>();
    services.AddTransient<ICommandHandler<CreateOrReplaceEducationCommand, CreateOrReplaceEducationResult>, CreateOrReplaceEducationCommandHandler>();
    services.AddTransient<ICommandHandler<DeleteEducationCommand, EducationModel?>, DeleteEducationCommandHandler>();
    services.AddTransient<ICommandHandler<UpdateEducationCommand, EducationModel?>, UpdateEducationCommandHandler>();
    services.AddTransient<IQueryHandler<ReadEducationQuery, EducationModel?>, ReadEducationQueryHandler>();
    services.AddTransient<IQueryHandler<SearchEducationsQuery, SearchResults<EducationModel>>, SearchEducationsQueryHandler>();
  }

  private readonly ICommandBus _commandBus;
  private readonly IQueryBus _queryBus;

  public EducationService(ICommandBus commandBus, IQueryBus queryBus)
  {
    _commandBus = commandBus;
    _queryBus = queryBus;
  }

  public async Task<CreateOrReplaceEducationResult> CreateOrReplaceAsync(CreateOrReplaceEducationPayload payload, Guid? id, CancellationToken cancellationToken)
  {
    CreateOrReplaceEducationCommand command = new(payload, id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<EducationModel?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    DeleteEducationCommand command = new(id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<EducationModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ReadEducationQuery query = new(id);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public async Task<SearchResults<EducationModel>> SearchAsync(SearchEducationsPayload payload, CancellationToken cancellationToken)
  {
    SearchEducationsQuery query = new(payload);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public async Task<EducationModel?> UpdateAsync(Guid id, UpdateEducationPayload payload, CancellationToken cancellationToken)
  {
    UpdateEducationCommand command = new(id, payload);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }
}
