using Krakenar.Contracts.Search;
using Logitar.CQRS;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Contracts.Castes;
using SkillCraft.Api.Core.Castes.Commands;
using SkillCraft.Api.Core.Castes.Queries;

namespace SkillCraft.Api.Core.Castes;

internal class CasteService : ICasteService
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<ICasteService, CasteService>();
    services.AddTransient<ICommandHandler<CreateOrReplaceCasteCommand, CreateOrReplaceCasteResult>, CreateOrReplaceCasteCommandHandler>();
    services.AddTransient<ICommandHandler<DeleteCasteCommand, CasteModel?>, DeleteCasteCommandHandler>();
    services.AddTransient<ICommandHandler<UpdateCasteCommand, CasteModel?>, UpdateCasteCommandHandler>();
    services.AddTransient<IQueryHandler<ReadCasteQuery, CasteModel?>, ReadCasteQueryHandler>();
    services.AddTransient<IQueryHandler<SearchCastesQuery, SearchResults<CasteModel>>, SearchCastesQueryHandler>();
  }

  private readonly ICommandBus _commandBus;
  private readonly IQueryBus _queryBus;

  public CasteService(ICommandBus commandBus, IQueryBus queryBus)
  {
    _commandBus = commandBus;
    _queryBus = queryBus;
  }

  public async Task<CreateOrReplaceCasteResult> CreateOrReplaceAsync(CreateOrReplaceCastePayload payload, Guid? id, CancellationToken cancellationToken)
  {
    CreateOrReplaceCasteCommand command = new(payload, id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<CasteModel?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    DeleteCasteCommand command = new(id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<CasteModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ReadCasteQuery query = new(id);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public async Task<SearchResults<CasteModel>> SearchAsync(SearchCastesPayload payload, CancellationToken cancellationToken)
  {
    SearchCastesQuery query = new(payload);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public async Task<CasteModel?> UpdateAsync(Guid id, UpdateCastePayload payload, CancellationToken cancellationToken)
  {
    UpdateCasteCommand command = new(id, payload);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }
}
