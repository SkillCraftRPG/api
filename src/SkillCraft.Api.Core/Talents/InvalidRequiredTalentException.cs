using Krakenar.Contracts;
using Logitar;

namespace SkillCraft.Api.Core.Talents;

public class InvalidRequiredTalentException : DomainException
{
  private const string ErrorMessage = "The required talent tier must be less than or equal to the requiring talent tier.";

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
  public Guid RequiredTalentId
  {
    get => (Guid)Data[nameof(RequiredTalentId)]!;
    private set => Data[nameof(RequiredTalentId)] = value;
  }
  public int RequiringTalentTier
  {
    get => (int)Data[nameof(RequiringTalentTier)]!;
    private set => Data[nameof(RequiringTalentTier)] = value;
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
      error.Data[nameof(RequiredTalentId)] = RequiredTalentId;
      error.Data[nameof(RequiringTalentTier)] = RequiringTalentTier;
      error.Data[nameof(RequiredTalentTier)] = RequiredTalentTier;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public InvalidRequiredTalentException(Talent talent)
    : base(BuildMessage(talent))
  {
    Talent requiredTalent = talent.RequiredTalent ?? throw new ArgumentException("The required talent is required.", nameof(talent));

    WorldId = talent.WorldId;
    RequiringTalentId = talent.Id;
    RequiredTalentId = requiredTalent.Id;
    RequiringTalentTier = talent.Tier;
    RequiredTalentTier = requiredTalent.Tier;
    PropertyName = nameof(Talent.RequiredTalentId);
  }

  private static string BuildMessage(Talent talent) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(WorldId), talent.WorldId)
    .AddData(nameof(RequiringTalentId), talent.Id)
    .AddData(nameof(RequiredTalentId), talent.RequiredTalent?.Id)
    .AddData(nameof(RequiringTalentTier), talent.Tier)
    .AddData(nameof(RequiredTalentTier), talent.RequiredTalent?.Tier)
    .AddData(nameof(PropertyName), nameof(Talent.RequiredTalentId))
    .Build();
}
