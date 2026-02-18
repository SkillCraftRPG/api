using Moq;
using SkillCraft.Api.Contracts.Customizations;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Storages;

namespace SkillCraft.Api.Core.Customizations.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class DeleteCustomizationCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly UnitTestContext _context = UnitTestContext.Generate();
  private readonly Mock<ICustomizationQuerier> _customizationQuerier = new();
  private readonly Mock<ICustomizationRepository> _customizationRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<IStorageService> _storageService = new();

  private readonly DeleteCustomizationCommandHandler _handler;

  public DeleteCustomizationCommandHandlerTests()
  {
    _handler = new DeleteCustomizationCommandHandler(
      _context,
      _customizationQuerier.Object,
      _customizationRepository.Object,
      _permissionService.Object,
      _storageService.Object);
  }

  [Fact(DisplayName = "It should return null when the customization is not found.")]
  public async Task Given_NotFound_When_HandleAsync_Then_NullReturned()
  {
    DeleteCustomizationCommand command = new(Guid.Empty);
    CustomizationModel? result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.Null(result);
  }
}
