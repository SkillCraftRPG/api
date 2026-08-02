using Krakenar.Contracts;
using Logitar;

namespace SkillCraft.Api.Core.Scripts;

public class ScriptNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified script was not found.";

  public Guid WorldId
  {
    get => (Guid)Data[nameof(WorldId)]!;
    private set => Data[nameof(WorldId)] = value;
  }
  public Guid ScriptId
  {
    get => (Guid)Data[nameof(ScriptId)]!;
    private set => Data[nameof(ScriptId)] = value;
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
      error.Data[nameof(ScriptId)] = ScriptId;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public ScriptNotFoundException(ScriptId scriptId, string propertyName) : base(BuildMessage(scriptId, propertyName))
  {
    WorldId = scriptId.WorldId.ResourceId;
    ScriptId = scriptId.ResourceId;
    PropertyName = propertyName;
  }

  private static string BuildMessage(ScriptId scriptId, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(WorldId), scriptId.WorldId.ResourceId)
    .AddData(nameof(ScriptId), scriptId.ResourceId)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
