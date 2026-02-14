using Krakenar.Contracts;
using Krakenar.Contracts.Actors;
using Logitar;
using Logitar.EventSourcing;
using SkillCraft.Api.Contracts.Customizations;
using SkillCraft.Api.Contracts.Worlds;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure;

internal class GameMapper
{
  private readonly Dictionary<ActorId, Actor> _actors = [];
  private readonly Actor _system = new();

  public GameMapper()
  {
  }

  public GameMapper(IEnumerable<KeyValuePair<ActorId, Actor>> actors)
  {
    foreach (KeyValuePair<ActorId, Actor> actor in actors)
    {
      _actors[actor.Key] = actor.Value;
    }
  }

  public CustomizationModel ToCustomization(CustomizationEntity source)
  {
    CustomizationModel destination = new()
    {
      Id = source.Id,
      Kind = source.Kind,
      Name = source.Name,
      Summary = source.Summary,
      Description = source.Description
    };

    MapAggregate(source, destination);

    return destination;
  }

  public WorldModel ToWorld(WorldEntity source)
  {
    WorldModel destination = new()
    {
      Id = source.Id,
      Name = source.Name,
      Description = source.Description,
      Owner = FindActor(source.OwnerId)
    };

    MapAggregate(source, destination);

    return destination;
  }

  private void MapAggregate(AggregateEntity source, Aggregate destination)
  {
    destination.Version = source.Version;

    destination.CreatedBy = FindActor(source.CreatedBy);
    destination.CreatedOn = source.CreatedOn.AsUniversalTime();

    destination.UpdatedBy = FindActor(source.UpdatedBy);
    destination.UpdatedOn = source.UpdatedOn.AsUniversalTime();
  }

  private Actor FindActor(string? id) => (string.IsNullOrWhiteSpace(id) ? null : TryFindActor(id)) ?? _system;
  private Actor FindActor(ActorId? id) => (id.HasValue ? TryFindActor(id.Value) : null) ?? _system;
  private Actor? TryFindActor(string? id) => string.IsNullOrWhiteSpace(id) ? null : TryFindActor(new ActorId(id));
  private Actor? TryFindActor(ActorId? id) => id.HasValue && _actors.TryGetValue(id.Value, out Actor? actor) ? actor : null;
}
