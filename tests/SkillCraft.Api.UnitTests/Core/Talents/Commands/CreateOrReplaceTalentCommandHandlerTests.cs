using Bogus;
using Moq;
using SkillCraft.Api.Contracts;
using SkillCraft.Api.Contracts.Talents;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Storages;

namespace SkillCraft.Api.Core.Talents.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class CreateOrReplaceTalentCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly UnitTestContext _context = UnitTestContext.Generate();
  private readonly Mock<ITalentQuerier> _talentQuerier = new();
  private readonly Mock<ITalentRepository> _talentRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<IStorageService> _storageService = new();

  private readonly CreateOrReplaceTalentCommandHandler _handler;

  public CreateOrReplaceTalentCommandHandlerTests()
  {
    _handler = new CreateOrReplaceTalentCommandHandler(
      _context,
      _permissionService.Object,
      _talentQuerier.Object,
      _talentRepository.Object,
      _storageService.Object);
  }

  [Fact(DisplayName = "It should throw EntityNotFoundException when the required talent does not exist.")]
  public async Task Given_NonExistentRequiredTalentId_When_HandleAsync_Then_EntityNotFoundException()
  {
    Guid requiredTalentId = Guid.Empty;
    CreateOrReplaceTalentPayload payload = new()
    {
      Name = "Melee",
      RequiredTalentId = requiredTalentId
    };

    _permissionService.Setup(x => x.CheckAsync(Actions.CreateTalent, _cancellationToken)).Returns(Task.CompletedTask);

    CreateOrReplaceTalentCommand command = new(payload, Id: null);
    var exception = await Assert.ThrowsAsync<EntityNotFoundException>(async () => await _handler.HandleAsync(command, _cancellationToken));

    Assert.Equal(Talent.EntityKind, exception.EntityKind);
    Assert.Equal(requiredTalentId, exception.EntityId);
    Assert.Equal("RequiredTalentId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task Given_InvalidPayload_When_HandleAsync_Then_ValidationException()
  {
    CreateOrReplaceTalentPayload payload = new()
    {
      Name = string.Empty,
      Summary = _faker.Random.String(length: 999),
      Skill = (GameSkill)(-1)
    };

    CreateOrReplaceTalentCommand command = new(payload, Id: null);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.HandleAsync(command, _cancellationToken));

    Assert.Equal(3, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Name");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "Summary");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "EnumValidator" && e.PropertyName == "Skill");
  }
}
