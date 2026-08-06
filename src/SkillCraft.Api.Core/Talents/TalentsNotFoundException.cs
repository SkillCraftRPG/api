using Krakenar.Contracts;
using Logitar;

namespace SkillCraft.Api.Core.Talents;

public class TalentsNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified talents were not found.";

  public Guid WorldId
  {
    get => (Guid)Data[nameof(WorldId)]!;
    private set => Data[nameof(WorldId)] = value;
  }
  public IReadOnlyCollection<Guid> TalentIds
  {
    get => (IReadOnlyCollection<Guid>)Data[nameof(TalentIds)]!;
    private set => Data[nameof(TalentIds)] = value;
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
      error.Data[nameof(TalentIds)] = TalentIds;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public TalentsNotFoundException(IEnumerable<TalentId> talentIds, string propertyName)
    : base(BuildMessage(talentIds, propertyName))
  {
    WorldId = talentIds.Select(id => id.WorldId).Distinct().Single().ResourceId;
    TalentIds = talentIds.Select(id => id.ResourceId).Distinct().ToList().AsReadOnly();
    PropertyName = propertyName;
  }

  private static string BuildMessage(IEnumerable<TalentId> talentIds, string propertyName)
  {
    StringBuilder message = new();
    message.AppendLine(ErrorMessage);
    message.Append(nameof(WorldId)).Append(": ").Append(talentIds.Select(id => id.WorldId).Distinct().Single().ResourceId).AppendLine();
    if (talentIds.Any())
    {
      message.Append(nameof(TalentIds)).Append(':').AppendLine();
      foreach (TalentId talentId in talentIds.Distinct())
      {
        message.Append(" - ").Append(talentId.ResourceId).AppendLine();
      }
    }
    message.Append(nameof(PropertyName)).Append(": ").Append(propertyName).AppendLine();
    return message.ToString();
  }
}
