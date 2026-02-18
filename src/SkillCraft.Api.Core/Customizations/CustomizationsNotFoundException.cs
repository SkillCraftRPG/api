using Krakenar.Contracts;
using Logitar;
using SkillCraft.Api.Core.Worlds;

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

  public CustomizationsNotFoundException(WorldId worldId, IEnumerable<Guid> customizationIds, string propertyName)
    : base(BuildMessage(worldId, customizationIds, propertyName))
  {
    WorldId = worldId.ToGuid();
    CustomizationIds = customizationIds.Distinct().ToList().AsReadOnly();
    PropertyName = propertyName;
  }

  private static string BuildMessage(WorldId worldId, IEnumerable<Guid> customizationIds, string propertyName)
  {
    StringBuilder message = new(ErrorMessage);
    message.AppendLine();
    message.Append(nameof(WorldId)).Append(": ").Append(worldId.ToGuid()).AppendLine();
    message.Append(nameof(PropertyName)).Append(": ").AppendLine(propertyName);
    message.Append(nameof(CustomizationIds)).AppendLine(":");
    foreach (Guid customizationId in customizationIds)
    {
      message.Append(" - ").Append(customizationId).AppendLine();
    }
    return message.ToString();
  }
}
