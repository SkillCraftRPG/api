using Logitar.CQRS;
using SkillCraft.Api.Contracts.Talents;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Storages;

namespace SkillCraft.Api.Core.Talents.Commands;

internal record DeleteTalentCommand(Guid Id) : ICommand<TalentModel?>;

internal class DeleteTalentCommandHandler : ICommandHandler<DeleteTalentCommand, TalentModel?>
{
  private readonly IContext _context;
  private readonly IPermissionService _permissionService;
  private readonly ITalentQuerier _talentQuerier;
  private readonly ITalentRepository _talentRepository;
  private readonly IStorageService _storageService;

  public DeleteTalentCommandHandler(
    IContext context,
    IPermissionService permissionService,
    ITalentQuerier talentQuerier,
    ITalentRepository talentRepository,
    IStorageService storageService)
  {
    _context = context;
    _permissionService = permissionService;
    _talentQuerier = talentQuerier;
    _talentRepository = talentRepository;
    _storageService = storageService;
  }

  public async Task<TalentModel?> HandleAsync(DeleteTalentCommand command, CancellationToken cancellationToken)
  {
    TalentId talentId = new(command.Id, _context.WorldId);
    Talent? talent = await _talentRepository.LoadAsync(talentId, cancellationToken);
    if (talent is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Delete, talent, cancellationToken);
    TalentModel model = await _talentQuerier.ReadAsync(talent, cancellationToken);

    talent.Delete(_context.UserId);

    await _storageService.ExecuteWithQuotaAsync(
      talent,
      async () => await _talentRepository.SaveAsync(talent, cancellationToken),
      cancellationToken);

    return model;
  }
}
