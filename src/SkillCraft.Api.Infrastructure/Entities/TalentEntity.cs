using Logitar;
using Logitar.EventSourcing;
using SkillCraft.Api.Contracts;
using SkillCraft.Api.Core.Talents;
using SkillCraft.Api.Core.Talents.Events;

namespace SkillCraft.Api.Infrastructure.Entities;

internal class TalentEntity : AggregateEntity, IWorldScoped
{
  public int TalentId { get; private set; }

  public WorldEntity? World { get; private set; }
  public int WorldId { get; private set; }
  public Guid WorldUid { get; private set; }

  public Guid Id { get; private set; }

  public int Tier { get; private set; }

  public string Name { get; private set; } = string.Empty;
  public string? Summary { get; private set; }
  public string? Description { get; private set; }

  public bool AllowMultiplePurchases { get; private set; }
  public GameSkill? Skill { get; private set; }

  public TalentEntity? RequiredTalent { get; private set; }
  public int? RequiredTalentId { get; private set; }
  public Guid? RequiredTalentUid { get; private set; }
  public List<TalentEntity> RequiringTalents { get; private set; } = [];

  public List<SpecializationEntity> RequiringSpecializations { get; private set; } = [];

  public TalentEntity(WorldEntity world, TalentCreated @event) : base(@event)
  {
    Id = new TalentId(@event.StreamId).EntityId;

    World = world;
    WorldId = world.WorldId;
    WorldUid = world.Id;

    Tier = @event.Tier.Value;

    Name = @event.Name.Value;
  }

  private TalentEntity() : base()
  {
  }

  public override IReadOnlyCollection<ActorId> GetActorIds()
  {
    HashSet<ActorId> actorIds = new(base.GetActorIds());
    if (RequiredTalent is not null)
    {
      actorIds.AddRange(RequiredTalent.GetActorIds());
    }
    return actorIds;
  }

  public void Update(TalentEntity? requiredTalent, TalentUpdated @event)
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

    if (@event.AllowMultiplePurchases is not null)
    {
      AllowMultiplePurchases = @event.AllowMultiplePurchases.Value;
    }
    if (@event.Skill is not null)
    {
      Skill = @event.Skill.Value;
    }
    if (@event.RequiredTalentId is not null)
    {
      RequiredTalent = requiredTalent;
      RequiredTalentId = requiredTalent?.TalentId;
      RequiredTalentUid = requiredTalent?.Id;
    }
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
