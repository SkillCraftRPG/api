using Logitar.CQRS;
using SkillCraft.Api.Contracts.Lineages;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Storages;

namespace SkillCraft.Api.Core.Lineages.Commands;

internal record DeleteLineageCommand(Guid Id) : ICommand<LineageModel?>;

internal class DeleteLineageCommandHandler : ICommandHandler<DeleteLineageCommand, LineageModel?>
{
  private readonly IContext _context;
  private readonly ILineageQuerier _lineageQuerier;
  private readonly ILineageRepository _lineageRepository;
  private readonly IPermissionService _permissionService;
  private readonly IStorageService _storageService;

  public DeleteLineageCommandHandler(
    IContext context,
    ILineageQuerier lineageQuerier,
    ILineageRepository lineageRepository,
    IPermissionService permissionService,
    IStorageService storageService)
  {
    _context = context;
    _lineageQuerier = lineageQuerier;
    _lineageRepository = lineageRepository;
    _permissionService = permissionService;
    _storageService = storageService;
  }

  public async Task<LineageModel?> HandleAsync(DeleteLineageCommand command, CancellationToken cancellationToken)
  {
    LineageId lineageId = new(command.Id, _context.WorldId);
    Lineage? lineage = await _lineageRepository.LoadAsync(lineageId, cancellationToken);
    if (lineage is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Delete, lineage, cancellationToken);
    LineageModel model = await _lineageQuerier.ReadAsync(lineage, cancellationToken);

    lineage.Delete(_context.UserId);

    await _storageService.ExecuteWithQuotaAsync(
      lineage,
      async () => await _lineageRepository.SaveAsync(lineage, cancellationToken),
      cancellationToken);

    return model;
  }
}
