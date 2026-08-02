using Krakenar.Contracts;
using Logitar;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Talents;

public class InvalidRequiredTalentException : DomainException
{
  public const string PropertyName = nameof(Talent.RequiredTalentId);
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

  public InvalidRequiredTalentException(Talent requiringTalent, Talent requiredTalent)
    : base(BuildMessage(requiringTalent, requiredTalent))
  {
    WorldId = new HashSet<WorldId>([requiringTalent.WorldId, requiredTalent.WorldId]).Single().ResourceId;
    RequiringTalentId = requiringTalent.ResourceId;
    RequiredTalentId = requiredTalent.ResourceId;
    RequiringTalentTier = requiringTalent.Tier.Value;
    RequiredTalentTier = requiredTalent.Tier.Value;
  }

  public static void ThrowIfNotValid(Talent requiringTalent, Talent requiredTalent)
  {
    if (requiringTalent.Tier.Value < requiredTalent.Tier.Value)
    {
      throw new InvalidRequiredTalentException(requiringTalent, requiredTalent);
    }
  }

  private static string BuildMessage(Talent requiringTalent, Talent requiredTalent) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(WorldId), new HashSet<WorldId>([requiringTalent.WorldId, requiredTalent.WorldId]).Single().ResourceId)
    .AddData(nameof(RequiringTalentId), requiringTalent.ResourceId)
    .AddData(nameof(RequiredTalentId), requiredTalent.ResourceId)
    .AddData(nameof(RequiringTalentTier), requiringTalent.Tier)
    .AddData(nameof(RequiredTalentTier), requiredTalent.Tier)
    .AddData(nameof(PropertyName), PropertyName)
    .Build();
}
