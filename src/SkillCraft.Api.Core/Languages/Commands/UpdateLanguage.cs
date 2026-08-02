using Logitar.CQRS;
using Logitar.EventSourcing;
using SkillCraft.Api.Core.Languages.Models;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Scripts;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Languages.Commands;

internal record UpdateLanguageCommand(Guid Id, UpdateLanguagePayload Payload) : ICommand<LanguageModel?>;

internal class UpdateLanguageCommandHandler : ICommandHandler<UpdateLanguageCommand, LanguageModel?>
{
  private readonly IContext _context;
  private readonly ILanguageQuerier _languageQuerier;
  private readonly ILanguageRepository _languageRepository;
  private readonly IPermissionService _permissionService;
  private readonly IScriptRepository _scriptRepository;

  public UpdateLanguageCommandHandler(
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

  public async Task<LanguageModel?> HandleAsync(UpdateLanguageCommand command, CancellationToken cancellationToken)
  {
    UpdateLanguagePayload payload = command.Payload;
    payload.Validate();

    ActorId? actorId = _context.ActorId;
    WorldId worldId = _context.WorldId;

    LanguageId languageId = new(worldId, command.Id);
    Language? language = await _languageRepository.LoadAsync(languageId, cancellationToken);
    if (language is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Update, language, cancellationToken);

    Name? name = Name.TryCreate(payload.Name);
    if (name is not null)
    {
      language.Rename(name, actorId);
    }

    if (payload.Summary is not null || payload.Content is not null)
    {
      language.Edit(
        payload.Summary is null ? language.Summary : Summary.TryCreate(payload.Summary.Value),
        payload.Content is null ? language.Content : Content.TryCreate(payload.Content.Value),
        actorId);
    }

    if (payload.TypicalSpeakers is not null || payload.ScriptId is not null)
    {
      Script? script = null;
      if (payload.ScriptId is not null && payload.ScriptId.Value.HasValue)
      {
        ScriptId scriptId = new(worldId, payload.ScriptId.Value.Value);
        script = await _scriptRepository.LoadAsync(scriptId, cancellationToken)
          ?? throw new ScriptNotFoundException(scriptId, nameof(payload.ScriptId));
      }

      language.SetRules(
        payload.TypicalSpeakers is null ? language.TypicalSpeakers : TypicalSpeakers.TryCreate(payload.TypicalSpeakers.Value),
        script,
        actorId);
    }

    await _languageRepository.SaveAsync(language, cancellationToken);

    return await _languageQuerier.ReadAsync(language, cancellationToken);
  }
}
