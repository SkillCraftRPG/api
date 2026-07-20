using Krakenar.Contracts;
using Logitar;

namespace SkillCraft.Api.Core;

public class ResourceNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified resource was not found.";

  public Guid? WorldId
  {
    get => (Guid?)Data[nameof(WorldId)];
    private set => Data[nameof(WorldId)] = value;
  }
  public string ResourceKind
  {
    get => (string)Data[nameof(ResourceKind)]!;
    private set => Data[nameof(ResourceKind)] = value;
  }
  public Guid ResourceId
  {
    get => (Guid)Data[nameof(ResourceId)]!;
    private set => Data[nameof(ResourceId)] = value;
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
      error.Data[nameof(ResourceKind)] = ResourceKind;
      error.Data[nameof(ResourceId)] = ResourceId;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public ResourceNotFoundException(ResourceIdentifier identifier, string propertyName)
    : base(BuildMessage(identifier, propertyName))
  {
    WorldId = identifier.WorldId;
    ResourceKind = identifier.Kind;
    ResourceId = identifier.Id;
    PropertyName = propertyName;
  }

  private static string BuildMessage(ResourceIdentifier identifier, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(WorldId), identifier.WorldId, "<null>")
    .AddData(nameof(ResourceKind), identifier.Kind)
    .AddData(nameof(ResourceId), identifier.Id)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
