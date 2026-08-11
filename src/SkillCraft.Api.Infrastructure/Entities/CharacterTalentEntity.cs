using Logitar;
using Logitar.EventSourcing;
using SkillCraft.Api.Core.Characters;
using SkillCraft.Api.Core.Characters.Models;

namespace SkillCraft.Api.Infrastructure.Entities;

internal class CharacterTalentEntity
{
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
    SetDiscounts(detail.Discounts);

    CreatedBy = UpdatedBy = @event.ActorId?.Value;
    CreatedOn = UpdatedOn = @event.OccurredOn.AsUniversalTime();
  }

  private CharacterTalentEntity()
  {
  }

  public IReadOnlyCollection<ActorId> GetActorIds()
  {
    HashSet<ActorId> actorIds = [];
    if (Talent is not null)
    {
      actorIds.AddRange(Talent.GetActorIds());
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

  public IReadOnlyCollection<CharacterTalentDiscountModel> GetDiscounts()
  {
    return (Discounts is null ? null : EntitySerializer.Instance.Deserialize<IReadOnlyCollection<CharacterTalentDiscountModel>>(Discounts)) ?? [];
  }
  private void SetDiscounts(IReadOnlyCollection<CharacterTalentDiscount> discounts)
  {
    IEnumerable<CharacterTalentDiscountModel> models = discounts.Select(discount => new CharacterTalentDiscountModel(discount));
    Discounts = models.Any() ? EntitySerializer.Instance.Serialize(models) : null;
  }

  public override bool Equals(object? obj) => obj is CharacterTalentEntity entity && entity.CharacterId == CharacterId && entity.Id == Id;
  public override int GetHashCode() => HashCode.Combine(CharacterId, Id);
  public override string ToString() => $"{base.ToString()} (CharacterId={CharacterId}, Id={Id})";
}
