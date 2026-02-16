using Logitar.CQRS;
using SkillCraft.Api.Contracts.Scripts;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Storages;

namespace SkillCraft.Api.Core.Scripts.Commands;

internal record DeleteScriptCommand(Guid Id) : ICommand<ScriptModel?>;

internal class DeleteScriptCommandHandler : ICommandHandler<DeleteScriptCommand, ScriptModel?>
{
  private readonly IContext _context;
  private readonly IPermissionService _permissionService;
  private readonly IScriptQuerier _scriptQuerier;
  private readonly IScriptRepository _scriptRepository;
  private readonly IStorageService _storageService;

  public DeleteScriptCommandHandler(
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

  public async Task<ScriptModel?> HandleAsync(DeleteScriptCommand command, CancellationToken cancellationToken)
  {
    ScriptId scriptId = new(command.Id, _context.WorldId);
    Script? script = await _scriptRepository.LoadAsync(scriptId, cancellationToken);
    if (script is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Delete, script, cancellationToken);
    ScriptModel model = await _scriptQuerier.ReadAsync(script, cancellationToken);

    script.Delete(_context.UserId);

    await _storageService.ExecuteWithQuotaAsync(
      script,
      async () => await _scriptRepository.SaveAsync(script, cancellationToken),
      cancellationToken);

    return model;
  }
}
