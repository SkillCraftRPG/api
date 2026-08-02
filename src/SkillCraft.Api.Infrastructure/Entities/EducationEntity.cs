using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Educations;
using SkillCraft.Api.Core.Educations.Events;
using SkillCraft.Api.Core.Features;

namespace SkillCraft.Api.Infrastructure.Entities;

internal class EducationEntity : AggregateEntity
{
  public int EducationId { get; private set; }

  public WorldEntity? World { get; private set; }
  public Guid WorldId { get; private set; }
  public Guid Id { get; private set; }

  public string Name { get; private set; } = string.Empty;
  public string? Summary { get; private set; }
  public string? Content { get; private set; }

  public Skill? Skill { get; private set; }
  public int? WealthMultiplier { get; private set; }
  public string? FeatureName { get; private set; }
  public string? FeatureContent { get; private set; }

  public EducationEntity(Education education) : base(education)
  {
    WorldId = education.WorldId.ResourceId;
    Id = education.ResourceId;

    Update(education);
  }

  public EducationEntity(EducationCreated @event) : base(@event)
  {
    EducationId educationId = new(@event.StreamId);
    WorldId = educationId.WorldId.ResourceId;
    Id = educationId.ResourceId;

    Name = @event.Name.Value;
  }

  private EducationEntity() : base()
  {
  }

  public void Edit(EducationEdited @event)
  {
    base.Update(@event);

    Summary = @event.Summary?.Value;
    Content = @event.Content?.Value;
  }

  public void Rename(EducationRenamed @event)
  {
    base.Update(@event);

    Name = @event.Name.Value;
  }

  public void SetRules(EducationRulesChanged @event)
  {
    base.Update(@event);

    Skill = @event.Skill;
    WealthMultiplier = @event.WealthMultiplier?.Value;
    SetFeature(@event.Feature);
  }

  public void Update(Education education)
  {
    base.Update(education);

    Name = education.Name.Value;
    Summary = education.Summary?.Value;
    Content = education.Content?.Value;

    Skill = education.Skill;
    WealthMultiplier = education.WealthMultiplier?.Value;
    SetFeature(education.Feature);
  }

  private void SetFeature(Feature? feature)
  {
    FeatureName = feature?.Name.Value;
    FeatureContent = feature?.Content?.Value;
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
