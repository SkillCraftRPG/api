using Logitar.CQRS;
using Logitar.EventSourcing;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Scripts.Models;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Scripts.Commands;

internal record UpdateScriptCommand(Guid Id, UpdateScriptPayload Payload) : ICommand<ScriptModel?>;

internal class UpdateScriptCommandHandler : ICommandHandler<UpdateScriptCommand, ScriptModel?>
{
  private readonly IContext _context;
  private readonly IPermissionService _permissionService;
  private readonly IScriptQuerier _scriptQuerier;
  private readonly IScriptRepository _scriptRepository;

  public UpdateScriptCommandHandler(
    IContext context,
    IPermissionService permissionService,
    IScriptQuerier scriptQuerier,
    IScriptRepository scriptRepository)
  {
    _context = context;
    _permissionService = permissionService;
    _scriptQuerier = scriptQuerier;
    _scriptRepository = scriptRepository;
  }

  public async Task<ScriptModel?> HandleAsync(UpdateScriptCommand command, CancellationToken cancellationToken)
  {
    UpdateScriptPayload payload = command.Payload;
    payload.Validate();

    ActorId? actorId = _context.ActorId;
    WorldId worldId = _context.WorldId;

    ScriptId scriptId = new(worldId, command.Id);
    Script? script = await _scriptRepository.LoadAsync(scriptId, cancellationToken);
    if (script is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Update, script, cancellationToken);

    Name? name = Name.TryCreate(payload.Name);
    if (name is not null)
    {
      script.Rename(name, actorId);
    }

    if (payload.Summary is not null || payload.Content is not null)
    {
      script.Edit(
        payload.Summary is null ? script.Summary : Summary.TryCreate(payload.Summary.Value),
        payload.Content is null ? script.Content : Content.TryCreate(payload.Content.Value),
        actorId);
    }

    await _scriptRepository.SaveAsync(script, cancellationToken);

    return await _scriptQuerier.ReadAsync(script, cancellationToken);
  }
}
