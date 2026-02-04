using Krakenar.Contracts.Search;
using Logitar.CQRS;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Contracts.Customizations;
using SkillCraft.Api.Core.Customizations.Commands;
using SkillCraft.Api.Core.Customizations.Queries;

namespace SkillCraft.Api.Core.Customizations;

internal class CustomizationService : ICustomizationService
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<ICustomizationService, CustomizationService>();
    services.AddTransient<ICommandHandler<CreateOrReplaceCustomizationCommand, CreateOrReplaceCustomizationResult>, CreateOrReplaceCustomizationCommandHandler>();
    services.AddTransient<ICommandHandler<DeleteCustomizationCommand, CustomizationModel?>, DeleteCustomizationCommandHandler>();
    services.AddTransient<ICommandHandler<UpdateCustomizationCommand, CustomizationModel?>, UpdateCustomizationCommandHandler>();
    services.AddTransient<IQueryHandler<ReadCustomizationQuery, CustomizationModel?>, ReadCustomizationQueryHandler>();
    services.AddTransient<IQueryHandler<SearchCustomizationsQuery, SearchResults<CustomizationModel>>, SearchCustomizationsQueryHandler>();
  }

  private readonly ICommandBus _commandBus;
  private readonly IQueryBus _queryBus;

  public CustomizationService(ICommandBus commandBus, IQueryBus queryBus)
  {
    _commandBus = commandBus;
    _queryBus = queryBus;
  }

  public async Task<CreateOrReplaceCustomizationResult> CreateOrReplaceAsync(CreateOrReplaceCustomizationPayload payload, Guid? id, CancellationToken cancellationToken)
  {
    CreateOrReplaceCustomizationCommand command = new(payload, id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<CustomizationModel?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    DeleteCustomizationCommand command = new(id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<CustomizationModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ReadCustomizationQuery query = new(id);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public async Task<SearchResults<CustomizationModel>> SearchAsync(SearchCustomizationsPayload payload, CancellationToken cancellationToken)
  {
    SearchCustomizationsQuery query = new(payload);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public async Task<CustomizationModel?> UpdateAsync(Guid id, UpdateCustomizationPayload payload, CancellationToken cancellationToken)
  {
    UpdateCustomizationCommand command = new(id, payload);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }
}
