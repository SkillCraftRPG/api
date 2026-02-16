using FluentValidation;
using Logitar.CQRS;
using SkillCraft.Api.Contracts.Languages;
using SkillCraft.Api.Core.Languages.Validators;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Scripts;
using SkillCraft.Api.Core.Storages;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Languages.Commands;

internal record UpdateLanguageCommand(Guid Id, UpdateLanguagePayload Payload) : ICommand<LanguageModel?>;

internal class UpdateLanguageCommandHandler : ICommandHandler<UpdateLanguageCommand, LanguageModel?>
{
  private readonly IContext _context;
  private readonly IPermissionService _permissionService;
  private readonly ILanguageQuerier _languageQuerier;
  private readonly ILanguageRepository _languageRepository;
  private readonly IScriptRepository _scriptRepository;
  private readonly IStorageService _storageService;

  public UpdateLanguageCommandHandler(
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

  public async Task<LanguageModel?> HandleAsync(UpdateLanguageCommand command, CancellationToken cancellationToken)
  {
    UpdateLanguagePayload payload = command.Payload;
    new UpdateLanguageValidator().ValidateAndThrow(payload);

    WorldId worldId = _context.WorldId;

    LanguageId languageId = new(command.Id, worldId);
    Language? language = await _languageRepository.LoadAsync(languageId, cancellationToken);
    if (language is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Update, language, cancellationToken);

    if (!string.IsNullOrWhiteSpace(payload.Name))
    {
      language.Name = new Name(payload.Name);
    }
    if (payload.Summary is not null)
    {
      language.Summary = Summary.TryCreate(payload.Summary.Value);
    }
    if (payload.Description is not null)
    {
      language.Description = Description.TryCreate(payload.Description.Value);
    }

    if (payload.ScriptId is not null)
    {
      Script? script = null;
      if (payload.ScriptId.Value.HasValue)
      {
        ScriptId scriptId = new(payload.ScriptId.Value.Value, worldId);
        script = await _scriptRepository.LoadAsync(scriptId, cancellationToken)
          ?? throw new EntityNotFoundException(new Entity(Script.EntityKind, payload.ScriptId.Value.Value), nameof(payload.ScriptId));
      }
      language.SetScript(script);
    }
    if (payload.TypicalSpeakers is not null)
    {
      language.TypicalSpeakers = TypicalSpeakers.TryCreate(payload.TypicalSpeakers.Value);
    }

    language.Update(_context.UserId);

    await _storageService.ExecuteWithQuotaAsync(
      language,
      async () => await _languageRepository.SaveAsync(language, cancellationToken),
      cancellationToken);

    return await _languageQuerier.ReadAsync(language, cancellationToken);
  }
}
