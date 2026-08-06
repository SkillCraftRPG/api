using SkillCraft.Api.Core.Talents;

namespace SkillCraft.Api.Core.Characters;

public record CharacterTalent
{
  public TalentId TalentId { get; }

  public Name? Qualifier { get; }
  public Notes? Notes { get; }
  public IReadOnlyCollection<CharacterTalentDiscount> Discounts { get; }

  [JsonIgnore]
  public Talent? Talent { get; }

  [JsonIgnore]
  public int Cost
  {
    get
    {
      if (Talent is null)
      {
        throw new InvalidOperationException("The talent is required.");
      }
      int cost = Talent.Cost;
      int discount = Discounts.Sum(discount => discount.Amount);
      return discount >= cost ? 0 : cost - discount;
    }
  }

  [JsonConstructor]
  public CharacterTalent(TalentId talentId, Name? qualifier, Notes? notes, IReadOnlyCollection<CharacterTalentDiscount> discounts)
  {
    TalentId = talentId;

    Qualifier = qualifier;
    Notes = notes;
    Discounts = discounts;
  }

  public CharacterTalent(Talent talent, Name? qualifier = null, Notes? notes = null, IEnumerable<CharacterTalentDiscount>? discounts = null)
    : this(talent.Id, qualifier, notes, (discounts ?? []).ToList().AsReadOnly())
  {
    Talent = talent;
  }
}
