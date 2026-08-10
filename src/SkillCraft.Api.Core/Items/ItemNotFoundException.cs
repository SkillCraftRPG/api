using Krakenar.Contracts;
using Logitar;

namespace SkillCraft.Api.Core.Items;

public class ItemNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified item was not found.";

  public Guid WorldId
  {
    get => (Guid)Data[nameof(WorldId)]!;
    private set => Data[nameof(WorldId)] = value;
  }
  public Guid ItemId
  {
    get => (Guid)Data[nameof(ItemId)]!;
    private set => Data[nameof(ItemId)] = value;
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
      error.Data[nameof(ItemId)] = ItemId;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public ItemNotFoundException(ItemId itemId, string propertyName) : base(BuildMessage(itemId, propertyName))
  {
    WorldId = itemId.WorldId.ResourceId;
    ItemId = itemId.ResourceId;
    PropertyName = propertyName;
  }

  private static string BuildMessage(ItemId itemId, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(WorldId), itemId.WorldId.ResourceId)
    .AddData(nameof(ItemId), itemId.ResourceId)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
