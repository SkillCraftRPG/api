using Bogus;
using Moq;
using SkillCraft.Api.Contracts.Scripts;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Storages;

namespace SkillCraft.Api.Core.Scripts.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class CreateOrReplaceScriptCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly UnitTestContext _context = UnitTestContext.Generate();
  private readonly Mock<IScriptQuerier> _scriptQuerier = new();
  private readonly Mock<IScriptRepository> _scriptRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<IStorageService> _storageService = new();

  private readonly CreateOrReplaceScriptCommandHandler _handler;

  public CreateOrReplaceScriptCommandHandlerTests()
  {
    _handler = new CreateOrReplaceScriptCommandHandler(
      _context,
      _permissionService.Object,
      _scriptQuerier.Object,
      _scriptRepository.Object,
      _storageService.Object);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task Given_InvalidPayload_When_HandleAsync_Then_ValidationException()
  {
    CreateOrReplaceScriptPayload payload = new()
    {
      Name = string.Empty,
      Summary = _faker.Random.String(length: 999)
    };

    CreateOrReplaceScriptCommand command = new(payload, Id: null);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.HandleAsync(command, _cancellationToken));

    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Name");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "Summary");
  }
}
