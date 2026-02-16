using Logitar.CQRS;
using SkillCraft.Api.Contracts.Languages;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Storages;

namespace SkillCraft.Api.Core.Languages.Commands;

internal record DeleteLanguageCommand(Guid Id) : ICommand<LanguageModel?>;

internal class DeleteLanguageCommandHandler : ICommandHandler<DeleteLanguageCommand, LanguageModel?>
{
  private readonly IContext _context;
  private readonly IPermissionService _permissionService;
  private readonly ILanguageQuerier _languageQuerier;
  private readonly ILanguageRepository _languageRepository;
  private readonly IStorageService _storageService;

  public DeleteLanguageCommandHandler(
    IContext context,
    IPermissionService permissionService,
    ILanguageQuerier languageQuerier,
    ILanguageRepository languageRepository,
    IStorageService storageService)
  {
    _context = context;
    _permissionService = permissionService;
    _languageQuerier = languageQuerier;
    _languageRepository = languageRepository;
    _storageService = storageService;
  }

  public async Task<LanguageModel?> HandleAsync(DeleteLanguageCommand command, CancellationToken cancellationToken)
  {
    LanguageId languageId = new(command.Id, _context.WorldId);
    Language? language = await _languageRepository.LoadAsync(languageId, cancellationToken);
    if (language is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Delete, language, cancellationToken);
    LanguageModel model = await _languageQuerier.ReadAsync(language, cancellationToken);

    language.Delete(_context.UserId);

    await _storageService.ExecuteWithQuotaAsync(
      language,
      async () => await _languageRepository.SaveAsync(language, cancellationToken),
      cancellationToken);

    return model;
  }
}
