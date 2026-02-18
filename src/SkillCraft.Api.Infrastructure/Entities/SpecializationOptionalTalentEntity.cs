namespace SkillCraft.Api.Infrastructure.Entities;

internal class SpecializationOptionalTalentEntity
{
  public SpecializationEntity? Specialization { get; private set; }
  public int SpecializationId { get; private set; }
  public Guid SpecializationUid { get; private set; }

  public TalentEntity? Talent { get; private set; }
  public int TalentId { get; private set; }
  public Guid TalentUid { get; private set; }

  public SpecializationOptionalTalentEntity(SpecializationEntity specialization, TalentEntity talent)
  {
    Specialization = specialization;
    SpecializationId = specialization.SpecializationId;
    SpecializationUid = specialization.Id;
    Talent = talent;
    TalentId = talent.TalentId;
    TalentUid = talent.Id;
  }

  private SpecializationOptionalTalentEntity()
  {
  }

  public override bool Equals(object? obj) => obj is SpecializationOptionalTalentEntity entity && entity.SpecializationId == SpecializationId && entity.TalentId == TalentId;
  public override int GetHashCode() => HashCode.Combine(SpecializationId, TalentId);
  public override string ToString() => $"{GetType()} (SpecializationId={SpecializationId}, TalentId={TalentId})";
}
