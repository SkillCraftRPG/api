using Logitar;
using Logitar.EventSourcing;
using SkillCraft.Api.Core.Lineages;
using SkillCraft.Api.Core.Lineages.Events;

namespace SkillCraft.Api.Infrastructure.Entities;

internal class LineageEntity : AggregateEntity, IWorldScoped
{
  public int LineageId { get; private set; }

  public WorldEntity? World { get; private set; }
  public int WorldId { get; private set; }
  public Guid WorldUid { get; private set; }

  public Guid Id { get; private set; }

  public LineageEntity? Parent { get; private set; }
  public int? ParentId { get; private set; }
  public Guid? ParentUid { get; private set; }
  public List<LineageEntity> Children { get; private set; } = [];

  public string Name { get; private set; } = string.Empty;
  public string? Summary { get; private set; }
  public string? Description { get; private set; }

  public LineageEntity(WorldEntity world, LineageEntity? parent, LineageCreated @event) : base(@event)
  {
    Id = new LineageId(@event.StreamId).EntityId;

    World = world;
    WorldId = world.WorldId;
    WorldUid = world.Id;

    Parent = parent;
    ParentId = parent?.LineageId;
    ParentUid = parent?.Id;

    Name = @event.Name.Value;
  }

  private LineageEntity() : base()
  {
  }

  public override IReadOnlyCollection<ActorId> GetActorIds()
  {
    HashSet<ActorId> actorIds = new(base.GetActorIds());
    if (Parent is not null)
    {
      actorIds.AddRange(Parent.GetActorIds());
    }
    return actorIds;
  }

  public void Update(LineageUpdated @event)
  {
    base.Update(@event);

    if (@event.Name is not null)
    {
      Name = @event.Name.Value;
    }
    if (@event.Summary is not null)
    {
      Summary = @event.Summary.Value?.Value;
    }
    if (@event.Description is not null)
    {
      Description = @event.Description.Value?.Value;
    }
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
