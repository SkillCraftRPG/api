using Krakenar.Contracts;
using Logitar;

namespace SkillCraft.Api.Core.Talents;

public class TalentTierCannotBeChangedException : DomainException
{
  private const string ErrorMessage = "The talent tier cannot be changed.";

  public Guid WorldId
  {
    get => (Guid)Data[nameof(WorldId)]!;
    private set => Data[nameof(WorldId)] = value;
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
      error.Data[nameof(TalentId)] = TalentId;
      error.Data[nameof(TalentTier)] = TalentTier;
      error.Data[nameof(AttemptedTier)] = AttemptedTier;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public TalentTierCannotBeChangedException(Talent talent, int attemptedTier, string propertyName)
    : base(BuildMessage(talent, attemptedTier, propertyName))
  {
    WorldId = talent.WorldId.ToGuid();
    TalentId = talent.EntityId;
    TalentTier = talent.Tier.Value;
    AttemptedTier = attemptedTier;
    PropertyName = propertyName;
  }

  private static string BuildMessage(Talent talent, int attemptedTier, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(WorldId), talent.WorldId.ToGuid())
    .AddData(nameof(TalentId), talent.EntityId)
    .AddData(nameof(TalentTier), talent.Tier)
    .AddData(nameof(AttemptedTier), attemptedTier)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
