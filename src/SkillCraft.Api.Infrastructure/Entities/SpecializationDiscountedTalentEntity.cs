namespace SkillCraft.Api.Infrastructure.Entities;

internal class SpecializationDiscountedTalentEntity
{
  public SpecializationEntity? Specialization { get; private set; }
  public int SpecializationId { get; private set; }
  public Guid SpecializationUid { get; private set; }

  public TalentEntity? Talent { get; private set; }
  public int TalentId { get; private set; }
  public Guid TalentUid { get; private set; }

  public SpecializationDiscountedTalentEntity(SpecializationEntity specialization, TalentEntity talent)
  {
    Specialization = specialization;
    SpecializationId = specialization.SpecializationId;
    SpecializationUid = specialization.Id;
    Talent = talent;
    TalentId = talent.TalentId;
    TalentUid = talent.Id;
  }

  private SpecializationDiscountedTalentEntity()
  {
  }

  public override bool Equals(object? obj) => obj is SpecializationDiscountedTalentEntity entity && entity.SpecializationId == SpecializationId && entity.TalentId == TalentId;
  public override int GetHashCode() => HashCode.Combine(SpecializationId, TalentId);
  public override string ToString() => $"{GetType()} (SpecializationId={SpecializationId}, TalentId={TalentId})";
}
