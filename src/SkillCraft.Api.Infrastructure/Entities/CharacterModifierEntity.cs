using Logitar;
using Logitar.EventSourcing;
using SkillCraft.Api.Core.Characters;
using SkillCraft.Api.Core.Characters.Events;

namespace SkillCraft.Api.Infrastructure.Entities;

internal class CharacterModifierEntity
{
  public int CharacterModifierId { get; private set; }

  public CharacterEntity? Character { get; private set; }
  public int CharacterId { get; private set; }
  public Guid Id { get; private set; }

  public CharacterModifierKind Kind { get; private set; }
  public string Target { get; private set; } = string.Empty;

  public int Value { get; private set; }

  public string? Name { get; private set; }
  public string? Notes { get; private set; }

  public string? CreatedBy { get; private set; }
  public DateTime CreatedOn { get; private set; }
  public string? UpdatedBy { get; private set; }
  public DateTime UpdatedOn { get; private set; }

  public CharacterModifierEntity(CharacterEntity character, CharacterModifierChanged @event)
  {
    Character = character;
    CharacterId = character.CharacterId;
    Id = @event.ModifierId;

    Kind = @event.Modifier.Kind;
    Target = @event.Modifier.Target;

    CreatedBy = @event.ActorId?.Value;
    CreatedOn = @event.OccurredOn.AsUniversalTime();

    Update(@event);
  }

  private CharacterModifierEntity()
  {
  }

  public IReadOnlyCollection<ActorId> GetActorIds()
  {
    HashSet<ActorId> actorIds = new(capacity: 2);
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

  public void Update(CharacterModifierChanged @event)
  {
    Value = @event.Modifier.Value;

    Name = @event.Modifier.Name?.Value;
    Notes = @event.Modifier.Notes?.Value;

    UpdatedBy = @event.ActorId?.Value;
    UpdatedOn = @event.OccurredOn.AsUniversalTime();
  }

  public override bool Equals(object? obj) => obj is CharacterModifierEntity entity && entity.CharacterModifierId == CharacterModifierId;
  public override int GetHashCode() => CharacterModifierId.GetHashCode();
  public override string ToString() => $"{Name ?? string.Join(':', Kind, Target)} | {base.ToString()} (CharacterModifierId={CharacterModifierId})";
}
