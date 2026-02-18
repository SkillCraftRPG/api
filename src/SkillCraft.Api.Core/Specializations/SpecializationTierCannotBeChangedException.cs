using Krakenar.Contracts;
using Logitar;

namespace SkillCraft.Api.Core.Specializations;

public class SpecializationTierCannotBeChangedException : DomainException
{
  private const string ErrorMessage = "The specialization tier cannot be changed.";

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
  public int AttemptedTier
  {
    get => (int)Data[nameof(AttemptedTier)]!;
    private set => Data[nameof(AttemptedTier)] = value;
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
      error.Data[nameof(AttemptedTier)] = AttemptedTier;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public SpecializationTierCannotBeChangedException(Specialization specialization, int attemptedTier, string propertyName)
    : base(BuildMessage(specialization, attemptedTier, propertyName))
  {
    WorldId = specialization.WorldId.ToGuid();
    SpecializationId = specialization.EntityId;
    SpecializationTier = specialization.Tier.Value;
    AttemptedTier = attemptedTier;
    PropertyName = propertyName;
  }

  private static string BuildMessage(Specialization specialization, int attemptedTier, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(WorldId), specialization.WorldId.ToGuid())
    .AddData(nameof(SpecializationId), specialization.EntityId)
    .AddData(nameof(SpecializationTier), specialization.Tier)
    .AddData(nameof(AttemptedTier), attemptedTier)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
