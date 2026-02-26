using Krakenar.Contracts;
using Logitar;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Talents;

public class TalentsNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified talents not found.";

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

  public TalentsNotFoundException(WorldId worldId, IEnumerable<Guid> talentIds, string propertyName)
    : base(BuildMessage(worldId, talentIds, propertyName))
  {
    WorldId = worldId.ToGuid();
    TalentIds = Sanitize(talentIds);
    PropertyName = propertyName;
  }

  private static string BuildMessage(WorldId worldId, IEnumerable<Guid> talentIds, string propertyName)
  {
    talentIds = Sanitize(talentIds);

    StringBuilder message = new();
    message.AppendLine(ErrorMessage);
    message.Append(nameof(WorldId)).Append(": ").Append(worldId.ToGuid()).AppendLine();
    message.Append(nameof(PropertyName)).Append(": ").AppendLine(propertyName);
    message.Append(nameof(TalentIds)).AppendLine(":");
    foreach (Guid talentId in talentIds)
    {
      message.Append(" - ").Append(talentId).AppendLine();
    }
    return message.ToString();
  }

  private static IReadOnlyCollection<Guid> Sanitize(IEnumerable<Guid> talentIds) => talentIds.Distinct().ToList().AsReadOnly();
}
