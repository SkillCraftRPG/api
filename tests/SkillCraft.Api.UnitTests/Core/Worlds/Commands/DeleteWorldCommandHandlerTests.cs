using Moq;
using SkillCraft.Api.Contracts.Worlds;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Storages;

namespace SkillCraft.Api.Core.Worlds.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class DeleteWorldCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly UnitTestContext _context = UnitTestContext.Generate();
  private readonly Mock<IWorldQuerier> _worldQuerier = new();
  private readonly Mock<IWorldRepository> _worldRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<IStorageService> _storageService = new();

  private readonly DeleteWorldCommandHandler _handler;

  public DeleteWorldCommandHandlerTests()
  {
    _handler = new DeleteWorldCommandHandler(
      _context,
      _worldQuerier.Object,
      _worldRepository.Object,
      _permissionService.Object,
      _storageService.Object);
  }

  [Fact(DisplayName = "It should return null when the world is not found.")]
  public async Task Given_NotFound_When_HandleAsync_Then_NullReturned()
  {
    DeleteWorldCommand command = new(Guid.Empty);
    WorldModel? result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.Null(result);
  }
}
