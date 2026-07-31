using Krakenar.Contracts;
using Logitar;
using System.Text;

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

  public LanguagesNotFoundException(Guid worldId, IEnumerable<Guid> languageIds, string propertyName)
    : base(BuildMessage(worldId, languageIds, propertyName))
  {
    WorldId = worldId;
    LanguageIds = languageIds.ToHashSet();
    PropertyName = propertyName;
  }

  private static string BuildMessage(Guid worldId, IEnumerable<Guid> languageIds, string propertyName)
  {
    StringBuilder message = new();
    message.AppendLine(ErrorMessage);
    message.Append(nameof(WorldId)).Append(": ").Append(worldId).AppendLine();
    if (languageIds.Any())
    {
      message.Append(nameof(LanguageIds)).Append(':').AppendLine();
      foreach (Guid languageId in languageIds.Distinct())
      {
        message.Append(" - ").Append(languageId).AppendLine();
      }
    }
    message.Append(nameof(PropertyName)).Append(": ").Append(propertyName).AppendLine();
    return message.ToString();
  }
}
