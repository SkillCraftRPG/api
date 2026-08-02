using Logitar.CQRS;
using Logitar.EventSourcing;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Scripts.Models;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Scripts.Commands;

internal record CreateOrReplaceScriptCommand(CreateOrReplaceScriptPayload Payload, Guid? Id) : ICommand<CreateOrReplaceScriptResult>;

internal class CreateOrReplaceScriptCommandHandler : ICommandHandler<CreateOrReplaceScriptCommand, CreateOrReplaceScriptResult>
{
  private readonly IContext _context;
  private readonly IPermissionService _permissionService;
  private readonly IScriptQuerier _scriptQuerier;
  private readonly IScriptRepository _scriptRepository;

  public CreateOrReplaceScriptCommandHandler(
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

  public async Task<CreateOrReplaceScriptResult> HandleAsync(CreateOrReplaceScriptCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceScriptPayload payload = command.Payload;
    payload.Validate();

    ActorId? actorId = _context.ActorId;
    WorldId worldId = _context.WorldId;

    Script? script = null;
    ScriptId scriptId = ScriptId.NewId(worldId);
    if (command.Id.HasValue)
    {
      scriptId = new ScriptId(worldId, command.Id.Value);
      script = await _scriptRepository.LoadAsync(scriptId, cancellationToken);
    }

    Name name = new(payload.Name);

    bool created = false;
    if (script is null)
    {
      await _permissionService.CheckAsync(Actions.CreateScript, cancellationToken);

      script = new Script(scriptId, name, actorId);
      created = true;
    }
    else
    {
      await _permissionService.CheckAsync(Actions.Update, script, cancellationToken);

      script.Rename(name, actorId);
    }

    script.Edit(Summary.TryCreate(payload.Summary), Content.TryCreate(payload.Content), actorId);

    await _scriptRepository.SaveAsync(script, cancellationToken);

    ScriptModel model = await _scriptQuerier.ReadAsync(script, cancellationToken);
    return new CreateOrReplaceScriptResult(model, created);
  }
}
