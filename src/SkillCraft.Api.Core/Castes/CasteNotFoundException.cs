using Krakenar.Contracts;
using Logitar;

namespace SkillCraft.Api.Core.Castes;

public class CasteNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified caste was not found.";

  public Guid WorldId
  {
    get => (Guid)Data[nameof(WorldId)]!;
    private set => Data[nameof(WorldId)] = value;
  }
  public Guid CasteId
  {
    get => (Guid)Data[nameof(CasteId)]!;
    private set => Data[nameof(CasteId)] = value;
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
      error.Data[nameof(CasteId)] = CasteId;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public CasteNotFoundException(CasteId casteId, string propertyName) : base(BuildMessage(casteId, propertyName))
  {
    WorldId = casteId.WorldId.ResourceId;
    CasteId = casteId.ResourceId;
    PropertyName = propertyName;
  }

  private static string BuildMessage(CasteId casteId, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(WorldId), casteId.WorldId.ResourceId)
    .AddData(nameof(CasteId), casteId.ResourceId)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
