using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Talents;
using SkillCraft.Api.Core.Talents.Events;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Infrastructure.Entities;

internal class TalentEntity : AggregateEntity
{
  public int TalentId { get; private set; }

  public World? World { get; private set; }
  public Guid WorldId { get; private set; }
  public Guid Id { get; private set; }

  public int Tier { get; private set; }

  public string Name { get; private set; } = string.Empty;
  public string? Summary { get; private set; }
  public string? Content { get; private set; }

  public bool AllowMultiplePurchases { get; private set; }
  public Skill? Skill { get; private set; }

  public TalentEntity? RequiredTalent { get; private set; }
  public int? RequiredTalentId { get; private set; }
  public List<TalentEntity> RequiringTalents { get; private set; } = [];

  public TalentEntity(Talent talent, int? requiredTalentId) : base(talent)
  {
    WorldId = talent.WorldId.ResourceId;
    Id = talent.ResourceId;

    Tier = talent.Tier.Value;

    Update(talent, requiredTalentId);
  }

  public TalentEntity(TalentCreated @event) : base(@event)
  {
    TalentId talentId = new(@event.StreamId);
    WorldId = talentId.WorldId.ResourceId;
    Id = talentId.ResourceId;

    Tier = @event.Tier.Value;

    Name = @event.Name.Value;
  }

  private TalentEntity() : base()
  {
  }

  public void Edit(TalentEdited @event)
  {
    base.Update(@event);

    Summary = @event.Summary?.Value;
    Content = @event.Content?.Value;
  }

  public void Rename(TalentRenamed @event)
  {
    base.Update(@event);

    Name = @event.Name.Value;
  }

  public void SetRequirements(int? requiredTalentId, TalentRequirementsChanged @event)
  {
    base.Update(@event);

    RequiredTalentId = requiredTalentId;
  }

  public void SetRules(TalentRulesChanged @event)
  {
    base.Update(@event);

    AllowMultiplePurchases = @event.AllowMultiplePurchases;
    Skill = @event.Skill;
  }

  public void Update(Talent talent, int? requiredTalentId)
  {
    base.Update(talent);

    Name = talent.Name.Value;
    Summary = talent.Summary?.Value;
    Content = talent.Content?.Value;

    AllowMultiplePurchases = talent.AllowMultiplePurchases;
    Skill = talent.Skill;
    RequiredTalentId = requiredTalentId;
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
