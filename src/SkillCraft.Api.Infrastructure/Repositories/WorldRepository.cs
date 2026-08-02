using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Worlds;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Repositories;

internal class WorldRepository : Logitar.EventSourcing.Repository, IWorldRepository // TODO(fpion): namespace, and other repos
{
  private readonly IContext _context;
  private readonly GameContext _database;

  public WorldRepository(IContext context, GameContext database, IEventStore eventStore) : base(eventStore)
  {
    _context = context;
    _database = database;
  }

  public async Task<World?> LoadAsync(WorldId id, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<World>(id.StreamId, isDeleted: false, cancellationToken);
  }
  public async Task<IReadOnlyCollection<World>> LoadAsync(IEnumerable<WorldId> ids, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<World>(ids.Select(id => id.StreamId), isDeleted: false, cancellationToken);
  }

  public async Task<World> LoadFromContextAsync(CancellationToken cancellationToken)
  {
    WorldId id = _context.WorldId;
    return await LoadAsync(id, cancellationToken) ?? throw new InvalidOperationException($"The world 'Id={id}' was not found.");
  }

  public async Task SaveAsync(World world, CancellationToken cancellationToken)
  {
    await base.SaveAsync(world, cancellationToken);

    await SynchronizeAsync(world, cancellationToken);
  }

  public async Task SaveAsync(IEnumerable<World> worlds, CancellationToken cancellationToken)
  {
    await base.SaveAsync(worlds, cancellationToken);

    foreach (World world in worlds)
    {
      await SynchronizeAsync(world, cancellationToken);
    }
  }

  private async Task SynchronizeAsync(World world, CancellationToken cancellationToken)
  {
    WorldEntity? entity = await _database.Worlds.SingleOrDefaultAsync(x => x.StreamId == world.Id.Value, cancellationToken);
    if (entity is null)
    {
      entity = new WorldEntity(world);
      _database.Worlds.Add(entity);
    }
    else
    {
      entity.Update(world);
    }
    await _database.SaveChangesAsync(cancellationToken);
  }
}
