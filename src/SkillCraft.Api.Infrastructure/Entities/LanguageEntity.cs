using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Languages.Events;

namespace SkillCraft.Api.Infrastructure.Entities;

internal class LanguageEntity : AggregateEntity, IWorldScoped
{
  public int LanguageId { get; private set; }

  public WorldEntity? World { get; private set; }
  public int WorldId { get; private set; }
  public Guid WorldUid { get; private set; }

  public Guid Id { get; private set; }

  public string Name { get; private set; } = string.Empty;
  public string? Summary { get; private set; }
  public string? Description { get; private set; }

  public string? TypicalSpeakers { get; private set; }

  public LanguageEntity(WorldEntity world, LanguageCreated @event) : base(@event)
  {
    Id = new LanguageId(@event.StreamId).EntityId;

    World = world;
    WorldId = world.WorldId;
    WorldUid = world.Id;

    Name = @event.Name.Value;
  }

  private LanguageEntity() : base()
  {
  }

  public void Update(LanguageUpdated @event)
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

    if (@event.TypicalSpeakers is not null)
    {
      TypicalSpeakers = @event.TypicalSpeakers.Value?.Value;
    }
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
