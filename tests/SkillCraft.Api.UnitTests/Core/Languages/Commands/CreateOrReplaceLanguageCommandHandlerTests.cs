using Bogus;
using Moq;
using SkillCraft.Api.Contracts.Languages;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Scripts;
using SkillCraft.Api.Core.Storages;

namespace SkillCraft.Api.Core.Languages.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class CreateOrReplaceLanguageCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly UnitTestContext _context = UnitTestContext.Generate();
  private readonly Mock<ILanguageQuerier> _languageQuerier = new();
  private readonly Mock<ILanguageRepository> _languageRepository = new();
  private readonly Mock<IScriptRepository> _scriptRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<IStorageService> _storageService = new();

  private readonly CreateOrReplaceLanguageCommandHandler _handler;

  public CreateOrReplaceLanguageCommandHandlerTests()
  {
    _handler = new CreateOrReplaceLanguageCommandHandler(
      _context,
      _permissionService.Object,
      _languageQuerier.Object,
      _languageRepository.Object,
      _scriptRepository.Object,
      _storageService.Object);
  }

  [Fact(DisplayName = "It should throw EntityNotFoundException when the script does not exist.")]
  public async Task Given_NonExistentScriptId_When_HandleAsync_Then_EntityNotFoundException()
  {
    Guid scriptId = Guid.Empty;
    CreateOrReplaceLanguagePayload payload = new()
    {
      Name = "Celfique",
      ScriptId = scriptId
    };

    _permissionService.Setup(x => x.CheckAsync(Actions.CreateLanguage, _cancellationToken)).Returns(Task.CompletedTask);

    CreateOrReplaceLanguageCommand command = new(payload, Id: null);
    var exception = await Assert.ThrowsAsync<EntityNotFoundException>(async () => await _handler.HandleAsync(command, _cancellationToken));

    Assert.Equal(Script.EntityKind, exception.EntityKind);
    Assert.Equal(scriptId, exception.EntityId);
    Assert.Equal("ScriptId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task Given_InvalidPayload_When_HandleAsync_Then_ValidationException()
  {
    CreateOrReplaceLanguagePayload payload = new()
    {
      Name = string.Empty,
      Summary = _faker.Random.String(length: 999)
    };

    CreateOrReplaceLanguageCommand command = new(payload, Id: null);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.HandleAsync(command, _cancellationToken));

    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Name");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "Summary");
  }
}
