using Logitar;
using Logitar.CQRS;
using SkillCraft.Api.Core.Castes.Events;
using SkillCraft.Api.Core.Castes.Models;
using SkillCraft.Api.Core.Features;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Castes.Commands;

internal record CreateOrReplaceCasteCommand(CreateOrReplaceCastePayload Payload, Guid? Id) : ICommand<CreateOrReplaceCasteResult>;

internal class CreateOrReplaceCasteCommandHandler : ICommandHandler<CreateOrReplaceCasteCommand, CreateOrReplaceCasteResult>
{
  private readonly ICasteRepository _casteRepository;
  private readonly IContext _context;
  private readonly IPermissionService _permissionService;
  private readonly IWorldRepository _worldRepository;

  public CreateOrReplaceCasteCommandHandler(
    ICasteRepository casteRepository,
    IContext context,
    IPermissionService permissionService,
    IWorldRepository worldRepository)
  {
    _casteRepository = casteRepository;
    _context = context;
    _permissionService = permissionService;
    _worldRepository = worldRepository;
  }

  public async Task<CreateOrReplaceCasteResult> HandleAsync(CreateOrReplaceCasteCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceCastePayload payload = command.Payload;
    payload.Validate();

    Caste? caste = null;
    if (command.Id.HasValue)
    {
      caste = await _casteRepository.LoadAsync(command.Id.Value, cancellationToken);
    }

    Guid userId = _context.UserId;
    Guid worldId = _context.WorldId;

    CasteSnapshot? snapshot = null;
    if (caste is null)
    {
      World world = await _worldRepository.LoadAsync(worldId, cancellationToken)
        ?? throw new InvalidOperationException($"The world 'Id={worldId}' was not found.");
      await _permissionService.CheckAsync(Actions.CreateCaste, world, cancellationToken);

      caste = new Caste(world, command.Id, userId);
      _casteRepository.Add(caste);
    }
    else
    {
      await _permissionService.CheckAsync(Actions.Update, caste, cancellationToken);

      snapshot = new CasteSnapshot(caste);
    }

    caste.Name = payload.Name.Trim();
    caste.Summary = payload.Summary?.CleanTrim();
    caste.HtmlContent = payload.HtmlContent?.CleanTrim();

    caste.Skill = payload.Skill;
    caste.WealthRoll = payload.WealthRoll?.CleanTrim()?.ToLowerInvariant();

    Feature? feature = payload.Feature is null ? null : new(payload.Feature);
    caste.FeatureName = feature?.Name;
    caste.FeatureHtmlContent = feature?.HtmlContent;

    if (snapshot is not null)
    {
      CasteUpdated? record = snapshot.Compare(caste);
      if (record is not null)
      {
        caste.Update(userId);
        _casteRepository.Update(caste, record);
      }
    }

    await _context.SaveChangesAsync(cancellationToken);

    CasteModel model = await _casteRepository.ReadAsync(caste, cancellationToken);
    return new CreateOrReplaceCasteResult(model, Created: snapshot is null);
  }
}
