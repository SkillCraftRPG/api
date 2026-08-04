using Krakenar.Contracts;
using Logitar;

namespace SkillCraft.Api.Core.Customizations;

public class CustomizationsNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified customizations were not found.";

  public Guid WorldId
  {
    get => (Guid)Data[nameof(WorldId)]!;
    private set => Data[nameof(WorldId)] = value;
  }
  public IReadOnlyCollection<Guid> CustomizationIds
  {
    get => (IReadOnlyCollection<Guid>)Data[nameof(CustomizationIds)]!;
    private set => Data[nameof(CustomizationIds)] = value;
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
      error.Data[nameof(CustomizationIds)] = CustomizationIds;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public CustomizationsNotFoundException(IEnumerable<CustomizationId> customizationIds, string propertyName)
    : base(BuildMessage(customizationIds, propertyName))
  {
    WorldId = customizationIds.Select(id => id.WorldId).Distinct().Single().ResourceId;
    CustomizationIds = customizationIds.Select(id => id.ResourceId).Distinct().ToList().AsReadOnly();
    PropertyName = propertyName;
  }

  private static string BuildMessage(IEnumerable<CustomizationId> customizationIds, string propertyName)
  {
    StringBuilder message = new();
    message.AppendLine(ErrorMessage);
    message.Append(nameof(WorldId)).Append(": ").Append(customizationIds.Select(id => id.WorldId).Distinct().Single().ResourceId).AppendLine();
    if (customizationIds.Any())
    {
      message.Append(nameof(CustomizationIds)).Append(':').AppendLine();
      foreach (CustomizationId customizationId in customizationIds.Distinct())
      {
        message.Append(" - ").Append(customizationId.ResourceId).AppendLine();
      }
    }
    message.Append(nameof(PropertyName)).Append(": ").Append(propertyName).AppendLine();
    return message.ToString();
  }
}
