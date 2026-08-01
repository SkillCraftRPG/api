using Logitar;
using Logitar.CQRS;
using SkillCraft.Api.Core.Features;
using SkillCraft.Api.Core.Lineages.Events;
using SkillCraft.Api.Core.Lineages.Models;
using SkillCraft.Api.Core.Permissions;

namespace SkillCraft.Api.Core.Lineages.Commands;

internal record CreateOrReplaceLineageFeatureCommand(Guid LineageId, FeatureModel Payload, Guid? FeatureId) : ICommand<CreateOrReplaceLineageFeatureResult>;

internal class CreateOrReplaceLineageFeatureCommandHandler : ICommandHandler<CreateOrReplaceLineageFeatureCommand, CreateOrReplaceLineageFeatureResult>
{
  private readonly IContext _context;
  private readonly ILineageRepository _lineageRepository;
  private readonly IPermissionService _permissionService;

  public CreateOrReplaceLineageFeatureCommandHandler(IContext context, ILineageRepository lineageRepository, IPermissionService permissionService)
  {
    _context = context;
    _lineageRepository = lineageRepository;
    _permissionService = permissionService;
  }

  public async Task<CreateOrReplaceLineageFeatureResult> HandleAsync(CreateOrReplaceLineageFeatureCommand command, CancellationToken cancellationToken)
  {
    FeatureModel payload = command.Payload;
    payload.Validate();

    Lineage lineage = await _lineageRepository.LoadAsync(command.LineageId, cancellationToken)
      ?? throw new ResourceNotFoundException(new ResourceIdentifier(Lineage.ResourceKind, command.LineageId, _context.WorldId), nameof(command.LineageId));
    await _permissionService.CheckAsync(Actions.Update, lineage, cancellationToken);

    LineageFeature? feature = command.FeatureId.HasValue
      ? lineage.Features.SingleOrDefault(feature => feature.Id == command.FeatureId.Value)
      : null;

    Guid userId = _context.UserId;

    LineageFeatureSnapshot? snapshot = null;
    if (feature is null)
    {
      feature = new LineageFeature(lineage, userId, command.FeatureId);
    }
    else
    {
      snapshot = new LineageFeatureSnapshot(feature);
    }

    feature.Name = payload.Name.Trim();
    feature.Content = payload.Content?.CleanTrim();

    if (snapshot is null)
    {
      lineage.Features.Add(feature);
      lineage.Update(userId, feature.CreatedOn);
      _lineageRepository.Add(feature);
    }
    else
    {
      LineageFeatureUpdated? record = snapshot.Compare(feature);
      if (record is not null)
      {
        feature.Update(userId);
        lineage.Update(feature.UpdatedBy, feature.UpdatedOn);
        _lineageRepository.Update(feature, record);
      }
    }

    await _context.SaveChangesAsync(cancellationToken);

    LineageModel model = await _lineageRepository.ReadAsync(lineage, cancellationToken);
    return new CreateOrReplaceLineageFeatureResult(model, feature.Id, Created: snapshot is null);
  }
}
