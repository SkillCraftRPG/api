using Logitar.EventSourcing;
using SkillCraft.Api.Core.Talents.Events;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Talents;

public class Talent : AggregateRoot, IResource
{
  public const string ResourceKind = "Talent";

  public new TalentId Id => new(base.Id);
  public WorldId WorldId => Id.WorldId;
  public Guid ResourceId => Id.ResourceId;

  private TalentTier? _tier = null;
  public TalentTier Tier => _tier ?? throw new InvalidOperationException("The tier has not been initialized.");

  private Name? _name = null;
  public Name Name => _name ?? throw new InvalidOperationException("The name has not been initialized.");
  public Summary? Summary { get; private set; }
  public Content? Content { get; private set; }

  public bool AllowMultiplePurchases { get; private set; }
  public Skill? Skill { get; private set; }
  public TalentId? RequiredTalentId { get; private set; }

  public ResourceIdentifier Identifier => new(ResourceKind, ResourceId, WorldId);

  public Talent() : base()
  {
  }

  public Talent(World world, TalentTier tier, Name name, ActorId? actorId = null)
    : this(TalentId.NewId(world.Id), tier, name, actorId)
  {
  }

  public Talent(TalentId talentId, TalentTier tier, Name name, ActorId? actorId = null)
    : base(talentId.StreamId)
  {
    Raise(new TalentCreated(tier, name), actorId);
  }
  protected virtual void Handle(TalentCreated @event)
  {
    _tier = @event.Tier;

    _name = @event.Name;
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new TalentDeleted(), actorId);
    }
  }

  public void Edit(Summary? summary, Content? content, ActorId? actorId = null)
  {
    if (!Equals(Summary, summary) || !Equals(Content, content))
    {
      Raise(new TalentEdited(summary, content), actorId);
    }
  }
  protected virtual void Handle(TalentEdited @event)
  {
    Summary = @event.Summary;
    Content = @event.Content;
  }

  public void Rename(Name name, ActorId? actorId = null)
  {
    if (!Equals(Name, name))
    {
      Raise(new TalentRenamed(name), actorId);
    }
  }
  protected virtual void Handle(TalentRenamed @event)
  {
    _name = @event.Name;
  }

  public void SetRequirements(Talent? talent, ActorId? actorId = null)
  {
    if (talent is not null)
    {
      WorldMismatchException.ThrowIfMismatch(WorldId, talent.WorldId, nameof(talent));
      InvalidRequiredTalentException.ThrowIfNotValid(this, talent);
      // TODO(fpion): should not be the same talent
    }

    if (!Equals(RequiredTalentId, talent?.Id))
    {
      Raise(new TalentRequirementsChanged(talent?.Id), actorId);
    }
  }
  protected virtual void Handle(TalentRequirementsChanged @event)
  {
    RequiredTalentId = @event.TalentId;
  }

  public void SetRules(bool allowMultiplePurchases, Skill? skill, ActorId? actorId = null)
  {
    if (allowMultiplePurchases && skill.HasValue)
    {
      throw new InvalidTalentSkillException(this, skill.Value);
    }
    if (skill.HasValue && !Enum.IsDefined(skill.Value))
    {
      throw new ArgumentOutOfRangeException(nameof(skill));
    }

    if (!Equals(AllowMultiplePurchases, allowMultiplePurchases) || !Equals(Skill, skill))
    {
      Raise(new TalentRulesChanged(allowMultiplePurchases, skill), actorId);
    }
  }
  protected virtual void Handle(TalentRulesChanged @event)
  {
    AllowMultiplePurchases = @event.AllowMultiplePurchases;
    Skill = @event.Skill;
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
