using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SkillCraft.Api.Core.Storages;
using SkillCraft.Api.Core.Storages.Events;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Handlers;

internal class StorageEvents : IEventHandler<EntityStored>, IEventHandler<StorageDeleted>, IEventHandler<StorageInitialized>
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IEventHandler<EntityStored>, StorageEvents>();
    services.AddTransient<IEventHandler<StorageDeleted>, StorageEvents>();
    services.AddTransient<IEventHandler<StorageInitialized>, StorageEvents>();
  }

  private readonly GameContext _game;
  private readonly ILogger<StorageEvents> _logger;

  public StorageEvents(GameContext game, ILogger<StorageEvents> logger)
  {
    _game = game;
    _logger = logger;
  }

  public async Task HandleAsync(EntityStored @event, CancellationToken cancellationToken)
  {
    try
    {
      long expectedVersion = @event.Version - 1;
      StorageSummaryEntity? storage = await _game.StorageSummary
        .Include(x => x.Detail)
        .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      if (storage is null)
      {
        throw new InvalidOperationException($"The storage entity 'StreamId={@event.StreamId}' was expected to be found at version {expectedVersion}, but was not found.");
      }
      else if (storage.Version != expectedVersion)
      {
        throw new InvalidOperationException($"The storage entity '{storage}' was expected to be found at version {expectedVersion}, but was found at version {storage.Version}.");
      }

      storage.Store(@event);
      await _game.SaveChangesAsync(cancellationToken);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, @event);
    }
  }

  public async Task HandleAsync(StorageDeleted @event, CancellationToken cancellationToken)
  {
    try
    {
      long expectedVersion = @event.Version - 1;
      StorageSummaryEntity? storage = await _game.StorageSummary.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      if (storage is null)
      {
        throw new InvalidOperationException($"The storage summary entity 'StreamId={@event.StreamId}' was expected to be found at version {expectedVersion}, but was not found.");
      }
      else if (storage.Version != expectedVersion)
      {
        throw new InvalidOperationException($"The storage summary entity '{storage}' was expected to be found at version {expectedVersion}, but was found at version {storage.Version}.");
      }

      _game.StorageSummary.Remove(storage);
      await _game.SaveChangesAsync(cancellationToken);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, @event);
    }
  }

  public async Task HandleAsync(StorageInitialized @event, CancellationToken cancellationToken)
  {
    try
    {
      StorageSummaryEntity? storage = await _game.StorageSummary.AsNoTracking().SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      if (storage is not null)
      {
        throw new InvalidOperationException($"The storage summary entity '{storage}' was expected not to exist, but was found at version {storage.Version}.");
      }

      StorageId storageId = new(@event.StreamId);
      WorldEntity world = await _game.Worlds.SingleOrDefaultAsync(x => x.StreamId == storageId.WorldId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The world entity 'StreamId={storageId.WorldId}' was not found.");

      storage = new StorageSummaryEntity(world, @event);
      _game.StorageSummary.Add(storage);
      await _game.SaveChangesAsync(cancellationToken);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, @event);
    }
  }
}
