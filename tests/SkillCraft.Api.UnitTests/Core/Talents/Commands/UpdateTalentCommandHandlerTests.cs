using Bogus;
using Moq;
using SkillCraft.Api.Contracts;
using SkillCraft.Api.Contracts.Talents;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Storages;

namespace SkillCraft.Api.Core.Talents.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class UpdateTalentCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly UnitTestContext _context = UnitTestContext.Generate();
  private readonly Mock<ITalentQuerier> _talentQuerier = new();
  private readonly Mock<ITalentRepository> _talentRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<IStorageService> _storageService = new();

  private readonly UpdateTalentCommandHandler _handler;

  public UpdateTalentCommandHandlerTests()
  {
    _handler = new UpdateTalentCommandHandler(
      _context,
      _permissionService.Object,
      _talentQuerier.Object,
      _talentRepository.Object,
      _storageService.Object);
  }

  [Fact(DisplayName = "It should throw EntityNotFoundException when the required talent does not exist.")]
  public async Task Given_NonExistentRequiredTalentId_When_HandleAsync_Then_EntityNotFoundException()
  {
    Talent talent = new(_context.World, new Tier(0), new Name("Melee"), _context.UserId);
    _talentRepository.Setup(x => x.LoadAsync(talent.Id, _cancellationToken)).ReturnsAsync(talent);

    _permissionService.Setup(x => x.CheckAsync(Actions.Update, talent, _cancellationToken)).Returns(Task.CompletedTask);

    Guid requiredTalentId = Guid.Empty;
    UpdateTalentPayload payload = new()
    {
      RequiredTalentId = new Update<Guid?>(requiredTalentId)
    };

    UpdateTalentCommand command = new(talent.EntityId, payload);
    var exception = await Assert.ThrowsAsync<EntityNotFoundException>(async () => await _handler.HandleAsync(command, _cancellationToken));

    Assert.Equal(Talent.EntityKind, exception.EntityKind);
    Assert.Equal(requiredTalentId, exception.EntityId);
    Assert.Equal("RequiredTalentId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task Given_InvalidPayload_When_HandleAsync_Then_ValidationException()
  {
    UpdateTalentPayload payload = new()
    {
      Name = _faker.Random.String(length: 999),
      Summary = new Update<string>(_faker.Random.String(length: 999)),
      Skill = new Update<GameSkill?>((GameSkill)(-1))
    };

    UpdateTalentCommand command = new(Guid.Empty, payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.HandleAsync(command, _cancellationToken));

    Assert.Equal(3, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "Name");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "Summary.Value");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "EnumValidator" && e.PropertyName == "Skill.Value");
  }
}
