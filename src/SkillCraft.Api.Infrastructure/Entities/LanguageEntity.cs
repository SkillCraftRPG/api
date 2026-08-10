using Logitar;
using Logitar.EventSourcing;
using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Languages.Events;

namespace SkillCraft.Api.Infrastructure.Entities;

internal class LanguageEntity : AggregateEntity
{
  public int LanguageId { get; private set; }

  public WorldEntity? World { get; private set; }
  public Guid WorldId { get; private set; }
  public Guid Id { get; private set; }

  public string Name { get; private set; } = string.Empty;
  public string? Summary { get; private set; }
  public string? Content { get; private set; }

  public string? TypicalSpeakers { get; private set; }

  public ScriptEntity? Script { get; private set; }
  public int? ScriptId { get; private set; }

  public List<CharacterEntity> Characters { get; private set; } = [];
  public List<LineageEntity> Lineages { get; private set; } = [];

  public LanguageEntity(Language language, int? scriptId) : base(language)
  {
    WorldId = language.WorldId.ResourceId;
    Id = language.ResourceId;

    Update(language, scriptId);
  }

  public LanguageEntity(LanguageCreated @event) : base(@event)
  {
    LanguageId languageId = new(@event.StreamId);
    WorldId = languageId.WorldId.ResourceId;
    Id = languageId.ResourceId;

    Name = @event.Name.Value;
  }

  private LanguageEntity() : base()
  {
  }

  public void Edit(LanguageEdited @event)
  {
    base.Update(@event);

    Summary = @event.Summary?.Value;
    Content = @event.Content?.Value;
  }

  public override IReadOnlyCollection<ActorId> GetActorIds()
  {
    HashSet<ActorId> actorIds = new(base.GetActorIds());
    if (Script is not null)
    {
      actorIds.AddRange(Script.GetActorIds());
    }
    return actorIds.AsReadOnly();
  }

  public void Rename(LanguageRenamed @event)
  {
    base.Update(@event);

    Name = @event.Name.Value;
  }

  public void SetRules(int? scriptId, LanguageRulesChanged @event)
  {
    base.Update(@event);

    TypicalSpeakers = @event.TypicalSpeakers?.Value;
    ScriptId = scriptId;
  }

  public void Update(Language language, int? scriptId)
  {
    base.Update(language);

    Name = language.Name.Value;
    Summary = language.Summary?.Value;
    Content = language.Content?.Value;

    TypicalSpeakers = language.TypicalSpeakers?.Value;
    ScriptId = scriptId;
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
