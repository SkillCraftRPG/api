using SkillCraft.Api.Contracts.Customizations;
using SkillCraft.Api.Core.Customizations.Events;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Infrastructure.Entities;

internal class CustomizationEntity : AggregateEntity, IWorldScoped
{
  public int CustomizationId { get; private set; }

  public WorldEntity? World { get; private set; }
  public int WorldId { get; private set; }
  public Guid WorldUid { get; private set; }

  public Guid Id { get; private set; }

  public CustomizationKind Kind { get; private set; }

  public string Name { get; private set; } = string.Empty;
  public string? Summary { get; private set; }
  public string? Description { get; private set; }

  public CustomizationEntity(WorldEntity world, CustomizationCreated @event) : base(@event)
  {
    Id = new WorldId(@event.StreamId).ToGuid();

    World = world;
    WorldId = world.WorldId;
    WorldUid = world.Id;

    Kind = @event.Kind;

    Name = @event.Name.Value;
  }

  private CustomizationEntity() : base()
  {
  }

  public void Update(CustomizationUpdated @event)
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
