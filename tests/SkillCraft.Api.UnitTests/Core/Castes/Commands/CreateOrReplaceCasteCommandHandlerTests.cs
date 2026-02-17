using Bogus;
using Moq;
using SkillCraft.Api.Contracts;
using SkillCraft.Api.Contracts.Castes;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Storages;

namespace SkillCraft.Api.Core.Castes.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class CreateOrReplaceCasteCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly UnitTestContext _context = UnitTestContext.Generate();
  private readonly Mock<ICasteQuerier> _casteQuerier = new();
  private readonly Mock<ICasteRepository> _casteRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<IStorageService> _storageService = new();

  private readonly CreateOrReplaceCasteCommandHandler _handler;

  public CreateOrReplaceCasteCommandHandlerTests()
  {
    _handler = new CreateOrReplaceCasteCommandHandler(
      _context,
      _casteQuerier.Object,
      _casteRepository.Object,
      _permissionService.Object,
      _storageService.Object);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task Given_InvalidPayload_When_HandleAsync_Then_ValidationException()
  {
    CreateOrReplaceCastePayload payload = new()
    {
      Name = string.Empty,
      Summary = _faker.Random.String(length: 999),
      Skill = (GameSkill)(-1),
      WealthRoll = "test",
      Feature = new FeatureModel()
    };

    CreateOrReplaceCasteCommand command = new(payload, Id: null);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.HandleAsync(command, _cancellationToken));

    Assert.Equal(5, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Name");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "Summary");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "EnumValidator" && e.PropertyName == "Skill");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "RegularExpressionValidator" && e.PropertyName == "WealthRoll");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Feature.Name");
  }
}
