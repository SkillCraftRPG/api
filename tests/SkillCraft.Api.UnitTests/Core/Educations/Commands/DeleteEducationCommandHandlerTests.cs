using Moq;
using SkillCraft.Api.Contracts.Educations;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Storages;

namespace SkillCraft.Api.Core.Educations.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class DeleteEducationCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly UnitTestContext _context = UnitTestContext.Generate();
  private readonly Mock<IEducationQuerier> _educationQuerier = new();
  private readonly Mock<IEducationRepository> _educationRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<IStorageService> _storageService = new();

  private readonly DeleteEducationCommandHandler _handler;

  public DeleteEducationCommandHandlerTests()
  {
    _handler = new DeleteEducationCommandHandler(
      _context,
      _educationQuerier.Object,
      _educationRepository.Object,
      _permissionService.Object,
      _storageService.Object);
  }

  [Fact(DisplayName = "It should return null when the education is not found.")]
  public async Task Given_NotFound_When_HandleAsync_Then_NullReturned()
  {
    DeleteEducationCommand command = new(Guid.Empty);
    EducationModel? result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.Null(result);
  }
}
