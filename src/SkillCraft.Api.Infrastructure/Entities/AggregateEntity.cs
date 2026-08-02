using Logitar;
using Logitar.EventSourcing;

namespace SkillCraft.Api.Infrastructure.Entities;

internal abstract class AggregateEntity
{
  public string StreamId { get; private set; } = string.Empty;
  public long Version { get; private set; }

  public string? CreatedBy { get; private set; }
  public DateTime CreatedOn { get; private set; }

  public string? UpdatedBy { get; private set; }
  public DateTime UpdatedOn { get; private set; }

  protected AggregateEntity()
  {
  }

  protected AggregateEntity(AggregateRoot aggregate)
  {
    StreamId = aggregate.Id.Value;

    CreatedBy = aggregate.CreatedBy?.Value;
    CreatedOn = aggregate.CreatedOn.AsUniversalTime();

    Update(aggregate);
  }

  protected AggregateEntity(DomainEvent @event)
  {
    StreamId = @event.StreamId.Value;

    CreatedBy = @event.ActorId?.Value;
    CreatedOn = @event.OccurredOn.AsUniversalTime();

    Update(@event);
  }

  public virtual IReadOnlyCollection<ActorId> GetActorIds()
  {
    HashSet<ActorId> actorIds = new(capacity: 2);
    if (CreatedBy is not null)
    {
      actorIds.Add(new ActorId(CreatedBy));
    }
    if (UpdatedBy is not null)
    {
      actorIds.Add(new ActorId(UpdatedBy));
    }
    return actorIds.AsReadOnly();
  }

  public virtual void Update(AggregateRoot aggregate)
  {
    Version = aggregate.Version;

    UpdatedBy = aggregate.UpdatedBy?.Value;
    UpdatedOn = aggregate.UpdatedOn.AsUniversalTime();
  }

  public virtual void Update(DomainEvent @event)
  {
    Version = @event.Version;

    UpdatedBy = @event.ActorId?.Value;
    UpdatedOn = @event.OccurredOn.AsUniversalTime();
  }

  public override bool Equals(object? obj) => obj is AggregateEntity aggregate && aggregate.StreamId == StreamId;
  public override int GetHashCode() => StreamId.GetHashCode();
  public override string ToString() => $"{base.ToString()} (StreamId={StreamId})";
}
