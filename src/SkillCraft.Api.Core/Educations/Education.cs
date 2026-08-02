using Logitar.EventSourcing;
using SkillCraft.Api.Core.Educations.Events;
using SkillCraft.Api.Core.Features;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Educations;

public class Education : AggregateRoot, IResource
{
  public const string ResourceKind = "Education";

  public new EducationId Id => new(base.Id);
  public WorldId WorldId => Id.WorldId;
  public Guid ResourceId => Id.ResourceId;

  private Name? _name = null;
  public Name Name => _name ?? throw new InvalidOperationException("The name has not been initialized.");
  public Summary? Summary { get; private set; }
  public Content? Content { get; private set; }

  public Skill? Skill { get; private set; }
  public WealthMultiplier? WealthMultiplier { get; private set; }
  public Feature? Feature { get; private set; }

  public ResourceIdentifier Identifier => new(ResourceKind, ResourceId, WorldId);

  public Education() : base()
  {
  }

  public Education(World world, Name name, ActorId? actorId = null)
    : this(EducationId.NewId(world.Id), name, actorId)
  {
  }

  public Education(EducationId educationId, Name name, ActorId? actorId = null)
    : base(educationId.StreamId)
  {
    Raise(new EducationCreated(name), actorId);
  }
  protected virtual void Handle(EducationCreated @event)
  {
    _name = @event.Name;
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new EducationDeleted(), actorId);
    }
  }

  public void Edit(Summary? summary, Content? content, ActorId? actorId = null)
  {
    if (!Equals(Summary, summary) || !Equals(Content, content))
    {
      Raise(new EducationEdited(summary, content), actorId);
    }
  }
  protected virtual void Handle(EducationEdited @event)
  {
    Summary = @event.Summary;
    Content = @event.Content;
  }

  public void Rename(Name name, ActorId? actorId = null)
  {
    if (!Equals(Name, name))
    {
      Raise(new EducationRenamed(name), actorId);
    }
  }
  protected virtual void Handle(EducationRenamed @event)
  {
    _name = @event.Name;
  }

  public void SetRules(Skill? skill, WealthMultiplier? wealthMultiplier, Feature? feature, ActorId? actorId = null)
  {
    if (skill.HasValue && !Enum.IsDefined(skill.Value))
    {
      throw new ArgumentOutOfRangeException(nameof(skill));
    }

    if (!Equals(Skill, skill) || !Equals(WealthMultiplier, wealthMultiplier) || !Equals(Feature, feature))
    {
      Raise(new EducationRulesChanged(skill, wealthMultiplier, feature), actorId);
    }
  }
  protected virtual void Handle(EducationRulesChanged @event)
  {
    Skill = @event.Skill;
    WealthMultiplier = @event.WealthMultiplier;
    Feature = @event.Feature;
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
