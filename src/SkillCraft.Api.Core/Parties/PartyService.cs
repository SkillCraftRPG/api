using Krakenar.Contracts.Search;
using Logitar.CQRS;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Contracts.Parties;
using SkillCraft.Api.Core.Parties.Commands;
using SkillCraft.Api.Core.Parties.Queries;

namespace SkillCraft.Api.Core.Parties;

internal class PartyService : IPartyService
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IPartyService, PartyService>();
    services.AddTransient<ICommandHandler<CreateOrReplacePartyCommand, CreateOrReplacePartyResult>, CreateOrReplacePartyCommandHandler>();
    services.AddTransient<ICommandHandler<DeletePartyCommand, PartyModel?>, DeletePartyCommandHandler>();
    services.AddTransient<ICommandHandler<UpdatePartyCommand, PartyModel?>, UpdatePartyCommandHandler>();
    services.AddTransient<IQueryHandler<ReadPartyQuery, PartyModel?>, ReadPartyQueryHandler>();
    services.AddTransient<IQueryHandler<SearchPartiesQuery, SearchResults<PartyModel>>, SearchPartiesQueryHandler>();
  }

  private readonly ICommandBus _commandBus;
  private readonly IQueryBus _queryBus;

  public PartyService(ICommandBus commandBus, IQueryBus queryBus)
  {
    _commandBus = commandBus;
    _queryBus = queryBus;
  }

  public async Task<CreateOrReplacePartyResult> CreateOrReplaceAsync(CreateOrReplacePartyPayload payload, Guid? id, CancellationToken cancellationToken)
  {
    CreateOrReplacePartyCommand command = new(payload, id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<PartyModel?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    DeletePartyCommand command = new(id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<PartyModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ReadPartyQuery query = new(id);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public async Task<SearchResults<PartyModel>> SearchAsync(SearchPartiesPayload payload, CancellationToken cancellationToken)
  {
    SearchPartiesQuery query = new(payload);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public async Task<PartyModel?> UpdateAsync(Guid id, UpdatePartyPayload payload, CancellationToken cancellationToken)
  {
    UpdatePartyCommand command = new(id, payload);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }
}
