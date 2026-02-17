using Krakenar.Contracts;
using Logitar;

namespace SkillCraft.Api.Core.Lineages;

public class LineageParentCannotBeChangedException : DomainException
{
  private const string ErrorMessage = "The lineage parent cannot be changed.";

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
  public Guid? ParentId
  {
    get => (Guid?)Data[nameof(ParentId)];
    private set => Data[nameof(ParentId)] = value;
  }
  public Guid? AttemptedId
  {
    get => (Guid?)Data[nameof(AttemptedId)];
    private set => Data[nameof(AttemptedId)] = value;
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
      error.Data[nameof(ParentId)] = ParentId;
      error.Data[nameof(AttemptedId)] = AttemptedId;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public LineageParentCannotBeChangedException(Lineage lineage, LineageId? attemptedId, string propertyName)
    : base(BuildMessage(lineage, attemptedId, propertyName))
  {
    WorldId = lineage.WorldId.ToGuid();
    LineageId = lineage.EntityId;
    ParentId = lineage.ParentId?.EntityId;
    AttemptedId = attemptedId?.EntityId;
    PropertyName = propertyName;
  }

  private static string BuildMessage(Lineage lineage, LineageId? attemptedId, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(WorldId), lineage.WorldId.ToGuid())
    .AddData(nameof(LineageId), lineage.EntityId)
    .AddData(nameof(ParentId), lineage.ParentId?.EntityId, "<null>")
    .AddData(nameof(AttemptedId), attemptedId?.EntityId)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
