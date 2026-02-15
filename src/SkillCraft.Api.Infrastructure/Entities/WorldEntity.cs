using Logitar.EventSourcing;
using SkillCraft.Api.Core.Worlds;
using SkillCraft.Api.Core.Worlds.Events;

namespace SkillCraft.Api.Infrastructure.Entities;

internal class WorldEntity : AggregateEntity
{
  public int WorldId { get; private set; }
  public Guid Id { get; private set; }

  public string OwnerId { get; private set; } = string.Empty;

  public string Name { get; private set; } = string.Empty;
  public string? Description { get; private set; }

  public List<CasteEntity> Castes { get; private set; } = [];
  public List<CustomizationEntity> Customizations { get; private set; } = [];
  public List<EducationEntity> Educations { get; private set; } = [];
  public List<PartyEntity> Parties { get; private set; } = [];
  public StorageSummaryEntity? StorageSummary { get; private set; }

  public WorldEntity(WorldCreated @event) : base(@event)
  {
    Id = new WorldId(@event.StreamId).ToGuid();

    OwnerId = @event.OwnerId.Value;

    Name = @event.Name.Value;
  }

  private WorldEntity() : base()
  {
  }

  public override IReadOnlyCollection<ActorId> GetActorIds()
  {
    HashSet<ActorId> actorIds = new(base.GetActorIds());
    actorIds.Add(new ActorId(OwnerId));
    return actorIds;
  }

  public void Update(WorldUpdated @event)
  {
    base.Update(@event);

    if (@event.Name is not null)
    {
      Name = @event.Name.Value;
    }
    if (@event.Description is not null)
    {
      Description = @event.Description.Value?.Value;
    }
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
