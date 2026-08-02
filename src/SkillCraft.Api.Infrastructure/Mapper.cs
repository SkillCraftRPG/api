using Krakenar.Contracts;
using Krakenar.Contracts.Actors;
using Logitar;
using Logitar.EventSourcing;
using SkillCraft.Api.Core.Customizations.Models;
using SkillCraft.Api.Core.Talents.Models;
using SkillCraft.Api.Core.Worlds.Models;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure;

internal class Mapper
{
  private readonly Dictionary<ActorId, Actor> _actors = [];
  private readonly Actor _system = new();

  public Mapper()
  {
  }

  public Mapper(IEnumerable<KeyValuePair<ActorId, Actor>> actors)
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
      Content = source.Content
    };

    MapAggregate(source, destination);

    return destination;
  }

  public TalentModel ToTalent(TalentEntity source)
  {
    TalentModel destination = new()
    {
      Id = source.Id,
      Tier = source.Tier,
      Name = source.Name,
      Summary = source.Summary,
      Content = source.Content,
      AllowMultiplePurchases = source.AllowMultiplePurchases,
      Skill = source.Skill
    };

    if (source.RequiredTalent is not null)
    {
      destination.RequiredTalent = ToTalent(source.RequiredTalent);
    }

    MapAggregate(source, destination);

    return destination;
  }

  public WorldModel ToWorld(WorldEntity source)
  {
    WorldModel destination = new()
    {
      Id = source.Id,
      Owner = FindActor(source.OwnerId),
      Key = source.Key,
      Name = source.Name,
      Content = source.Content
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

  private Actor FindActor(string? id) => TryFindActor(id) ?? _system;
  private Actor FindActor(ActorId? id) => TryFindActor(id) ?? _system;
  private Actor? TryFindActor(string? id) => TryFindActor(id is null ? null : new ActorId(id));
  private Actor? TryFindActor(ActorId? id) => id.HasValue ? (_actors.TryGetValue(id.Value, out Actor? actor) ? actor : null) : null;
}
