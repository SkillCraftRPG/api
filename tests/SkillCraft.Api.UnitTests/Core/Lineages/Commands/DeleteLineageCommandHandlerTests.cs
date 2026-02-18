using Moq;
using SkillCraft.Api.Contracts.Lineages;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Storages;

namespace SkillCraft.Api.Core.Lineages.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class DeleteLineageCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly UnitTestContext _context = UnitTestContext.Generate();
  private readonly Mock<ILineageQuerier> _lineageQuerier = new();
  private readonly Mock<ILineageRepository> _lineageRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<IStorageService> _storageService = new();

  private readonly DeleteLineageCommandHandler _handler;

  public DeleteLineageCommandHandlerTests()
  {
    _handler = new DeleteLineageCommandHandler(
      _context,
      _lineageQuerier.Object,
      _lineageRepository.Object,
      _permissionService.Object,
      _storageService.Object);
  }

  [Fact(DisplayName = "It should return null when the lineage is not found.")]
  public async Task Given_NotFound_When_HandleAsync_Then_NullReturned()
  {
    DeleteLineageCommand command = new(Guid.Empty);
    LineageModel? result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.Null(result);
  }
}
