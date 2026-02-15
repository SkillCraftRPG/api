using SkillCraft.Api.Contracts;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Educations;
using SkillCraft.Api.Core.Educations.Events;

namespace SkillCraft.Api.Infrastructure.Entities;

internal class EducationEntity : AggregateEntity, IWorldScoped
{
  public int EducationId { get; private set; }

  public WorldEntity? World { get; private set; }
  public int WorldId { get; private set; }
  public Guid WorldUid { get; private set; }

  public Guid Id { get; private set; }

  public string Name { get; private set; } = string.Empty;
  public string? Summary { get; private set; }
  public string? Description { get; private set; }

  public GameSkill? Skill { get; private set; }
  public int? WealthMultiplier { get; private set; }
  public string? FeatureName { get; private set; }
  public string? FeatureDescription { get; private set; }

  public EducationEntity(WorldEntity world, EducationCreated @event) : base(@event)
  {
    Id = new EducationId(@event.StreamId).EntityId;

    World = world;
    WorldId = world.WorldId;
    WorldUid = world.Id;

    Name = @event.Name.Value;
  }

  private EducationEntity() : base()
  {
  }

  public void Update(EducationUpdated @event)
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
    if (@event.WealthMultiplier is not null)
    {
      WealthMultiplier = @event.WealthMultiplier.Value?.Value;
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
