using Krakenar.Contracts;
using Logitar;

namespace SkillCraft.Api.Core.Lineages;

public class InvalidParentLineageException : DomainException
{
  private const string ErrorMessage = "The parent lineage should not have a parent.";

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
  public Guid ParentId
  {
    get => (Guid)Data[nameof(ParentId)]!;
    private set => Data[nameof(ParentId)] = value;
  }
  public Guid AncestorId
  {
    get => (Guid)Data[nameof(AncestorId)]!;
    private set => Data[nameof(AncestorId)] = value;
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
      error.Data[nameof(AncestorId)] = AncestorId;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public InvalidParentLineageException(Lineage lineage, Lineage parent, string propertyName)
    : base(BuildMessage(lineage, parent, propertyName))
  {
    if (!parent.ParentId.HasValue)
    {
      throw new ArgumentException("The parent lineage should have a parent.", nameof(parent));
    }

    WorldId = lineage.WorldId.ToGuid();
    LineageId = lineage.EntityId;
    ParentId = parent.EntityId;
    AncestorId = parent.ParentId.Value.EntityId;
    PropertyName = propertyName;
  }

  private static string BuildMessage(Lineage lineage, Lineage parent, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(WorldId), lineage.WorldId.ToGuid())
    .AddData(nameof(LineageId), lineage.EntityId)
    .AddData(nameof(ParentId), parent.EntityId)
    .AddData(nameof(AncestorId), parent.ParentId?.EntityId)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
