using Moq;
using SkillCraft.Api.Contracts.Talents;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Storages;

namespace SkillCraft.Api.Core.Talents.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class DeleteTalentCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly UnitTestContext _context = UnitTestContext.Generate();
  private readonly Mock<ITalentQuerier> _talentQuerier = new();
  private readonly Mock<ITalentRepository> _talentRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<IStorageService> _storageService = new();

  private readonly DeleteTalentCommandHandler _handler;

  public DeleteTalentCommandHandlerTests()
  {
    _handler = new DeleteTalentCommandHandler(
      _context,
      _permissionService.Object,
      _talentQuerier.Object,
      _talentRepository.Object,
      _storageService.Object);
  }

  [Fact(DisplayName = "It should return null when the talent is not found.")]
  public async Task Given_NotFound_When_HandleAsync_Then_NullReturned()
  {
    DeleteTalentCommand command = new(Guid.Empty);
    TalentModel? result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.Null(result);
  }
}
