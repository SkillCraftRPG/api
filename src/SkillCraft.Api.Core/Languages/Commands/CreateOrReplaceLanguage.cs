using Logitar;
using Logitar.CQRS;
using SkillCraft.Api.Core.Languages.Events;
using SkillCraft.Api.Core.Languages.Models;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Scripts;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Languages.Commands;

internal record CreateOrReplaceLanguageCommand(CreateOrReplaceLanguagePayload Payload, Guid? Id) : ICommand<CreateOrReplaceLanguageResult>;

internal class CreateOrReplaceLanguageCommandHandler : ICommandHandler<CreateOrReplaceLanguageCommand, CreateOrReplaceLanguageResult>
{
  private readonly IContext _context;
  private readonly ILanguageRepository _languageRepository;
  private readonly IPermissionService _permissionService;
  private readonly IScriptRepository _scriptRepository;
  private readonly IWorldRepository _worldRepository;

  public CreateOrReplaceLanguageCommandHandler(
    IContext context,
    ILanguageRepository languageRepository,
    IPermissionService permissionService,
    IScriptRepository scriptRepository,
    IWorldRepository worldRepository)
  {
    _context = context;
    _languageRepository = languageRepository;
    _permissionService = permissionService;
    _scriptRepository = scriptRepository;
    _worldRepository = worldRepository;
  }

  public async Task<CreateOrReplaceLanguageResult> HandleAsync(CreateOrReplaceLanguageCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceLanguagePayload payload = command.Payload;
    payload.Validate();

    Language? language = null;
    if (command.Id.HasValue)
    {
      language = await _languageRepository.LoadAsync(command.Id.Value, cancellationToken);
    }

    Guid userId = _context.UserId;
    Guid worldId = _context.WorldUid;

    Script? script = null;
    if (payload.ScriptId.HasValue)
    {
      script = await _scriptRepository.LoadAsync(payload.ScriptId.Value, cancellationToken)
        ?? throw new ResourceNotFoundException(new ResourceIdentifier(Script.ResourceKind, payload.ScriptId.Value, worldId), nameof(Language.ScriptId));
    }

    LanguageSnapshot? snapshot = null;
    if (language is null)
    {
      World world = await _worldRepository.LoadAsync(worldId, cancellationToken)
        ?? throw new InvalidOperationException($"The world 'Id={worldId}' was not found.");
      await _permissionService.CheckAsync(Actions.CreateLanguage, world, cancellationToken);

      language = new Language(world, command.Id, userId);
      _languageRepository.Add(language);
    }
    else
    {
      await _permissionService.CheckAsync(Actions.Update, language, cancellationToken);

      snapshot = new LanguageSnapshot(language);
    }

    language.Name = payload.Name.Trim();
    language.Summary = payload.Summary?.CleanTrim();
    language.Content = payload.Content?.CleanTrim();

    language.SetScript(script);
    language.TypicalSpeakers = payload.TypicalSpeakers?.CleanTrim();

    if (snapshot is not null)
    {
      LanguageUpdated? record = snapshot.Compare(language);
      if (record is not null)
      {
        language.Update(userId);
        _languageRepository.Update(language, record);
      }
    }

    await _context.SaveChangesAsync(cancellationToken);

    LanguageModel model = await _languageRepository.ReadAsync(language, cancellationToken);
    return new CreateOrReplaceLanguageResult(model, Created: snapshot is null);
  }
}
