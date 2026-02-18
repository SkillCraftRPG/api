using Moq;
using SkillCraft.Api.Contracts.Castes;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Storages;

namespace SkillCraft.Api.Core.Castes.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class DeleteCasteCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly UnitTestContext _context = UnitTestContext.Generate();
  private readonly Mock<ICasteQuerier> _casteQuerier = new();
  private readonly Mock<ICasteRepository> _casteRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<IStorageService> _storageService = new();

  private readonly DeleteCasteCommandHandler _handler;

  public DeleteCasteCommandHandlerTests()
  {
    _handler = new DeleteCasteCommandHandler(
      _context,
      _casteQuerier.Object,
      _casteRepository.Object,
      _permissionService.Object,
      _storageService.Object);
  }

  [Fact(DisplayName = "It should return null when the caste is not found.")]
  public async Task Given_NotFound_When_HandleAsync_Then_NullReturned()
  {
    DeleteCasteCommand command = new(Guid.Empty);
    CasteModel? result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.Null(result);
  }
}
