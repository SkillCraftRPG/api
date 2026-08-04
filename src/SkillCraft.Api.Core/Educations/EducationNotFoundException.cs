using Krakenar.Contracts;
using Logitar;

namespace SkillCraft.Api.Core.Educations;

public class EducationNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified education was not found.";

  public Guid WorldId
  {
    get => (Guid)Data[nameof(WorldId)]!;
    private set => Data[nameof(WorldId)] = value;
  }
  public Guid EducationId
  {
    get => (Guid)Data[nameof(EducationId)]!;
    private set => Data[nameof(EducationId)] = value;
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
      error.Data[nameof(EducationId)] = EducationId;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public EducationNotFoundException(EducationId educationId, string propertyName) : base(BuildMessage(educationId, propertyName))
  {
    WorldId = educationId.WorldId.ResourceId;
    EducationId = educationId.ResourceId;
    PropertyName = propertyName;
  }

  private static string BuildMessage(EducationId educationId, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(WorldId), educationId.WorldId.ResourceId)
    .AddData(nameof(EducationId), educationId.ResourceId)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
