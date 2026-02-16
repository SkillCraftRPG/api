using FluentValidation;
using Logitar.CQRS;
using SkillCraft.Api.Contracts.Scripts;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Scripts.Validators;
using SkillCraft.Api.Core.Storages;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Scripts.Commands;

internal record CreateOrReplaceScriptCommand(CreateOrReplaceScriptPayload Payload, Guid? Id) : ICommand<CreateOrReplaceScriptResult>;

internal class CreateOrReplaceScriptCommandHandler : ICommandHandler<CreateOrReplaceScriptCommand, CreateOrReplaceScriptResult>
{
  private readonly IContext _context;
  private readonly IPermissionService _permissionService;
  private readonly IScriptQuerier _scriptQuerier;
  private readonly IScriptRepository _scriptRepository;
  private readonly IStorageService _storageService;

  public CreateOrReplaceScriptCommandHandler(
    IContext context,
    IPermissionService permissionService,
    IScriptQuerier scriptQuerier,
    IScriptRepository scriptRepository,
    IStorageService storageService)
  {
    _context = context;
    _permissionService = permissionService;
    _scriptQuerier = scriptQuerier;
    _scriptRepository = scriptRepository;
    _storageService = storageService;
  }

  public async Task<CreateOrReplaceScriptResult> HandleAsync(CreateOrReplaceScriptCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceScriptPayload payload = command.Payload;
    new CreateOrReplaceScriptValidator().ValidateAndThrow(payload);

    UserId userId = _context.UserId;
    WorldId worldId = _context.WorldId;

    ScriptId scriptId = ScriptId.NewId(worldId);
    Script? script = null;
    if (command.Id.HasValue)
    {
      scriptId = new(command.Id.Value, worldId);
      script = await _scriptRepository.LoadAsync(scriptId, cancellationToken);
    }

    Name name = new(payload.Name);

    bool created = false;
    if (script is null)
    {
      await _permissionService.CheckAsync(Actions.CreateScript, cancellationToken);

      script = new Script(worldId, name, userId, scriptId);
      created = true;
    }
    else
    {
      await _permissionService.CheckAsync(Actions.Update, script, cancellationToken);

      script.Name = name;
    }

    script.Summary = Summary.TryCreate(payload.Summary);
    script.Description = Description.TryCreate(payload.Description);

    script.Update(userId);

    await _storageService.ExecuteWithQuotaAsync(
      script,
      async () => await _scriptRepository.SaveAsync(script, cancellationToken),
      cancellationToken);

    ScriptModel model = await _scriptQuerier.ReadAsync(script, cancellationToken);
    return new CreateOrReplaceScriptResult(model, created);
  }
}
