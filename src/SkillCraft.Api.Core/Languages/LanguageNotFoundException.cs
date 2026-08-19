using Krakenar.Contracts;
using Logitar;

namespace SkillCraft.Api.Core.Languages;

public class LanguageNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified language was not found.";

  public Guid WorldId
  {
    get => (Guid)Data[nameof(WorldId)]!;
    private set => Data[nameof(WorldId)] = value;
  }
  public Guid LanguageId
  {
    get => (Guid)Data[nameof(LanguageId)]!;
    private set => Data[nameof(LanguageId)] = value;
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
      error.Data[nameof(LanguageId)] = LanguageId;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public LanguageNotFoundException(LanguageId languageId, string propertyName) : base(BuildMessage(languageId, propertyName))
  {
    WorldId = languageId.WorldId.ResourceId;
    LanguageId = languageId.ResourceId;
    PropertyName = propertyName;
  }

  private static string BuildMessage(LanguageId languageId, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(WorldId), languageId.WorldId.ResourceId)
    .AddData(nameof(LanguageId), languageId.ResourceId)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
