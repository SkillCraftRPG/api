using Logitar.CQRS;
using Logitar.EventSourcing;
using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Lineages.Models;
using SkillCraft.Api.Core.Permissions;

namespace SkillCraft.Api.Core.Lineages.Commands;

internal record UpdateLineageCommand(Guid Id, UpdateLineagePayload Payload) : ICommand<LineageModel?>;

internal class UpdateLineageCommandHandler : ICommandHandler<UpdateLineageCommand, LineageModel?>
{
  private readonly IContext _context;
  private readonly ILanguageRepository _languageRepository;
  private readonly ILineageQuerier _lineageQuerier;
  private readonly ILineageRepository _lineageRepository;
  private readonly IPermissionService _permissionService;

  public UpdateLineageCommandHandler(
    IContext context,
    ILanguageRepository languageRepository,
    ILineageQuerier lineageQuerier,
    ILineageRepository lineageRepository,
    IPermissionService permissionService)
  {
    _context = context;
    _languageRepository = languageRepository;
    _lineageQuerier = lineageQuerier;
    _lineageRepository = lineageRepository;
    _permissionService = permissionService;
  }

  public async Task<LineageModel?> HandleAsync(UpdateLineageCommand command, CancellationToken cancellationToken)
  {
    UpdateLineagePayload payload = command.Payload;
    payload.Validate();

    LineageId lineageId = new(_context.WorldId, command.Id);
    Lineage? lineage = await _lineageRepository.LoadAsync(lineageId, cancellationToken);
    if (lineage is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Update, lineage, cancellationToken);

    ActorId? actorId = _context.ActorId;

    Name? name = Name.TryCreate(payload.Name);
    if (name is not null)
    {
      lineage.Rename(name, actorId);
    }

    if (payload.Summary is not null || payload.Content is not null)
    {
      lineage.Edit(
        payload.Summary is null ? lineage.Summary : Summary.TryCreate(payload.Summary.Value),
        payload.Content is null ? lineage.Content : Content.TryCreate(payload.Content.Value),
        actorId);
    }

    if (payload.Languages is not null)
    {
      LineageLanguages languages = await LineageHelper.GetLanguagesAsync(_languageRepository, _context.WorldId, payload.Languages, cancellationToken);
      lineage.SetLanguages(languages, actorId);
    }

    if (payload.Names is not null)
    {
      LineageNames names = LineageHelper.GetNames(payload.Names);
      lineage.SetNames(names, actorId);
    }

    if (payload.Speeds is not null)
    {
      LineageSpeeds speeds = new(payload.Speeds);
      lineage.SetSpeeds(speeds, actorId);
    }

    if (payload.Size is not null || payload.Weight is not null || payload.Age is not null)
    {
      LineageSize size = payload.Size is null ? lineage.Size : LineageHelper.GetSize(payload.Size);
      LineageWeight weight = payload.Weight is null ? lineage.Weight : LineageHelper.GetWeight(payload.Weight);
      LineageAge age = payload.Age is null ? lineage.Age : new(payload.Age);
      lineage.SetTraits(size, weight, age, actorId);
    }

    await _lineageRepository.SaveAsync(lineage, cancellationToken);

    return await _lineageQuerier.ReadAsync(lineage, cancellationToken);
  }
}
