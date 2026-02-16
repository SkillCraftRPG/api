using FluentValidation;
using Logitar.CQRS;
using SkillCraft.Api.Contracts.Languages;
using SkillCraft.Api.Core.Languages.Validators;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Scripts;
using SkillCraft.Api.Core.Storages;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Languages.Commands;

internal record CreateOrReplaceLanguageCommand(CreateOrReplaceLanguagePayload Payload, Guid? Id) : ICommand<CreateOrReplaceLanguageResult>;

internal class CreateOrReplaceLanguageCommandHandler : ICommandHandler<CreateOrReplaceLanguageCommand, CreateOrReplaceLanguageResult>
{
  private readonly IContext _context;
  private readonly IPermissionService _permissionService;
  private readonly ILanguageQuerier _languageQuerier;
  private readonly ILanguageRepository _languageRepository;
  private readonly IScriptRepository _scriptRepository;
  private readonly IStorageService _storageService;

  public CreateOrReplaceLanguageCommandHandler(
    IContext context,
    IPermissionService permissionService,
    ILanguageQuerier languageQuerier,
    ILanguageRepository languageRepository,
    IScriptRepository scriptRepository,
    IStorageService storageService)
  {
    _context = context;
    _permissionService = permissionService;
    _languageQuerier = languageQuerier;
    _languageRepository = languageRepository;
    _scriptRepository = scriptRepository;
    _storageService = storageService;
  }

  public async Task<CreateOrReplaceLanguageResult> HandleAsync(CreateOrReplaceLanguageCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceLanguagePayload payload = command.Payload;
    new CreateOrReplaceLanguageValidator().ValidateAndThrow(payload);

    UserId userId = _context.UserId;
    WorldId worldId = _context.WorldId;

    LanguageId languageId = LanguageId.NewId(worldId);
    Language? language = null;
    if (command.Id.HasValue)
    {
      languageId = new(command.Id.Value, worldId);
      language = await _languageRepository.LoadAsync(languageId, cancellationToken);
    }

    Name name = new(payload.Name);

    bool created = false;
    if (language is null)
    {
      await _permissionService.CheckAsync(Actions.CreateLanguage, cancellationToken);

      language = new Language(worldId, name, userId, languageId);
      created = true;
    }
    else
    {
      await _permissionService.CheckAsync(Actions.Update, language, cancellationToken);

      language.Name = name;
    }

    language.Summary = Summary.TryCreate(payload.Summary);
    language.Description = Description.TryCreate(payload.Description);

    await SetScriptAsync(language, payload, worldId, cancellationToken);
    language.TypicalSpeakers = TypicalSpeakers.TryCreate(payload.TypicalSpeakers);

    language.Update(userId);

    await _storageService.ExecuteWithQuotaAsync(
      language,
      async () => await _languageRepository.SaveAsync(language, cancellationToken),
      cancellationToken);

    LanguageModel model = await _languageQuerier.ReadAsync(language, cancellationToken);
    return new CreateOrReplaceLanguageResult(model, created);
  }

  private async Task SetScriptAsync(Language language, CreateOrReplaceLanguagePayload payload, WorldId worldId, CancellationToken cancellationToken)
  {
    Script? script = null;
    if (payload.ScriptId.HasValue)
    {
      ScriptId scriptId = new(payload.ScriptId.Value, worldId);
      script = await _scriptRepository.LoadAsync(scriptId, cancellationToken)
        ?? throw new EntityNotFoundException(new Entity(Script.EntityKind, payload.ScriptId.Value), nameof(payload.ScriptId));
    }
    language.SetScript(script);
  }
}
