using Logitar.CQRS;
using Logitar.EventSourcing;
using SkillCraft.Api.Core.Castes.Models;
using SkillCraft.Api.Core.Features;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Castes.Commands;

internal record UpdateCasteCommand(Guid Id, UpdateCastePayload Payload) : ICommand<CasteModel?>;

internal class UpdateCasteCommandHandler : ICommandHandler<UpdateCasteCommand, CasteModel?>
{
  private readonly ICasteQuerier _casteQuerier;
  private readonly ICasteRepository _casteRepository;
  private readonly IContext _context;
  private readonly IPermissionService _permissionService;

  public UpdateCasteCommandHandler(
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

  public async Task<CasteModel?> HandleAsync(UpdateCasteCommand command, CancellationToken cancellationToken)
  {
    UpdateCastePayload payload = command.Payload;
    payload.Validate();

    ActorId? actorId = _context.ActorId;
    WorldId worldId = _context.WorldId;

    CasteId casteId = new(worldId, command.Id);
    Caste? caste = await _casteRepository.LoadAsync(casteId, cancellationToken);
    if (caste is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Update, caste, cancellationToken);

    Name? name = Name.TryCreate(payload.Name);
    if (name is not null)
    {
      caste.Rename(name, actorId);
    }

    if (payload.Summary is not null || payload.Content is not null)
    {
      caste.Edit(
        payload.Summary is null ? caste.Summary : Summary.TryCreate(payload.Summary.Value),
        payload.Content is null ? caste.Content : Content.TryCreate(payload.Content.Value),
        actorId);
    }

    if (payload.Skill is not null || payload.WealthRoll is not null || payload.Feature is not null)
    {
      Skill? skill = payload.Skill is null ? caste.Skill : payload.Skill.Value;
      Roll? wealthRoll = payload.WealthRoll is null ? caste.WealthRoll : Roll.TryCreate(payload.WealthRoll.Value);
      Feature? feature = payload.Feature is null
        ? caste.Feature
        : payload.Feature.Value is null
          ? null
          : new Feature(new Name(payload.Feature.Value.Name), Content.TryCreate(payload.Feature.Value.Content));

      caste.SetRules(skill, wealthRoll, feature, actorId);
    }

    await _casteRepository.SaveAsync(caste, cancellationToken);

    return await _casteQuerier.ReadAsync(caste, cancellationToken);
  }
}
