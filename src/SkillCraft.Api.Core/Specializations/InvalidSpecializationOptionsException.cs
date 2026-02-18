using Krakenar.Contracts;
using Logitar;
using SkillCraft.Api.Core.Talents;

namespace SkillCraft.Api.Core.Specializations;

public class InvalidSpecializationOptionsException : DomainException
{
  private const string ErrorMessage = "The optional talent tiers should be lower than the specialization tier.";

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
  public IReadOnlyDictionary<Guid, int> Talents
  {
    get => (IReadOnlyDictionary<Guid, int>)Data[nameof(Talents)]!;
    private set => Data[nameof(Talents)] = value;
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
      error.Data[nameof(Talents)] = Talents;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public InvalidSpecializationOptionsException(Specialization specialization, IEnumerable<Talent> talents, string propertyName)
    : base(BuildMessage(specialization, talents, propertyName))
  {
    WorldId = specialization.WorldId.ToGuid();
    SpecializationId = specialization.EntityId;
    SpecializationTier = specialization.Tier.Value;
    Talents = talents.ToDictionary(x => x.Id.EntityId, x => x.Tier.Value);
    PropertyName = propertyName;
  }

  private static string BuildMessage(Specialization specialization, IEnumerable<Talent> talents, string propertyName)
  {
    StringBuilder message = new(ErrorMessage);
    message.AppendLine().Append(nameof(WorldId)).Append(": ").Append(specialization.WorldId.ToGuid()).AppendLine();
    message.Append(nameof(SpecializationId)).Append(": ").Append(specialization.EntityId).AppendLine();
    message.Append(nameof(SpecializationTier)).Append(": ").Append(specialization.Tier).AppendLine();
    message.Append(nameof(PropertyName)).Append(": ").Append(propertyName).AppendLine();
    message.Append(nameof(Talents)).AppendLine(":");
    foreach (Talent talent in talents)
    {
      message.Append(" - ").Append(talent.EntityId).Append('=').Append(talent.Tier).AppendLine();
    }
    return message.ToString();
  }
}
