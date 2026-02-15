using SkillCraft.Api.Core.Parties;
using SkillCraft.Api.Core.Parties.Events;

namespace SkillCraft.Api.Infrastructure.Entities;

internal class PartyEntity : AggregateEntity, IWorldScoped
{
  public int PartyId { get; private set; }

  public WorldEntity? World { get; private set; }
  public int WorldId { get; private set; }
  public Guid WorldUid { get; private set; }

  public Guid Id { get; private set; }

  public string Name { get; private set; } = string.Empty;
  public string? Description { get; private set; }

  public PartyEntity(WorldEntity world, PartyCreated @event) : base(@event)
  {
    Id = new PartyId(@event.StreamId).EntityId;

    World = world;
    WorldId = world.WorldId;
    WorldUid = world.Id;

    Name = @event.Name.Value;
  }

  private PartyEntity() : base()
  {
  }

  public void Update(PartyUpdated @event)
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
