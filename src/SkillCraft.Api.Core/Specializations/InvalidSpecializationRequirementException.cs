using Krakenar.Contracts;
using Logitar;
using SkillCraft.Api.Core.Talents;

namespace SkillCraft.Api.Core.Specializations;

public class InvalidSpecializationRequirementException : DomainException
{
  private const string ErrorMessage = "The required talent tier should be lower than the specialization tier.";

  public Guid WorldId
  {
    get => (Guid)Data[nameof(WorldId)]!;
    private set => Data[nameof(WorldId)] = value;
  }
  public Guid SpecializationId
  {
    get => (Guid)Data[nameof(SpecializationId)]!;
    private set => Data[nameof(SpecializationId)] = value;
  }
  public int SpecializationTier
  {
    get => (int)Data[nameof(SpecializationTier)]!;
    private set => Data[nameof(SpecializationTier)] = value;
  }
  public Guid TalentId
  {
    get => (Guid)Data[nameof(TalentId)]!;
    private set => Data[nameof(TalentId)] = value;
  }
  public int TalentTier
  {
    get => (int)Data[nameof(TalentTier)]!;
    private set => Data[nameof(TalentTier)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error
  {
    get
    {
      Error error = new(this.GetErrorCode(), ErrorMessage);
      error.Data[nameof(WorldId)] = WorldId;
      error.Data[nameof(SpecializationId)] = SpecializationId;
      error.Data[nameof(SpecializationTier)] = SpecializationTier;
      error.Data[nameof(TalentId)] = TalentId;
      error.Data[nameof(TalentTier)] = TalentTier;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public InvalidSpecializationRequirementException(Specialization specialization, Talent talent, string propertyName)
    : base(BuildMessage(specialization, talent, propertyName))
  {
    WorldId = specialization.WorldId.ToGuid();
    SpecializationId = specialization.EntityId;
    SpecializationTier = specialization.Tier.Value;
    TalentId = talent.EntityId;
    TalentTier = talent.Tier.Value;
    PropertyName = propertyName;
  }

  private static string BuildMessage(Specialization specialization, Talent talent, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(WorldId), specialization.WorldId.ToGuid())
    .AddData(nameof(SpecializationId), specialization.EntityId)
    .AddData(nameof(SpecializationTier), specialization.Tier)
    .AddData(nameof(TalentId), talent.EntityId)
    .AddData(nameof(TalentTier), talent.Tier)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
