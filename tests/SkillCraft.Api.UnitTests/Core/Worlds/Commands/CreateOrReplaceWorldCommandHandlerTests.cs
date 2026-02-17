using Moq;
using SkillCraft.Api.Contracts.Worlds;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Storages;

namespace SkillCraft.Api.Core.Worlds.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class CreateOrReplaceWorldCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly UnitTestContext _context = UnitTestContext.Generate();
  private readonly Mock<IWorldQuerier> _worldQuerier = new();
  private readonly Mock<IWorldRepository> _worldRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<IStorageService> _storageService = new();

  private readonly CreateOrReplaceWorldCommandHandler _handler;

  public CreateOrReplaceWorldCommandHandlerTests()
  {
    _handler = new CreateOrReplaceWorldCommandHandler(
      _context,
      _worldQuerier.Object,
      _worldRepository.Object,
      _permissionService.Object,
      _storageService.Object);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task Given_InvalidPayload_When_HandleAsync_Then_ValidationException()
  {
    CreateOrReplaceWorldPayload payload = new()
    {
      Name = string.Empty
    };

    CreateOrReplaceWorldCommand command = new(payload, Id: null);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.HandleAsync(command, _cancellationToken));

    Assert.Single(exception.Errors);
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Name");
  }
}
