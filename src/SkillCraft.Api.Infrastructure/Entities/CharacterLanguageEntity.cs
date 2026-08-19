using Logitar;
using Logitar.EventSourcing;
using SkillCraft.Api.Core.Characters;
using SkillCraft.Api.Core.Characters.Events;

namespace SkillCraft.Api.Infrastructure.Entities;

internal class CharacterLanguageEntity
{
  public CharacterEntity? Character { get; private set; }
  public int CharacterId { get; private set; }

  public LanguageEntity? Language { get; private set; }
  public int LanguageId { get; private set; }

  public CharacterLanguageSource Source { get; private set; }
  public string? Target { get; private set; }
  public string? Notes { get; private set; }

  public string? CreatedBy { get; private set; }
  public DateTime CreatedOn { get; private set; }
  public string? UpdatedBy { get; private set; }
  public DateTime UpdatedOn { get; private set; }

  public CharacterLanguageEntity(CharacterEntity character, LanguageEntity language, CharacterCreated @event) : this(character, language)
  {
    Source = CharacterLanguageSource.Extra;

    CreatedBy = UpdatedBy = @event.ActorId?.Value;
    CreatedOn = UpdatedOn = @event.OccurredOn.AsUniversalTime();
  }

  public CharacterLanguageEntity(CharacterEntity character, LanguageEntity language, CharacterLanguageChanged @event) : this(character, language)
  {
    Source = @event.Acquisition.Source;
    Target = @event.Acquisition.Target;

    CreatedBy = @event.ActorId?.Value;
    CreatedOn = @event.OccurredOn.AsUniversalTime();

    Update(@event);
  }

  private CharacterLanguageEntity(CharacterEntity character, LanguageEntity language)
  {
    Character = character;
    CharacterId = character.CharacterId;

    Language = language;
    LanguageId = language.LanguageId;
  }

  private CharacterLanguageEntity()
  {
  }

  public IReadOnlyCollection<ActorId> GetActorIds()
  {
    HashSet<ActorId> actorIds = [];
    if (Language is not null)
    {
      actorIds.AddRange(Language.GetActorIds());
    }
    if (CreatedBy is not null)
    {
      actorIds.Add(new ActorId(CreatedBy));
    }
    if (UpdatedBy is not null)
    {
      actorIds.Add(new ActorId(UpdatedBy));
    }
    return actorIds.AsReadOnly();
  }

  public void Update(CharacterLanguageChanged @event)
  {
    Notes = @event.Acquisition.Notes?.Value;

    UpdatedBy = @event.ActorId?.Value;
    UpdatedOn = @event.OccurredOn.AsUniversalTime();
  }

  public override bool Equals(object? obj) => obj is CharacterLanguageEntity entity && entity.CharacterId == CharacterId && entity.LanguageId == LanguageId;
  public override int GetHashCode() => HashCode.Combine(CharacterId, LanguageId);
  public override string ToString() => $"{base.ToString()} (CharacterId={CharacterId}, LanguageId={LanguageId})";
}
