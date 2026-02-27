using Krakenar.Contracts;
using Logitar;
using SkillCraft.Api.Core.Customizations;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Characters;

public class InvalidCustomizationSelectionException : DomainException
{
  private const string ErrorMessage = "The number of gifts should match the number of disabilities.";

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
  public int Gifts
  {
    get => (int)Data[nameof(Gifts)]!;
    private set => Data[nameof(Gifts)] = value;
  }
  public int Disabilities
  {
    get => (int)Data[nameof(Disabilities)]!;
    private set => Data[nameof(Disabilities)] = value;
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
      return error;
    }
  }

  public InvalidCustomizationSelectionException(WorldId worldId, IEnumerable<CustomizationId> customizationIds, int gifts, int disabilities, string propertyName)
    : base(BuildMessage(worldId, customizationIds, gifts, disabilities, propertyName))
  {
    WorldId = worldId.ToGuid();
    CustomizationIds = GetCustomizationIds(customizationIds);
    Gifts = gifts;
    Disabilities = disabilities;
    PropertyName = propertyName;
  }

  private static string BuildMessage(WorldId worldId, IEnumerable<CustomizationId> customizationIds, int gifts, int disabilities, string propertyName)
  {
    IEnumerable<Guid> entityIds = GetCustomizationIds(customizationIds);

    StringBuilder message = new();
    message.AppendLine(ErrorMessage);
    message.Append(nameof(WorldId)).Append(": ").Append(worldId.ToGuid()).AppendLine();
    message.Append(nameof(Gifts)).Append(": ").Append(gifts).AppendLine();
    message.Append(nameof(Disabilities)).Append(": ").Append(disabilities).AppendLine();
    message.Append(nameof(PropertyName)).Append(": ").Append(propertyName).AppendLine();
    message.Append(nameof(CustomizationIds)).AppendLine(":");
    foreach (Guid customizationId in entityIds)
    {
      message.Append(" - ").Append(customizationId).AppendLine();
    }
    return message.ToString();
  }

  private static IReadOnlyCollection<Guid> GetCustomizationIds(IEnumerable<CustomizationId> customizationIds) => customizationIds
    .Select(customizationId => customizationId.EntityId)
    .Distinct()
    .ToList()
    .AsReadOnly();
}
