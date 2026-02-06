using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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

  public WorldEvents(GameContext game)
  {
    _game = game;
  }

  public async Task HandleAsync(WorldCreated @event, CancellationToken cancellationToken)
  {
    WorldEntity? world = await _game.Worlds.AsNoTracking().SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (world is null)
    {
      world = new WorldEntity(@event);
      _game.Worlds.Add(world);
      await _game.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task HandleAsync(WorldDeleted @event, CancellationToken cancellationToken)
  {
    WorldEntity? world = await _game.Worlds.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (world is not null)
    {
      _game.Worlds.Remove(world);
      await _game.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task HandleAsync(WorldUpdated @event, CancellationToken cancellationToken)
  {
    WorldEntity? world = await _game.Worlds.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (world is not null && world.Version == (@event.Version - 1))
    {
      world.Update(@event);
      await _game.SaveChangesAsync(cancellationToken);
    }
  }
} // TODO(fpion): Logging
