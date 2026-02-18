using Bogus;
using Moq;
using SkillCraft.Api.Contracts;
using SkillCraft.Api.Contracts.Languages;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Scripts;
using SkillCraft.Api.Core.Storages;

namespace SkillCraft.Api.Core.Languages.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class UpdateLanguageCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly UnitTestContext _context = UnitTestContext.Generate();
  private readonly Mock<ILanguageQuerier> _languageQuerier = new();
  private readonly Mock<ILanguageRepository> _languageRepository = new();
  private readonly Mock<IScriptRepository> _scriptRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<IStorageService> _storageService = new();

  private readonly UpdateLanguageCommandHandler _handler;

  public UpdateLanguageCommandHandlerTests()
  {
    _handler = new UpdateLanguageCommandHandler(
      _context,
      _permissionService.Object,
      _languageQuerier.Object,
      _languageRepository.Object,
      _scriptRepository.Object,
      _storageService.Object);
  }

  [Fact(DisplayName = "It should return null when the language is not found.")]
  public async Task Given_NotFound_When_HandleAsync_Then_NullReturned()
  {
    UpdateLanguageCommand command = new(Guid.Empty, new UpdateLanguagePayload());
    LanguageModel? result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.Null(result);
  }

  [Fact(DisplayName = "It should throw EntityNotFoundException when the script does not exist.")]
  public async Task Given_NonExistentScriptId_When_HandleAsync_Then_EntityNotFoundException()
  {
    Language language = new(_context.World, new Name("Celfique"), _context.UserId);
    _languageRepository.Setup(x => x.LoadAsync(language.Id, _cancellationToken)).ReturnsAsync(language);

    _permissionService.Setup(x => x.CheckAsync(Actions.Update, language, _cancellationToken)).Returns(Task.CompletedTask);

    Guid scriptId = Guid.Empty;
    UpdateLanguagePayload payload = new()
    {
      ScriptId = new Update<Guid?>(scriptId)
    };

    UpdateLanguageCommand command = new(language.EntityId, payload);
    var exception = await Assert.ThrowsAsync<EntityNotFoundException>(async () => await _handler.HandleAsync(command, _cancellationToken));

    Assert.Equal(Script.EntityKind, exception.EntityKind);
    Assert.Equal(scriptId, exception.EntityId);
    Assert.Equal("ScriptId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task Given_InvalidPayload_When_HandleAsync_Then_ValidationException()
  {
    UpdateLanguagePayload payload = new()
    {
      Name = _faker.Random.String(length: 999),
      Summary = new Update<string>(_faker.Random.String(length: 999))
    };

    UpdateLanguageCommand command = new(Guid.Empty, payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.HandleAsync(command, _cancellationToken));

    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "Name");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "Summary.Value");
  }
}
