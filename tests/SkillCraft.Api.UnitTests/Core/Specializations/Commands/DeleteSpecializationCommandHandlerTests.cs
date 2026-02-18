using Moq;
using SkillCraft.Api.Contracts.Specializations;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Storages;

namespace SkillCraft.Api.Core.Specializations.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class DeleteSpecializationCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly UnitTestContext _context = UnitTestContext.Generate();
  private readonly Mock<ISpecializationQuerier> _specializationQuerier = new();
  private readonly Mock<ISpecializationRepository> _specializationRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<IStorageService> _storageService = new();

  private readonly DeleteSpecializationCommandHandler _handler;

  public DeleteSpecializationCommandHandlerTests()
  {
    _handler = new DeleteSpecializationCommandHandler(
      _context,
      _permissionService.Object,
      _specializationQuerier.Object,
      _specializationRepository.Object,
      _storageService.Object);
  }

  [Fact(DisplayName = "It should return null when the specialization is not found.")]
  public async Task Given_NotFound_When_HandleAsync_Then_NullReturned()
  {
    DeleteSpecializationCommand command = new(Guid.Empty);
    SpecializationModel? result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.Null(result);
  }
}
