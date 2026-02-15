using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SkillCraft.Api.Core.Customizations;
using SkillCraft.Api.Core.Customizations.Events;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Handlers;

internal class CustomizationEvents : IEventHandler<CustomizationCreated>, IEventHandler<CustomizationDeleted>, IEventHandler<CustomizationUpdated>
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IEventHandler<CustomizationCreated>, CustomizationEvents>();
    services.AddTransient<IEventHandler<CustomizationDeleted>, CustomizationEvents>();
    services.AddTransient<IEventHandler<CustomizationUpdated>, CustomizationEvents>();
  }

  private readonly GameContext _game;
  private readonly ILogger<CustomizationEvents> _logger;

  public CustomizationEvents(GameContext game, ILogger<CustomizationEvents> logger)
  {
    _game = game;
    _logger = logger;
  }

  public async Task HandleAsync(CustomizationCreated @event, CancellationToken cancellationToken)
  {
    try
    {
      CustomizationEntity? customization = await _game.Customizations.AsNoTracking().SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      if (customization is not null)
      {
        throw new InvalidOperationException($"The customization entity '{customization}' was expected not to exist, but was found at version {customization.Version}.");
      }

      CustomizationId customizationId = new(@event.StreamId);
      WorldEntity world = await _game.Worlds.SingleOrDefaultAsync(x => x.StreamId == customizationId.WorldId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The world entity 'StreamId={customizationId.WorldId}' was not found.");

      customization = new CustomizationEntity(world, @event);
      _game.Customizations.Add(customization);
      await _game.SaveChangesAsync(cancellationToken);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, @event);
    }
  }

  public async Task HandleAsync(CustomizationDeleted @event, CancellationToken cancellationToken)
  {
    try
    {
      long expectedVersion = @event.Version - 1;
      CustomizationEntity? customization = await _game.Customizations.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      if (customization is null)
      {
        throw new InvalidOperationException($"The customization entity 'StreamId={@event.StreamId}' was expected to be found at version {expectedVersion}, but was not found.");
      }
      else if (customization.Version != expectedVersion)
      {
        throw new InvalidOperationException($"The customization entity '{customization}' was expected to be found at version {expectedVersion}, but was found at version {customization.Version}.");
      }

      _game.Customizations.Remove(customization);
      await _game.SaveChangesAsync(cancellationToken);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, @event);
    }
  }

  public async Task HandleAsync(CustomizationUpdated @event, CancellationToken cancellationToken)
  {
    try
    {
      long expectedVersion = @event.Version - 1;
      CustomizationEntity? customization = await _game.Customizations.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      if (customization is null)
      {
        throw new InvalidOperationException($"The customization entity 'StreamId={@event.StreamId}' was expected to be found at version {expectedVersion}, but was not found.");
      }
      else if (customization.Version != expectedVersion)
      {
        throw new InvalidOperationException($"The customization entity '{customization}' was expected to be found at version {expectedVersion}, but was found at version {customization.Version}.");
      }

      customization.Update(@event);
      await _game.SaveChangesAsync(cancellationToken);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, @event);
    }
  }
}
