using Logitar.CQRS;
using Logitar.EventSourcing;
using SkillCraft.Api.Core.Languages.Models;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Scripts;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Languages.Commands;

internal record CreateOrReplaceLanguageCommand(CreateOrReplaceLanguagePayload Payload, Guid? Id) : ICommand<CreateOrReplaceLanguageResult>;

internal class CreateOrReplaceLanguageCommandHandler : ICommandHandler<CreateOrReplaceLanguageCommand, CreateOrReplaceLanguageResult>
{
  private readonly IContext _context;
  private readonly ILanguageQuerier _languageQuerier;
  private readonly ILanguageRepository _languageRepository;
  private readonly IPermissionService _permissionService;
  private readonly IScriptRepository _scriptRepository;

  public CreateOrReplaceLanguageCommandHandler(
    IContext context,
    ILanguageQuerier languageQuerier,
    ILanguageRepository languageRepository,
    IPermissionService permissionService,
    IScriptRepository scriptRepository)
  {
    _context = context;
    _languageQuerier = languageQuerier;
    _languageRepository = languageRepository;
    _permissionService = permissionService;
    _scriptRepository = scriptRepository;
  }

  public async Task<CreateOrReplaceLanguageResult> HandleAsync(CreateOrReplaceLanguageCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceLanguagePayload payload = command.Payload;
    payload.Validate();

    ActorId? actorId = _context.ActorId;
    WorldId worldId = _context.WorldId;

    Language? language = null;
    LanguageId languageId = LanguageId.NewId(worldId);
    if (command.Id.HasValue)
    {
      languageId = new LanguageId(worldId, command.Id.Value);
      language = await _languageRepository.LoadAsync(languageId, cancellationToken);
    }

    Name name = new(payload.Name);

    Script? script = null;
    if (payload.ScriptId.HasValue)
    {
      ScriptId scriptId = new(worldId, payload.ScriptId.Value);
      script = await _scriptRepository.LoadAsync(scriptId, cancellationToken)
        ?? throw new ScriptNotFoundException(scriptId, nameof(payload.ScriptId));
    }

    bool created = false;
    if (language is null)
    {
      await _permissionService.CheckAsync(Actions.CreateLanguage, cancellationToken);

      language = new Language(languageId, name, actorId);
      created = true;
    }
    else
    {
      await _permissionService.CheckAsync(Actions.Update, language, cancellationToken);

      language.Rename(name, actorId);
    }

    language.Edit(Summary.TryCreate(payload.Summary), Content.TryCreate(payload.Content), actorId);
    language.SetRules(TypicalSpeakers.TryCreate(payload.TypicalSpeakers), script, actorId);

    await _languageRepository.SaveAsync(language, cancellationToken);

    LanguageModel model = await _languageQuerier.ReadAsync(language, cancellationToken);
    return new CreateOrReplaceLanguageResult(model, created);
  }
}
