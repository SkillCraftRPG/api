using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SkillCraft.Api.Core.Worlds.Events;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Handlers;

internal class WorldEvents : IEventHandler<WorldCreated>, IEventHandler<WorldDeleted>, IEventHandler<WorldUpdated>
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IEventHandler<WorldCreated>, WorldEvents>();
    services.AddTransient<IEventHandler<WorldDeleted>, WorldEvents>();
    services.AddTransient<IEventHandler<WorldUpdated>, WorldEvents>();
  }

  private readonly GameContext _game;
  private readonly ILogger<WorldEvents> _logger;

  public WorldEvents(GameContext game, ILogger<WorldEvents> logger)
  {
    _game = game;
    _logger = logger;
  }

  public async Task HandleAsync(WorldCreated @event, CancellationToken cancellationToken)
  {
    try
    {
      WorldEntity? world = await _game.Worlds.AsNoTracking().SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      if (world is not null)
      {
        throw new InvalidOperationException($"The world entity '{world}' was expected not to exist, but was found at version {world.Version}.");
      }

      world = new WorldEntity(@event);
      _game.Worlds.Add(world);
      await _game.SaveChangesAsync(cancellationToken);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, @event);
    }
  }

  public async Task HandleAsync(WorldDeleted @event, CancellationToken cancellationToken)
  {
    try
    {
      long expectedVersion = @event.Version - 1;
      WorldEntity? world = await _game.Worlds.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      if (world is null)
      {
        throw new InvalidOperationException($"The world entity 'StreamId={@event.StreamId}' was expected to be found at version {expectedVersion}, but was not found.");
      }
      else if (world.Version != expectedVersion)
      {
        throw new InvalidOperationException($"The world entity '{world}' was expected to be found at version {expectedVersion}, but was found at version {world.Version}.");
      }

      _game.Worlds.Remove(world);
      await _game.SaveChangesAsync(cancellationToken);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, @event);
    }
  }

  public async Task HandleAsync(WorldUpdated @event, CancellationToken cancellationToken)
  {
    try
    {
      long expectedVersion = @event.Version - 1;
      WorldEntity? world = await _game.Worlds.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      if (world is null)
      {
        throw new InvalidOperationException($"The world entity 'StreamId={@event.StreamId}' was expected to be found at version {expectedVersion}, but was not found.");
      }
      else if (world.Version != expectedVersion)
      {
        throw new InvalidOperationException($"The world entity '{world}' was expected to be found at version {expectedVersion}, but was found at version {world.Version}.");
      }

      world.Update(@event);
      await _game.SaveChangesAsync(cancellationToken);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, @event);
    }
  }
}
