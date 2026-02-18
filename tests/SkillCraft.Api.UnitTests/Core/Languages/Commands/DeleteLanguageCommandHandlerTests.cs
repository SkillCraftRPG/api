using Moq;
using SkillCraft.Api.Contracts.Languages;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Storages;

namespace SkillCraft.Api.Core.Languages.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class DeleteLanguageCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly UnitTestContext _context = UnitTestContext.Generate();
  private readonly Mock<ILanguageQuerier> _languageQuerier = new();
  private readonly Mock<ILanguageRepository> _languageRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<IStorageService> _storageService = new();

  private readonly DeleteLanguageCommandHandler _handler;

  public DeleteLanguageCommandHandlerTests()
  {
    _handler = new DeleteLanguageCommandHandler(
      _context,
      _permissionService.Object,
      _languageQuerier.Object,
      _languageRepository.Object,
      _storageService.Object);
  }

  [Fact(DisplayName = "It should return null when the language is not found.")]
  public async Task Given_NotFound_When_HandleAsync_Then_NullReturned()
  {
    DeleteLanguageCommand command = new(Guid.Empty);
    LanguageModel? result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.Null(result);
  }
}
