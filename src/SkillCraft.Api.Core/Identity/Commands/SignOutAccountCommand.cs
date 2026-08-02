using Krakenar.Contracts.Sessions;
using Logitar.CQRS;
using SkillCraft.Api.Core.Permissions;

namespace SkillCraft.Api.Core.Identity.Commands;

internal record SignOutAccountCommand(Guid? SessionId) : ICommand<bool>;

internal class SignOutAccountCommandHandler : ICommandHandler<SignOutAccountCommand, bool>
{
  private readonly IContext _context;
  private readonly ISessionGateway _sessionGateway;
  private readonly IUserGateway _userGateway;

  public SignOutAccountCommandHandler(IContext context, ISessionGateway sessionGateway, IUserGateway userGateway)
  {
    _context = context;
    _sessionGateway = sessionGateway;
    _userGateway = userGateway;
  }

  public async Task<bool> HandleAsync(SignOutAccountCommand command, CancellationToken cancellationToken)
  {
    UserId userId = _context.UserId;

    if (command.SessionId.HasValue)
    {
      Session? session = await _sessionGateway.FindAsync(command.SessionId.Value, cancellationToken);
      if (session is null)
      {
        return false;
      }
      else if (session.User.Id != userId.ResourceId)
      {
        throw new PermissionDeniedException(userId, "SignOut", new ResourceIdentifier("Session", command.SessionId.Value));
      }

      await _sessionGateway.SignOutAsync(session, cancellationToken);
    }
    else
    {
      await _userGateway.SignOutAsync(userId.ResourceId, cancellationToken);
    }

    return true;
  }
}
