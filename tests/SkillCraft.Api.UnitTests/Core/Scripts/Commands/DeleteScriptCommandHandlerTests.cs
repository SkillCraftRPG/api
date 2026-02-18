using Moq;
using SkillCraft.Api.Contracts.Scripts;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Storages;

namespace SkillCraft.Api.Core.Scripts.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class DeleteScriptCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly UnitTestContext _context = UnitTestContext.Generate();
  private readonly Mock<IScriptQuerier> _scriptQuerier = new();
  private readonly Mock<IScriptRepository> _scriptRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<IStorageService> _storageService = new();

  private readonly DeleteScriptCommandHandler _handler;

  public DeleteScriptCommandHandlerTests()
  {
    _handler = new DeleteScriptCommandHandler(
      _context,
      _permissionService.Object,
      _scriptQuerier.Object,
      _scriptRepository.Object,
      _storageService.Object);
  }

  [Fact(DisplayName = "It should return null when the script is not found.")]
  public async Task Given_NotFound_When_HandleAsync_Then_NullReturned()
  {
    DeleteScriptCommand command = new(Guid.Empty);
    ScriptModel? result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.Null(result);
  }
}
