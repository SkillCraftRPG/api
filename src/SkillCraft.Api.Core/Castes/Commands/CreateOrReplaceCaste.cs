using Logitar.CQRS;
using Logitar.EventSourcing;
using SkillCraft.Api.Core.Castes.Models;
using SkillCraft.Api.Core.Features;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Castes.Commands;

internal record CreateOrReplaceCasteCommand(CreateOrReplaceCastePayload Payload, Guid? Id) : ICommand<CreateOrReplaceCasteResult>;

internal class CreateOrReplaceCasteCommandHandler : ICommandHandler<CreateOrReplaceCasteCommand, CreateOrReplaceCasteResult>
{
  private readonly ICasteQuerier _casteQuerier;
  private readonly ICasteRepository _casteRepository;
  private readonly IContext _context;
  private readonly IPermissionService _permissionService;

  public CreateOrReplaceCasteCommandHandler(
    ICasteQuerier casteQuerier,
    ICasteRepository casteRepository,
    IContext context,
    IPermissionService permissionService)
  {
    _casteQuerier = casteQuerier;
    _casteRepository = casteRepository;
    _context = context;
    _permissionService = permissionService;
  }

  public async Task<CreateOrReplaceCasteResult> HandleAsync(CreateOrReplaceCasteCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceCastePayload payload = command.Payload;
    payload.Validate();

    ActorId? actorId = _context.ActorId;
    WorldId worldId = _context.WorldId;

    Caste? caste = null;
    CasteId casteId = CasteId.NewId(worldId);
    if (command.Id.HasValue)
    {
      casteId = new CasteId(worldId, command.Id.Value);
      caste = await _casteRepository.LoadAsync(casteId, cancellationToken);
    }

    Name name = new(payload.Name);
    Roll? wealthRoll = Roll.TryCreate(payload.WealthRoll);
    Feature? feature = payload.Feature is null
      ? null
      : new Feature(new Name(payload.Feature.Name), Content.TryCreate(payload.Feature.Content));

    bool created = false;
    if (caste is null)
    {
      await _permissionService.CheckAsync(Actions.CreateCaste, cancellationToken);

      caste = new Caste(casteId, name, actorId);
      created = true;
    }
    else
    {
      await _permissionService.CheckAsync(Actions.Update, caste, cancellationToken);

      caste.Rename(name, actorId);
    }

    caste.Edit(Summary.TryCreate(payload.Summary), Content.TryCreate(payload.Content), actorId);
    caste.SetRules(payload.Skill, wealthRoll, feature, actorId);

    await _casteRepository.SaveAsync(caste, cancellationToken);

    CasteModel model = await _casteQuerier.ReadAsync(caste, cancellationToken);
    return new CreateOrReplaceCasteResult(model, created);
  }
}
