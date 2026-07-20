using Logitar.CQRS;
using SkillCraft.Api.Core.Languages.Events;
using SkillCraft.Api.Core.Languages.Models;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Scripts;

namespace SkillCraft.Api.Core.Languages.Commands;

internal record UpdateLanguageCommand(Guid Id, UpdateLanguagePayload Payload) : ICommand<LanguageModel?>;

internal class UpdateLanguageCommandHandler : ICommandHandler<UpdateLanguageCommand, LanguageModel?>
{
  private readonly IContext _context;
  private readonly ILanguageRepository _languageRepository;
  private readonly IPermissionService _permissionService;
  private readonly IScriptRepository _scriptRepository;

  public UpdateLanguageCommandHandler(
    IContext context,
    ILanguageRepository languageRepository,
    IPermissionService permissionService,
    IScriptRepository scriptRepository)
  {
    _context = context;
    _languageRepository = languageRepository;
    _permissionService = permissionService;
    _scriptRepository = scriptRepository;
  }

  public async Task<LanguageModel?> HandleAsync(UpdateLanguageCommand command, CancellationToken cancellationToken)
  {
    UpdateLanguagePayload payload = command.Payload;
    payload.Validate();

    Language? language = await _languageRepository.LoadAsync(command.Id, cancellationToken);
    if (language is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Update, language, cancellationToken);

    Script? script = null;
    if (payload.ScriptId is not null && payload.ScriptId.Value.HasValue)
    {
      script = await _scriptRepository.LoadAsync(payload.ScriptId.Value.Value, cancellationToken)
        ?? throw new ResourceNotFoundException(new ResourceIdentifier(Script.ResourceKind, payload.ScriptId.Value.Value, _context.WorldId), nameof(Language.ScriptId));
    }

    LanguageUpdated record = language.Update(
      string.IsNullOrWhiteSpace(payload.Name) ? language.Name : payload.Name,
      payload.Summary is null ? language.Summary : payload.Summary.Value,
      payload.HtmlContent is null ? language.HtmlContent : payload.HtmlContent.Value,
      payload.ScriptId is null ? language.Script : script,
      payload.TypicalSpeakers is null ? language.TypicalSpeakers : payload.TypicalSpeakers.Value,
      _context.UserId);
    _languageRepository.Update(language, record);

    await _context.SaveChangesAsync(cancellationToken);

    return await _languageRepository.ReadAsync(language, cancellationToken);
  }
}
