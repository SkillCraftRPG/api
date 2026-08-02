using Logitar;
using Logitar.CQRS;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Scripts.Events;
using SkillCraft.Api.Core.Scripts.Models;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Scripts.Commands;

internal record CreateOrReplaceScriptCommand(CreateOrReplaceScriptPayload Payload, Guid? Id) : ICommand<CreateOrReplaceScriptResult>;

internal class CreateOrReplaceScriptCommandHandler : ICommandHandler<CreateOrReplaceScriptCommand, CreateOrReplaceScriptResult>
{
  private readonly IContext _context;
  private readonly IPermissionService _permissionService;
  private readonly IScriptRepository _scriptRepository;
  private readonly IWorldRepository _worldRepository;

  public CreateOrReplaceScriptCommandHandler(
    IContext context,
    IPermissionService permissionService,
    IScriptRepository scriptRepository,
    IWorldRepository worldRepository)
  {
    _context = context;
    _permissionService = permissionService;
    _scriptRepository = scriptRepository;
    _worldRepository = worldRepository;
  }

  public async Task<CreateOrReplaceScriptResult> HandleAsync(CreateOrReplaceScriptCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceScriptPayload payload = command.Payload;
    payload.Validate();

    Script? script = null;
    if (command.Id.HasValue)
    {
      script = await _scriptRepository.LoadAsync(command.Id.Value, cancellationToken);
    }

    Guid userId = _context.UserId;
    Guid worldId = _context.WorldUid;

    ScriptSnapshot? snapshot = null;
    if (script is null)
    {
      World world = await _worldRepository.LoadAsync(worldId, cancellationToken)
        ?? throw new InvalidOperationException($"The world 'Id={worldId}' was not found.");
      await _permissionService.CheckAsync(Actions.CreateScript, world, cancellationToken);

      script = new Script(world, command.Id, userId);
      _scriptRepository.Add(script);
    }
    else
    {
      await _permissionService.CheckAsync(Actions.Update, script, cancellationToken);

      snapshot = new ScriptSnapshot(script);
    }

    script.Name = payload.Name.Trim();
    script.Summary = payload.Summary?.CleanTrim();
    script.Content = payload.Content?.CleanTrim();

    if (snapshot is not null)
    {
      ScriptUpdated? record = snapshot.Compare(script);
      if (record is not null)
      {
        script.Update(userId);
        _scriptRepository.Update(script, record);
      }
    }

    await _context.SaveChangesAsync(cancellationToken);

    ScriptModel model = await _scriptRepository.ReadAsync(script, cancellationToken);
    return new CreateOrReplaceScriptResult(model, Created: snapshot is null);
  }
}
