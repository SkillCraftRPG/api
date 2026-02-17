using Krakenar.Contracts;
using Logitar;
using SkillCraft.Api.Contracts.Customizations;

namespace SkillCraft.Api.Core.Customizations;

public class CannotChangeCustomizationKindException : DomainException
{
  private const string ErrorMessage = "The customization kind cannot be changed.";

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
  public CustomizationKind CustomizationKind
  {
    get => (CustomizationKind)Data[nameof(CustomizationKind)]!;
    private set => Data[nameof(CustomizationKind)] = value;
  }
  public CustomizationKind AttemptedKind
  {
    get => (CustomizationKind)Data[nameof(AttemptedKind)]!;
    private set => Data[nameof(AttemptedKind)] = value;
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
      error.Data[nameof(CustomizationKind)] = CustomizationKind;
      error.Data[nameof(AttemptedKind)] = AttemptedKind;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public CannotChangeCustomizationKindException(Customization customization, CustomizationKind attemptedKind, string propertyName)
    : base(BuildMessage(customization, attemptedKind, propertyName))
  {
    WorldId = customization.WorldId.ToGuid();
    CustomizationId = customization.EntityId;
    CustomizationKind = customization.Kind;
    AttemptedKind = attemptedKind;
    PropertyName = propertyName;
  }

  private static string BuildMessage(Customization customization, CustomizationKind attemptedKind, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(WorldId), customization.WorldId.ToGuid())
    .AddData(nameof(CustomizationId), customization.EntityId)
    .AddData(nameof(CustomizationKind), customization.Kind)
    .AddData(nameof(AttemptedKind), attemptedKind)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
