using Krakenar.Contracts;
using Logitar;
using SkillCraft.Api.Core.Identity;

namespace SkillCraft.Api.Core.Permissions;

public class PermissionDeniedException : ErrorException
{
  private const string ErrorMessage = "The specified permission was denied.";

  public Guid? UserId
  {
    get => (Guid?)Data[nameof(UserId)];
    private set => Data[nameof(UserId)] = value;
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

  public override Error Error => new(this.GetErrorCode(), ErrorMessage);

  public PermissionDeniedException(UserId? userId, string action, ResourceIdentifier? resource)
    : base(BuildMessage(userId, action, resource))
  {
    UserId = userId?.ResourceId;
    Action = action;
    Resource = resource?.ToString();
  }

  private static string BuildMessage(UserId? userId, string action, ResourceIdentifier? resource) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(UserId), userId?.ResourceId, "<null>")
    .AddData(nameof(Action), action)
    .AddData(nameof(Resource), resource, "<null>")
    .Build();
}
