using Logitar;
using Logitar.CQRS;
using SkillCraft.Api.Core.Languages.Events;
using SkillCraft.Api.Core.Languages.Models;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Scripts;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Languages.Commands;

internal record UpdateLanguageCommand(Guid Id, UpdateLanguagePayload Payload) : ICommand<LanguageModel?>;

internal class UpdateLanguageCommandHandler : ICommandHandler<UpdateLanguageCommand, LanguageModel?>
{
  private readonly IContext _context;
  private readonly ILanguageRepository _languageRepository;
  private readonly IPermissionService _permissionService;
  private readonly IScriptQuerier _scriptQuerier;
  private readonly IScriptRepository _scriptRepository;

  public UpdateLanguageCommandHandler(
    IContext context,
    ILanguageRepository languageRepository,
    IPermissionService permissionService,
    IScriptQuerier scriptQuerier,
    IScriptRepository scriptRepository)
  {
    _context = context;
    _languageRepository = languageRepository;
    _permissionService = permissionService;
    _scriptQuerier = scriptQuerier;
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

    LanguageSnapshot snapshot = new(language);

    if (!string.IsNullOrWhiteSpace(payload.Name))
    {
      language.Name = payload.Name.Trim();
    }
    if (payload.Summary is not null)
    {
      language.Summary = payload.Summary.Value?.CleanTrim();
    }
    if (payload.Content is not null)
    {
      language.Content = payload.Content.Value?.CleanTrim();
    }

    if (payload.ScriptId is not null)
    {
      int? scriptKey = null;
      Guid? scriptUid = null;
      if (payload.ScriptId.Value.HasValue)
      {
        ScriptId scriptId = new(_context.WorldId, payload.ScriptId.Value.Value);
        Script script = await _scriptRepository.LoadAsync(scriptId, cancellationToken)
          ?? throw new ResourceNotFoundException(new ResourceIdentifier(Script.ResourceKind, payload.ScriptId.Value.Value, _context.WorldUid), nameof(Language.ScriptId));
        scriptKey = await _scriptQuerier.FindKeyAsync(script.Id, cancellationToken)
          ?? throw new InvalidOperationException($"The script entity 'StreamId={script.Id}' was not found.");
        scriptUid = script.ResourceId;
      }
      language.SetScript(scriptKey, scriptUid);
    }
    if (payload.TypicalSpeakers is not null)
    {
      language.TypicalSpeakers = payload.TypicalSpeakers.Value?.CleanTrim();
    }

    LanguageUpdated? record = snapshot.Compare(language);
    if (record is not null)
    {
      language.Update(_context.UserUid);
      _languageRepository.Update(language, record);

      await _context.SaveChangesAsync(cancellationToken);
    }

    return await _languageRepository.ReadAsync(language, cancellationToken);
  }
}
