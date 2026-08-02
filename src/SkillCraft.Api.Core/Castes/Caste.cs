using Logitar.EventSourcing;
using SkillCraft.Api.Core.Castes.Events;
using SkillCraft.Api.Core.Features;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Castes;

public class Caste : AggregateRoot, IResource
{
  public const string ResourceKind = "Caste";

  public new CasteId Id => new(base.Id);
  public WorldId WorldId => Id.WorldId;
  public Guid ResourceId => Id.ResourceId;

  private Name? _name = null;
  public Name Name => _name ?? throw new InvalidOperationException("The name has not been initialized.");
  public Summary? Summary { get; private set; }
  public Content? Content { get; private set; }

  public Skill? Skill { get; private set; }
  public Roll? WealthRoll { get; private set; }
  public Feature? Feature { get; private set; }

  public ResourceIdentifier Identifier => new(ResourceKind, ResourceId, WorldId);

  public Caste() : base()
  {
  }

  public Caste(World world, Name name, ActorId? actorId = null)
    : this(CasteId.NewId(world.Id), name, actorId)
  {
  }

  public Caste(CasteId casteId, Name name, ActorId? actorId = null)
    : base(casteId.StreamId)
  {
    Raise(new CasteCreated(name), actorId);
  }
  protected virtual void Handle(CasteCreated @event)
  {
    _name = @event.Name;
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new CasteDeleted(), actorId);
    }
  }

  public void Edit(Summary? summary, Content? content, ActorId? actorId = null)
  {
    if (!Equals(Summary, summary) || !Equals(Content, content))
    {
      Raise(new CasteEdited(summary, content), actorId);
    }
  }
  protected virtual void Handle(CasteEdited @event)
  {
    Summary = @event.Summary;
    Content = @event.Content;
  }

  public void Rename(Name name, ActorId? actorId = null)
  {
    if (!Equals(Name, name))
    {
      Raise(new CasteRenamed(name), actorId);
    }
  }
  protected virtual void Handle(CasteRenamed @event)
  {
    _name = @event.Name;
  }

  public void SetRules(Skill? skill, Roll? wealthRoll, Feature? feature, ActorId? actorId = null)
  {
    if (skill.HasValue && !Enum.IsDefined(skill.Value))
    {
      throw new ArgumentOutOfRangeException(nameof(skill));
    }

    if (!Equals(Skill, skill) || !Equals(WealthRoll, wealthRoll) || !Equals(Feature, feature))
    {
      Raise(new CasteRulesChanged(skill, wealthRoll, feature), actorId);
    }
  }
  protected virtual void Handle(CasteRulesChanged @event)
  {
    Skill = @event.Skill;
    WealthRoll = @event.WealthRoll;
    Feature = @event.Feature;
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
