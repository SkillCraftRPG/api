using Krakenar.Contracts;
using Logitar;

namespace SkillCraft.Api.Core.Lineages;

public class InvalidParentLineageException : DomainException
{
  private const string ErrorMessage = "The specified parent lineage should not have a parent.";

  public Guid WorldId
  {
    get => (Guid)Data[nameof(WorldId)]!;
    private set => Data[nameof(WorldId)] = value;
  }
  public Guid ParentId
  {
    get => (Guid)Data[nameof(ParentId)]!;
    private set => Data[nameof(ParentId)] = value;
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
      error.Data[nameof(ParentId)] = ParentId;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public InvalidParentLineageException(Lineage parent, string propertyName)
    : base(BuildMessage(parent, propertyName))
  {
    WorldId = parent.WorldId;
    ParentId = parent.Id;
    PropertyName = propertyName;
  }

  private static string BuildMessage(Lineage parent, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(WorldId), parent.WorldId)
    .AddData(nameof(ParentId), parent.Id)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
