using Krakenar.Contracts.Search;
using Logitar.CQRS;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Core.Spells.Commands;
using SkillCraft.Api.Core.Spells.Models;
using SkillCraft.Api.Core.Spells.Queries;

namespace SkillCraft.Api.Core.Spells;

public interface ISpellService
{
  Task<CreateOrReplaceSpellResult> CreateOrReplaceAsync(CreateOrReplaceSpellPayload payload, Guid? id = null, CancellationToken cancellationToken = default);
  Task<SpellModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<SearchResults<SpellModel>> SearchAsync(SearchSpellsPayload payload, CancellationToken cancellationToken = default);
  Task<SpellModel?> UpdateAsync(Guid id, UpdateSpellPayload payload, CancellationToken cancellationToken = default);
}

internal class SpellService : ISpellService
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<ISpellService, SpellService>();
    services.AddTransient<ICommandHandler<CreateOrReplaceSpellCommand, CreateOrReplaceSpellResult>, CreateOrReplaceSpellCommandHandler>();
    services.AddTransient<ICommandHandler<UpdateSpellCommand, SpellModel?>, UpdateSpellCommandHandler>();
    services.AddTransient<IQueryHandler<ReadSpellQuery, SpellModel?>, ReadSpellQueryHandler>();
    services.AddTransient<IQueryHandler<SearchSpellsQuery, SearchResults<SpellModel>>, SearchSpellsQueryHandler>();
  }

  private readonly ICommandBus _commandBus;
  private readonly IQueryBus _queryBus;

  public SpellService(ICommandBus commandBus, IQueryBus queryBus)
  {
    _commandBus = commandBus;
    _queryBus = queryBus;
  }

  public async Task<CreateOrReplaceSpellResult> CreateOrReplaceAsync(CreateOrReplaceSpellPayload payload, Guid? id, CancellationToken cancellationToken)
  {
    CreateOrReplaceSpellCommand command = new(payload, id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<SpellModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ReadSpellQuery query = new(id);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public async Task<SearchResults<SpellModel>> SearchAsync(SearchSpellsPayload payload, CancellationToken cancellationToken)
  {
    SearchSpellsQuery query = new(payload);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public async Task<SpellModel?> UpdateAsync(Guid id, UpdateSpellPayload payload, CancellationToken cancellationToken)
  {
    UpdateSpellCommand command = new(id, payload);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }
}
