using SkillCraft.Api.Contracts;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Castes;
using SkillCraft.Api.Core.Castes.Events;

namespace SkillCraft.Api.Infrastructure.Entities;

internal class CasteEntity : AggregateEntity, IWorldScoped
{
  public int CasteId { get; private set; }

  public WorldEntity? World { get; private set; }
  public int WorldId { get; private set; }
  public Guid WorldUid { get; private set; }

  public Guid Id { get; private set; }

  public string Name { get; private set; } = string.Empty;
  public string? Summary { get; private set; }
  public string? Description { get; private set; }

  public GameSkill? Skill { get; private set; }
  public string? WealthRoll { get; private set; }
  public string? FeatureName { get; private set; }
  public string? FeatureDescription { get; private set; }

  public CasteEntity(WorldEntity world, CasteCreated @event) : base(@event)
  {
    Id = new CasteId(@event.StreamId).EntityId;

    World = world;
    WorldId = world.WorldId;
    WorldUid = world.Id;

    Name = @event.Name.Value;
  }

  private CasteEntity() : base()
  {
  }

  public void Update(CasteUpdated @event)
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

    if (@event.Skill is not null)
    {
      Skill = @event.Skill.Value;
    }
    if (@event.WealthRoll is not null)
    {
      WealthRoll = @event.WealthRoll.Value?.Value;
    }
    if (@event.Feature is not null)
    {
      SetFeature(@event.Feature.Value);
    }
  }

  private void SetFeature(Feature? feature)
  {
    FeatureName = feature?.Name.Value;
    FeatureDescription = feature?.Description?.Value;
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
