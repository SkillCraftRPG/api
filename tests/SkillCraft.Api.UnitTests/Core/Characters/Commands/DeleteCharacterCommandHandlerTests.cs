using Moq;
using SkillCraft.Api.Contracts.Characters;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Storages;

namespace SkillCraft.Api.Core.Characters.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class DeleteCharacterCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly UnitTestContext _context = UnitTestContext.Generate();
  private readonly Mock<ICharacterQuerier> _characterQuerier = new();
  private readonly Mock<ICharacterRepository> _characterRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<IStorageService> _storageService = new();

  private readonly DeleteCharacterCommandHandler _handler;

  public DeleteCharacterCommandHandlerTests()
  {
    _handler = new DeleteCharacterCommandHandler(
      _context,
      _characterQuerier.Object,
      _characterRepository.Object,
      _permissionService.Object,
      _storageService.Object);
  }

  [Fact(DisplayName = "It should return null when the character is not found.")]
  public async Task Given_NotFound_When_HandleAsync_Then_NullReturned()
  {
    DeleteCharacterCommand command = new(Guid.Empty);
    CharacterModel? result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.Null(result);
  }
}
