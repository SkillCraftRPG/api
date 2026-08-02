using Logitar.EventSourcing;
using SkillCraft.Api.Core.Languages.Events;
using SkillCraft.Api.Core.Scripts;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Languages;

public class Language : AggregateRoot, IResource
{
  public const string ResourceKind = "Language";

  public new LanguageId Id => new(base.Id);
  public WorldId WorldId => Id.WorldId;
  public Guid ResourceId => Id.ResourceId;

  private Name? _name = null;
  public Name Name => _name ?? throw new InvalidOperationException("The name has not been initialized.");
  public Summary? Summary { get; private set; }
  public Content? Content { get; private set; }

  public TypicalSpeakers? TypicalSpeakers { get; private set; }
  public ScriptId? ScriptId { get; private set; }

  public ResourceIdentifier Identifier => new(ResourceKind, ResourceId, WorldId);

  public Language() : base()
  {
  }

  public Language(World world, Name name, ActorId? actorId = null)
    : this(LanguageId.NewId(world.Id), name, actorId)
  {
  }

  public Language(LanguageId languageId, Name name, ActorId? actorId = null)
    : base(languageId.StreamId)
  {
    Raise(new LanguageCreated(name), actorId);
  }
  protected virtual void Handle(LanguageCreated @event)
  {
    _name = @event.Name;
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new LanguageDeleted(), actorId);
    }
  }

  public void Edit(Summary? summary, Content? content, ActorId? actorId = null)
  {
    if (!Equals(Summary, summary) || !Equals(Content, content))
    {
      Raise(new LanguageEdited(summary, content), actorId);
    }
  }
  protected virtual void Handle(LanguageEdited @event)
  {
    Summary = @event.Summary;
    Content = @event.Content;
  }

  public void Rename(Name name, ActorId? actorId = null)
  {
    if (!Equals(Name, name))
    {
      Raise(new LanguageRenamed(name), actorId);
    }
  }
  protected virtual void Handle(LanguageRenamed @event)
  {
    _name = @event.Name;
  }

  public void SetRules(TypicalSpeakers? typicalSpeakers, Script? script, ActorId? actorId = null)
  {
    if (script is not null)
    {
      WorldMismatchException.ThrowIfMismatch(WorldId, script.WorldId, nameof(script));
    }

    if (!Equals(TypicalSpeakers, typicalSpeakers) || !Equals(ScriptId, script?.Id))
    {
      Raise(new LanguageRulesChanged(typicalSpeakers, script?.Id), actorId);
    }
  }
  protected virtual void Handle(LanguageRulesChanged @event)
  {
    TypicalSpeakers = @event.TypicalSpeakers;
    ScriptId = @event.ScriptId;
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
