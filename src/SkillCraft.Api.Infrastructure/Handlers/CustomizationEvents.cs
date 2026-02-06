using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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

  public CustomizationEvents(GameContext game)
  {
    _game = game;
  }

  public async Task HandleAsync(CustomizationCreated @event, CancellationToken cancellationToken)
  {
    CustomizationEntity? customization = await _game.Customizations.AsNoTracking().SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (customization is null)
    {
      CustomizationId customizationId = new(@event.StreamId);
      WorldEntity? world = await _game.Worlds.SingleOrDefaultAsync(x => x.StreamId == customizationId.WorldId.Value, cancellationToken);
      if (world is not null)
      {
        customization = new CustomizationEntity(world, @event);
        _game.Customizations.Add(customization);
        await _game.SaveChangesAsync(cancellationToken);
      }
    }
  }

  public async Task HandleAsync(CustomizationDeleted @event, CancellationToken cancellationToken)
  {
    CustomizationEntity? customization = await _game.Customizations.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (customization is not null)
    {
      _game.Customizations.Remove(customization);
      await _game.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task HandleAsync(CustomizationUpdated @event, CancellationToken cancellationToken)
  {
    CustomizationEntity? customization = await _game.Customizations.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (customization is not null && customization.Version == (@event.Version - 1))
    {
      customization.Update(@event);
      await _game.SaveChangesAsync(cancellationToken);
    }
  }
} // TODO(fpion): Logging
