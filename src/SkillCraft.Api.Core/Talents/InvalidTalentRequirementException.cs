using Krakenar.Contracts;
using Logitar;

namespace SkillCraft.Api.Core.Talents;

public class InvalidTalentRequirementException : DomainException
{
  private const string ErrorMessage = "The required talent tier should be lower than or equal to the requiring talent tier.";

  public Guid WorldId
  {
    get => (Guid)Data[nameof(WorldId)]!;
    private set => Data[nameof(WorldId)] = value;
  }
  public Guid RequiringTalentId
  {
    get => (Guid)Data[nameof(RequiringTalentId)]!;
    private set => Data[nameof(RequiringTalentId)] = value;
  }
  public int RequiringTalentTier
  {
    get => (int)Data[nameof(RequiringTalentTier)]!;
    private set => Data[nameof(RequiringTalentTier)] = value;
  }
  public Guid RequiredTalentId
  {
    get => (Guid)Data[nameof(RequiredTalentId)]!;
    private set => Data[nameof(RequiredTalentId)] = value;
  }
  public int RequiredTalentTier
  {
    get => (int)Data[nameof(RequiredTalentTier)]!;
    private set => Data[nameof(RequiredTalentTier)] = value;
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
      error.Data[nameof(RequiringTalentId)] = RequiringTalentId;
      error.Data[nameof(RequiringTalentTier)] = RequiringTalentTier;
      error.Data[nameof(RequiredTalentId)] = RequiredTalentId;
      error.Data[nameof(RequiredTalentTier)] = RequiredTalentTier;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public InvalidTalentRequirementException(Talent requiringTalent, Talent requiredTalent, string propertyName)
    : base(BuildMessage(requiringTalent, requiredTalent, propertyName))
  {
    WorldId = requiringTalent.WorldId.ToGuid();
    RequiringTalentId = requiringTalent.EntityId;
    RequiringTalentTier = requiringTalent.Tier.Value;
    RequiredTalentId = requiredTalent.EntityId;
    RequiredTalentTier = requiredTalent.Tier.Value;
    PropertyName = propertyName;
  }

  private static string BuildMessage(Talent requiringTalent, Talent requiredTalent, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(WorldId), requiringTalent.WorldId.ToGuid())
    .AddData(nameof(RequiringTalentId), requiringTalent.EntityId)
    .AddData(nameof(RequiringTalentTier), requiringTalent.Tier)
    .AddData(nameof(RequiredTalentId), requiredTalent.EntityId)
    .AddData(nameof(RequiredTalentTier), requiredTalent.Tier)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
