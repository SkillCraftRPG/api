using Logitar.CQRS;
using SkillCraft.Api.Core.Lineages.Models;
using SkillCraft.Api.Core.Permissions;

namespace SkillCraft.Api.Core.Lineages.Commands;

internal record DeleteLineageFeatureCommand(Guid LineageId, Guid FeatureId) : ICommand<LineageModel?>;

internal class DeleteLineageFeatureCommandHandler : ICommandHandler<DeleteLineageFeatureCommand, LineageModel?>
{
  private readonly IContext _context;
  private readonly ILineageRepository _lineageRepository;
  private readonly IPermissionService _permissionService;

  public DeleteLineageFeatureCommandHandler(IContext context, ILineageRepository lineageRepository, IPermissionService permissionService)
  {
    _context = context;
    _lineageRepository = lineageRepository;
    _permissionService = permissionService;
  }

  public async Task<LineageModel?> HandleAsync(DeleteLineageFeatureCommand command, CancellationToken cancellationToken)
  {
    Lineage? lineage = await _lineageRepository.LoadAsync(command.LineageId, cancellationToken);
    if (lineage is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Update, lineage, cancellationToken);

    LineageFeature? feature = lineage.Features.SingleOrDefault(feature => feature.Id == command.FeatureId);
    if (feature is null)
    {
      return null;
    }

    lineage.Features.Remove(feature);
    lineage.Update(_context.UserId);
    _lineageRepository.Remove(feature);

    await _context.SaveChangesAsync(cancellationToken);

    return await _lineageRepository.ReadAsync(lineage, cancellationToken);
  }
}
