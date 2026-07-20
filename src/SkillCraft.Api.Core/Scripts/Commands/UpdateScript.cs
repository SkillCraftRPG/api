using Logitar.CQRS;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Scripts.Events;
using SkillCraft.Api.Core.Scripts.Models;

namespace SkillCraft.Api.Core.Scripts.Commands;

internal record UpdateScriptCommand(Guid Id, UpdateScriptPayload Payload) : ICommand<ScriptModel?>;

internal class UpdateScriptCommandHandler : ICommandHandler<UpdateScriptCommand, ScriptModel?>
{
  private readonly IContext _context;
  private readonly IPermissionService _permissionService;
  private readonly IScriptRepository _scriptRepository;

  public UpdateScriptCommandHandler(IContext context, IPermissionService permissionService, IScriptRepository scriptRepository)
  {
    _context = context;
    _permissionService = permissionService;
    _scriptRepository = scriptRepository;
  }

  public async Task<ScriptModel?> HandleAsync(UpdateScriptCommand command, CancellationToken cancellationToken)
  {
    UpdateScriptPayload payload = command.Payload;
    payload.Validate();

    Script? script = await _scriptRepository.LoadAsync(command.Id, cancellationToken);
    if (script is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Update, script, cancellationToken);

    ScriptUpdated record = script.Update(
      string.IsNullOrWhiteSpace(payload.Name) ? script.Name : payload.Name,
      payload.Description is null ? script.Description : payload.Description.Value,
      _context.UserId);
    _scriptRepository.Update(script, record);

    await _context.SaveChangesAsync(cancellationToken);

    return await _scriptRepository.ReadAsync(script, cancellationToken);
  }
}
