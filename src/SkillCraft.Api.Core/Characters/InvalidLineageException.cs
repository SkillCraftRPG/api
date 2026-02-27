using Krakenar.Contracts;
using Logitar;
using SkillCraft.Api.Core.Lineages;

namespace SkillCraft.Api.Core.Characters;

public class InvalidLineageException : DomainException
{
  private const string ErrorMessage = "THe character lineage should not have children.";

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

  public InvalidLineageException(Lineage lineage, string propertyName) : base(BuildMessage(lineage, propertyName))
  {
    WorldId = lineage.WorldId.ToGuid();
    LineageId = lineage.EntityId;
    PropertyName = propertyName;
  }

  private static string BuildMessage(Lineage lineage, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(WorldId), lineage.WorldId.ToGuid())
    .AddData(nameof(LineageId), lineage.EntityId)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
