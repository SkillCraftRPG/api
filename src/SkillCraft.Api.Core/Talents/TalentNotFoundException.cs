using Krakenar.Contracts;
using Logitar;

namespace SkillCraft.Api.Core.Talents;

public class TalentNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified talent was not found.";

  public Guid WorldId
  {
    get => (Guid)Data[nameof(WorldId)]!;
    private set => Data[nameof(WorldId)] = value;
  }
  public Guid TalentId
  {
    get => (Guid)Data[nameof(TalentId)]!;
    private set => Data[nameof(TalentId)] = value;
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
      error.Data[nameof(TalentId)] = TalentId;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public TalentNotFoundException(TalentId talentId, string propertyName) : base(BuildMessage(talentId, propertyName))
  {
    WorldId = talentId.WorldId.ResourceId;
    TalentId = talentId.ResourceId;
    PropertyName = propertyName;
  }

  private static string BuildMessage(TalentId talentId, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(WorldId), talentId.WorldId.ResourceId)
    .AddData(nameof(TalentId), talentId.ResourceId)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
