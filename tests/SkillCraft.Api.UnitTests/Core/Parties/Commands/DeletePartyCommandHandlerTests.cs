using Moq;
using SkillCraft.Api.Contracts.Parties;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Storages;

namespace SkillCraft.Api.Core.Parties.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class DeletePartyCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly UnitTestContext _context = UnitTestContext.Generate();
  private readonly Mock<IPartyQuerier> _partyQuerier = new();
  private readonly Mock<IPartyRepository> _partyRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<IStorageService> _storageService = new();

  private readonly DeletePartyCommandHandler _handler;

  public DeletePartyCommandHandlerTests()
  {
    _handler = new DeletePartyCommandHandler(
      _context,
      _partyQuerier.Object,
      _partyRepository.Object,
      _permissionService.Object,
      _storageService.Object);
  }

  [Fact(DisplayName = "It should return null when the party is not found.")]
  public async Task Given_NotFound_When_HandleAsync_Then_NullReturned()
  {
    DeletePartyCommand command = new(Guid.Empty);
    PartyModel? result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.Null(result);
  }
}
