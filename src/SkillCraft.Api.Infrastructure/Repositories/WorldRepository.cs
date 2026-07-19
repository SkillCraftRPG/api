using Krakenar.Contracts.Actors;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Worlds;
using SkillCraft.Api.Core.Worlds.Models;
using SkillCraft.Api.Infrastructure.Actors;

namespace SkillCraft.Api.Infrastructure.Repositories;

internal class WorldRepository : IWorldRepository
{
  private readonly IActorService _actorService;
  private readonly IContext _context;
  private readonly GameContext _game;

  public WorldRepository(IActorService actorService, IContext context, GameContext game)
  {
    _actorService = actorService;
    _context = context;
    _game = game;
  }

  public void Add(World world)
  {
    _game.Worlds.Add(world);
  }
  public void Remove(World world)
  {
    _game.Worlds.Remove(world);
  }
  public void Update(World world)
  {
    _game.Worlds.Update(world);
  }

  public async Task EnsureUnicityAsync(World world, CancellationToken cancellationToken)
  {
  }

  public async Task<World?> LoadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _game.Worlds.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
  }

  public async Task<WorldModel> ReadAsync(World world, CancellationToken cancellationToken)
  {
    return await ReadAsync(world.Id, cancellationToken) ?? throw new InvalidOperationException($"The world 'Id={world.Id}' was not found.");
  }
  public async Task<WorldModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    World? world = await _game.Worlds.AsNoTracking()
      .Where(x => x.Id == id && x.OwnerId == _context.UserId)
      .SingleOrDefaultAsync(cancellationToken);

    return world is null ? null : await MapAsync(world, cancellationToken);
  }
  public async Task<WorldModel?> ReadAsync(string key, CancellationToken cancellationToken)
  {
    World? world = await _game.Worlds.AsNoTracking()
      .Where(x => x.Key == SlugHelper.Format(key) && x.OwnerId == _context.UserId)
      .SingleOrDefaultAsync(cancellationToken);

    return world is null ? null : await MapAsync(world, cancellationToken);
  }

  private async Task<WorldModel> MapAsync(World world, CancellationToken cancellationToken)
  {
    return (await MapAsync([world], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<WorldModel>> MapAsync(IEnumerable<World> worlds, CancellationToken cancellationToken)
  {
    IEnumerable<Guid> userIds = worlds.SelectMany(world => world.GetUserIds());
    IReadOnlyDictionary<Guid, Actor> actors = await _actorService.FindAsync(userIds, cancellationToken);
    Mapper mapper = new(actors);

    return worlds.Select(mapper.ToWorld).ToList().AsReadOnly();
  }
}
