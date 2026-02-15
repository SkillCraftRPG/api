using Logitar.CQRS;
using SkillCraft.Api.Contracts.Castes;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Storages;

namespace SkillCraft.Api.Core.Castes.Commands;

internal record DeleteCasteCommand(Guid Id) : ICommand<CasteModel?>;

internal class DeleteCasteCommandHandler : ICommandHandler<DeleteCasteCommand, CasteModel?>
{
  private readonly IContext _context;
  private readonly ICasteQuerier _casteQuerier;
  private readonly ICasteRepository _casteRepository;
  private readonly IPermissionService _permissionService;
  private readonly IStorageService _storageService;

  public DeleteCasteCommandHandler(
    IContext context,
    ICasteQuerier casteQuerier,
    ICasteRepository casteRepository,
    IPermissionService permissionService,
    IStorageService storageService)
  {
    _context = context;
    _casteQuerier = casteQuerier;
    _casteRepository = casteRepository;
    _permissionService = permissionService;
    _storageService = storageService;
  }

  public async Task<CasteModel?> HandleAsync(DeleteCasteCommand command, CancellationToken cancellationToken)
  {
    CasteId casteId = new(command.Id, _context.WorldId);
    Caste? caste = await _casteRepository.LoadAsync(casteId, cancellationToken);
    if (caste is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Delete, caste, cancellationToken);
    CasteModel model = await _casteQuerier.ReadAsync(caste, cancellationToken);

    caste.Delete(_context.UserId);

    await _storageService.ExecuteWithQuotaAsync(
      caste,
      async () => await _casteRepository.SaveAsync(caste, cancellationToken),
      cancellationToken);

    return model;
  }
}
