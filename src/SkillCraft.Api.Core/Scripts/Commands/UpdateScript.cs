using FluentValidation;
using Logitar.CQRS;
using SkillCraft.Api.Contracts.Scripts;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Scripts.Validators;
using SkillCraft.Api.Core.Storages;

namespace SkillCraft.Api.Core.Scripts.Commands;

internal record UpdateScriptCommand(Guid Id, UpdateScriptPayload Payload) : ICommand<ScriptModel?>;

internal class UpdateScriptCommandHandler : ICommandHandler<UpdateScriptCommand, ScriptModel?>
{
  private readonly IContext _context;
  private readonly IPermissionService _permissionService;
  private readonly IScriptQuerier _scriptQuerier;
  private readonly IScriptRepository _scriptRepository;
  private readonly IStorageService _storageService;

  public UpdateScriptCommandHandler(
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

  public async Task<ScriptModel?> HandleAsync(UpdateScriptCommand command, CancellationToken cancellationToken)
  {
    UpdateScriptPayload payload = command.Payload;
    new UpdateScriptValidator().ValidateAndThrow(payload);

    ScriptId scriptId = new(command.Id, _context.WorldId);
    Script? script = await _scriptRepository.LoadAsync(scriptId, cancellationToken);
    if (script is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Update, script, cancellationToken);

    if (!string.IsNullOrWhiteSpace(payload.Name))
    {
      script.Name = new Name(payload.Name);
    }
    if (payload.Summary is not null)
    {
      script.Summary = Summary.TryCreate(payload.Summary.Value);
    }
    if (payload.Description is not null)
    {
      script.Description = Description.TryCreate(payload.Description.Value);
    }

    script.Update(_context.UserId);

    await _storageService.ExecuteWithQuotaAsync(
      script,
      async () => await _scriptRepository.SaveAsync(script, cancellationToken),
      cancellationToken);

    return await _scriptQuerier.ReadAsync(script, cancellationToken);
  }
}
