using Logitar;
using Logitar.CQRS;
using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Lineages.Events;
using SkillCraft.Api.Core.Lineages.Models;
using SkillCraft.Api.Core.Permissions;

namespace SkillCraft.Api.Core.Lineages.Commands;

internal record UpdateLineageCommand(Guid Id, UpdateLineagePayload Payload) : ICommand<LineageModel?>;

internal class UpdateLineageCommandHandler : ICommandHandler<UpdateLineageCommand, LineageModel?>
{
  private readonly IContext _context;
  private readonly ILanguageRepository _languageRepository;
  private readonly ILineageRepository _lineageRepository;
  private readonly IPermissionService _permissionService;

  public UpdateLineageCommandHandler(
    IContext context,
    ILanguageRepository languageRepository,
    ILineageRepository lineageRepository,
    IPermissionService permissionService)
  {
    _context = context;
    _languageRepository = languageRepository;
    _lineageRepository = lineageRepository;
    _permissionService = permissionService;
  }

  public async Task<LineageModel?> HandleAsync(UpdateLineageCommand command, CancellationToken cancellationToken)
  {
    UpdateLineagePayload payload = command.Payload;
    payload.Validate();

    Lineage? lineage = await _lineageRepository.LoadAsync(command.Id, cancellationToken);
    if (lineage is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Update, lineage, cancellationToken);

    LineageSnapshot snapshot = new(lineage);

    if (!string.IsNullOrWhiteSpace(payload.Name))
    {
      lineage.Name = payload.Name.Trim();
    }
    if (payload.Summary is not null)
    {
      lineage.Summary = payload.Summary.Value?.CleanTrim();
    }
    if (payload.Content is not null)
    {
      lineage.Content = payload.Content.Value?.CleanTrim();
    }

    if (payload.Languages is not null)
    {
      IReadOnlyCollection<Language> languages = [];
      if (payload.Languages.Ids.Count > 0)
      {
        languages = await _languageRepository.LoadAsync(payload.Languages.Ids, cancellationToken);

        HashSet<Guid> missingIds = payload.Languages.Ids.Except(languages.Select(language => language.Id)).ToHashSet();
        if (missingIds.Count > 0)
        {
          string propertyName = string.Join('.', nameof(payload.Languages), nameof(payload.Languages.Ids));
          throw new LanguagesNotFoundException(_context.WorldUid, missingIds, propertyName);
        }
      }
      lineage.SetLanguages(languages, payload.Languages.Extra, payload.Languages.Content);
    }
    if (payload.Names is not null)
    {
      lineage.SetNames(payload.Names.Family, payload.Names.Female, payload.Names.Male, payload.Names.Unisex, payload.Names.Custom, payload.Names.Content);
    }
    if (payload.Speeds is not null)
    {
      lineage.SetSpeeds(payload.Speeds);
    }
    if (payload.Size is not null)
    {
      lineage.SetSize(payload.Size);
    }
    if (payload.Weight is not null)
    {
      lineage.SetWeight(payload.Weight);
    }
    if (payload.Age is not null)
    {
      lineage.SetAge(payload.Age);
    }

    LineageUpdated? record = snapshot.Compare(lineage);
    if (record is not null)
    {
      lineage.Update(_context.UserId);
      _lineageRepository.Update(lineage, record);

      await _context.SaveChangesAsync(cancellationToken);
    }

    return await _lineageRepository.ReadAsync(lineage, cancellationToken);
  }
}
