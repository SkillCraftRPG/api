using Krakenar.Contracts;
using Logitar;

namespace SkillCraft.Api.Core.Lineages;

public class LineageNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified lineage was not found.";

  public Guid WorldId
  {
    get => (Guid)Data[nameof(WorldId)]!;
    private set => Data[nameof(WorldId)] = value;
  }
  public Guid LineageId
  {
    get => (Guid)Data[nameof(LineageId)]!;
    private set => Data[nameof(LineageId)] = value;
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
      error.Data[nameof(LineageId)] = LineageId;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public LineageNotFoundException(LineageId lineageId, string propertyName) : base(BuildMessage(lineageId, propertyName))
  {
    WorldId = lineageId.WorldId.ResourceId;
    LineageId = lineageId.ResourceId;
    PropertyName = propertyName;
  }

  private static string BuildMessage(LineageId lineageId, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(WorldId), lineageId.WorldId.ResourceId)
    .AddData(nameof(LineageId), lineageId.ResourceId)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
