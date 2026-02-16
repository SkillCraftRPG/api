using Krakenar.Contracts.Search;
using Logitar.CQRS;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Contracts.Talents;
using SkillCraft.Api.Core.Talents.Commands;
using SkillCraft.Api.Core.Talents.Queries;

namespace SkillCraft.Api.Core.Talents;

internal class TalentService : ITalentService
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<ITalentService, TalentService>();
    services.AddTransient<ICommandHandler<CreateOrReplaceTalentCommand, CreateOrReplaceTalentResult>, CreateOrReplaceTalentCommandHandler>();
    services.AddTransient<ICommandHandler<DeleteTalentCommand, TalentModel?>, DeleteTalentCommandHandler>();
    services.AddTransient<ICommandHandler<UpdateTalentCommand, TalentModel?>, UpdateTalentCommandHandler>();
    services.AddTransient<IQueryHandler<ReadTalentQuery, TalentModel?>, ReadTalentQueryHandler>();
    services.AddTransient<IQueryHandler<SearchTalentsQuery, SearchResults<TalentModel>>, SearchTalentsQueryHandler>();
  }

  private readonly ICommandBus _commandBus;
  private readonly IQueryBus _queryBus;

  public TalentService(ICommandBus commandBus, IQueryBus queryBus)
  {
    _commandBus = commandBus;
    _queryBus = queryBus;
  }

  public async Task<CreateOrReplaceTalentResult> CreateOrReplaceAsync(CreateOrReplaceTalentPayload payload, Guid? id, CancellationToken cancellationToken)
  {
    CreateOrReplaceTalentCommand command = new(payload, id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<TalentModel?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    DeleteTalentCommand command = new(id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<TalentModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ReadTalentQuery query = new(id);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public async Task<SearchResults<TalentModel>> SearchAsync(SearchTalentsPayload payload, CancellationToken cancellationToken)
  {
    SearchTalentsQuery query = new(payload);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public async Task<TalentModel?> UpdateAsync(Guid id, UpdateTalentPayload payload, CancellationToken cancellationToken)
  {
    UpdateTalentCommand command = new(id, payload);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }
}
