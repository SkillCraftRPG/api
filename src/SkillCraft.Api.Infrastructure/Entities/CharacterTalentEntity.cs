using Logitar;
using Logitar.EventSourcing;
using SkillCraft.Api.Core.Characters;

namespace SkillCraft.Api.Infrastructure.Entities;

internal class CharacterTalentEntity
{
  private static readonly JsonSerializerOptions _serializerOptions = new(); // TODO(fpion): remove this
  static CharacterTalentEntity()
  {
    _serializerOptions.Converters.Add(new JsonStringEnumConverter());
  }

  public int CharacterTalentId { get; private set; }

  public CharacterEntity? Character { get; private set; }
  public int CharacterId { get; private set; }
  public Guid Id { get; private set; }

  public TalentEntity? Talent { get; private set; }
  public int TalentId { get; private set; }

  public string? Qualifier { get; private set; }
  public string? Notes { get; private set; }
  public string? Discounts { get; private set; }

  public string? CreatedBy { get; private set; }
  public DateTime CreatedOn { get; private set; }
  public string? UpdatedBy { get; private set; }
  public DateTime UpdatedOn { get; private set; }

  public CharacterTalentEntity(CharacterEntity character, TalentEntity talent, CharacterTalent detail, DomainEvent @event, Guid? id = null)
  {
    Character = character;
    CharacterId = character.CharacterId;

    Id = id ?? Guid.NewGuid();

    Talent = talent;
    TalentId = talent.TalentId;

    Qualifier = detail.Qualifier?.Value;
    Notes = detail.Notes?.Value;
    Discounts = detail.Discounts.Count < 1 ? null : JsonSerializer.Serialize(detail.Discounts, _serializerOptions);

    CreatedBy = UpdatedBy = @event.ActorId?.Value;
    CreatedOn = UpdatedOn = @event.OccurredOn.AsUniversalTime();
  }

  private CharacterTalentEntity()
  {
  }

  public override bool Equals(object? obj) => obj is CharacterTalentEntity entity && entity.CharacterId == CharacterId && entity.Id == Id;
  public override int GetHashCode() => HashCode.Combine(CharacterId, Id);
  public override string ToString() => $"{base.ToString()} (CharacterId={CharacterId}, Id={Id})";
}
