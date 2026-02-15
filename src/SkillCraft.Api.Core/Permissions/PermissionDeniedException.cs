using Krakenar.Contracts;
using Logitar;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Permissions;

public class PermissionDeniedException : ErrorException
{
  private const string ErrorMessage = "The specified permission was denied.";

  public string? UserId
  {
    get => (string?)Data[nameof(UserId)];
    private set => Data[nameof(UserId)] = value;
  }
  public string Action
  {
    get => (string)Data[nameof(Action)]!;
    private set => Data[nameof(Action)] = value;
  }
  public string? EntityKind
  {
    get => (string?)Data[nameof(EntityKind)];
    private set => Data[nameof(EntityKind)] = value;
  }
  public Guid? EntityId
  {
    get => (Guid?)Data[nameof(EntityId)];
    private set => Data[nameof(EntityId)] = value;
  }
  public Guid? WorldId
  {
    get => (Guid?)Data[nameof(WorldId)];
    private set => Data[nameof(WorldId)] = value;
  }

  public override Error Error => new(this.GetErrorCode(), ErrorMessage);

  public PermissionDeniedException(UserId? userId, string action, Entity? entity, WorldId? worldId)
    : base(BuildMessage(userId, action, entity, worldId))
  {
    UserId = userId?.Value;
    Action = action;
    EntityKind = entity?.Kind;
    EntityId = entity?.Id;
    WorldId = worldId?.ToGuid();
  }

  private static string BuildMessage(UserId? userId, string action, Entity? entity, WorldId? worldId) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(UserId), userId, "<null>")
    .AddData(nameof(Action), action)
    .AddData(nameof(EntityKind), entity?.Kind, "<null>")
    .AddData(nameof(EntityId), entity?.Id, "<null>")
    .AddData(nameof(WorldId), worldId?.ToGuid(), "<null>")
    .Build();
}
