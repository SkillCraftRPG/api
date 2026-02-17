using Krakenar.Contracts;
using Logitar;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Languages;

public class LanguagesNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified languages were not found.";

  public Guid WorldId
  {
    get => (Guid)Data[nameof(WorldId)]!;
    private set => Data[nameof(WorldId)] = value;
  }
  public IReadOnlyCollection<Guid> LanguageIds
  {
    get => (IReadOnlyCollection<Guid>)Data[nameof(LanguageIds)]!;
    private set => Data[nameof(LanguageIds)] = value;
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
      error.Data[nameof(LanguageIds)] = LanguageIds;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public LanguagesNotFoundException(WorldId worldId, IEnumerable<LanguageId> languageIds, string propertyName)
    : base(BuildMessage(worldId, languageIds, propertyName))
  {
    WorldId = worldId.ToGuid();
    LanguageIds = languageIds.Distinct().Select(id => id.EntityId).ToList().AsReadOnly();
    PropertyName = propertyName;
  }

  private static string BuildMessage(WorldId worldId, IEnumerable<LanguageId> languageIds, string propertyName)
  {
    StringBuilder message = new(ErrorMessage);
    message.AppendLine();
    message.Append("WorldId: ").Append(worldId.ToGuid()).AppendLine();
    message.Append("PropertyName: ").AppendLine(propertyName);
    message.AppendLine("LanguageIds:");
    foreach (LanguageId languageId in languageIds)
    {
      message.Append(" - ").Append(languageId.EntityId).AppendLine();
    }
    return message.ToString();
  }
}
