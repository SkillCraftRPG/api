using Krakenar.Contracts;
using Logitar;
using Logitar.EventSourcing;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Permissions;

public class PermissionDeniedException : ErrorException
{
  private const string ErrorMessage = "The specified permission was denied.";

  public string? Principal
  {
    get => (string?)Data[nameof(Principal)];
    private set => Data[nameof(Principal)] = value;
  }
  public string Action
  {
    get => (string)Data[nameof(Action)]!;
    private set => Data[nameof(Action)] = value;
  }
  public string? Resource
  {
    get => (string?)Data[nameof(Resource)];
    private set => Data[nameof(Resource)] = value;
  }
  public Guid? WorldId
  {
    get => (Guid?)Data[nameof(WorldId)];
    private set => Data[nameof(WorldId)] = value;
  }

  public override Error Error => new(this.GetErrorCode(), ErrorMessage);

  public PermissionDeniedException(ActorId? actorId, string action, ResourceIdentifier? resource, WorldId? worldId)
    : base(BuildMessage(actorId, action, resource, worldId))
  {
    Principal = actorId?.Value;
    Action = action;
    Resource = resource?.ToString();
    WorldId = worldId?.ResourceId;
  }

  private static string BuildMessage(ActorId? actorId, string action, ResourceIdentifier? resource, WorldId? worldId) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(Principal), actorId, "<null>")
    .AddData(nameof(Action), action)
    .AddData(nameof(Resource), resource, "<null>")
    .AddData(nameof(WorldId), worldId?.ResourceId, "<null>")
    .Build();
}
