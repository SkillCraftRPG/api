using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Castes;
using SkillCraft.Api.Core.Castes.Events;
using SkillCraft.Api.Core.Features;

namespace SkillCraft.Api.Infrastructure.Entities;

internal class CasteEntity : AggregateEntity
{
  public int CasteId { get; private set; }

  public WorldEntity? World { get; private set; }
  public Guid WorldId { get; private set; }
  public Guid Id { get; private set; }

  public string Name { get; private set; } = string.Empty;
  public string? Summary { get; private set; }
  public string? Content { get; private set; }

  public Skill? Skill { get; private set; }
  public string? WealthRoll { get; private set; }
  public string? FeatureName { get; private set; }
  public string? FeatureContent { get; private set; }

  public List<CharacterEntity> Characters { get; private set; } = [];

  public CasteEntity(Caste caste) : base(caste)
  {
    WorldId = caste.WorldId.ResourceId;
    Id = caste.ResourceId;

    Update(caste);
  }

  public CasteEntity(CasteCreated @event) : base(@event)
  {
    CasteId casteId = new(@event.StreamId);
    WorldId = casteId.WorldId.ResourceId;
    Id = casteId.ResourceId;

    Name = @event.Name.Value;
  }

  private CasteEntity() : base()
  {
  }

  public void Edit(CasteEdited @event)
  {
    base.Update(@event);

    Summary = @event.Summary?.Value;
    Content = @event.Content?.Value;
  }

  public void Rename(CasteRenamed @event)
  {
    base.Update(@event);

    Name = @event.Name.Value;
  }

  public void SetRules(CasteRulesChanged @event)
  {
    base.Update(@event);

    Skill = @event.Skill;
    WealthRoll = @event.WealthRoll?.Value;
    SetFeature(@event.Feature);
  }

  public void Update(Caste caste)
  {
    base.Update(caste);

    Name = caste.Name.Value;
    Summary = caste.Summary?.Value;
    Content = caste.Content?.Value;

    Skill = caste.Skill;
    WealthRoll = caste.WealthRoll?.Value;
    SetFeature(caste.Feature);
  }

  private void SetFeature(Feature? feature)
  {
    FeatureName = feature?.Name.Value;
    FeatureContent = feature?.Content?.Value;
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
