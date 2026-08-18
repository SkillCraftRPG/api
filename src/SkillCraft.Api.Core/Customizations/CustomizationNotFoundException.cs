using Krakenar.Contracts;
using Logitar;

namespace SkillCraft.Api.Core.Customizations;

public class CustomizationNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified customization was not found.";

  public Guid WorldId
  {
    get => (Guid)Data[nameof(WorldId)]!;
    private set => Data[nameof(WorldId)] = value;
  }
  public Guid CustomizationId
  {
    get => (Guid)Data[nameof(CustomizationId)]!;
    private set => Data[nameof(CustomizationId)] = value;
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
      error.Data[nameof(CustomizationId)] = CustomizationId;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public CustomizationNotFoundException(CustomizationId customizationId, string propertyName) : base(BuildMessage(customizationId, propertyName))
  {
    WorldId = customizationId.WorldId.ResourceId;
    CustomizationId = customizationId.ResourceId;
    PropertyName = propertyName;
  }

  private static string BuildMessage(CustomizationId customizationId, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(WorldId), customizationId.WorldId.ResourceId)
    .AddData(nameof(CustomizationId), customizationId.ResourceId)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
