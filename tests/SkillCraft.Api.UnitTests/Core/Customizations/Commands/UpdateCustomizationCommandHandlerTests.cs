using Bogus;
using Moq;
using SkillCraft.Api.Contracts;
using SkillCraft.Api.Contracts.Customizations;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Storages;

namespace SkillCraft.Api.Core.Customizations.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class UpdateCustomizationCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly UnitTestContext _context = UnitTestContext.Generate();
  private readonly Mock<ICustomizationQuerier> _customizationQuerier = new();
  private readonly Mock<ICustomizationRepository> _customizationRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<IStorageService> _storageService = new();

  private readonly UpdateCustomizationCommandHandler _handler;

  public UpdateCustomizationCommandHandlerTests()
  {
    _handler = new UpdateCustomizationCommandHandler(
      _context,
      _customizationQuerier.Object,
      _customizationRepository.Object,
      _permissionService.Object,
      _storageService.Object);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task Given_InvalidPayload_When_HandleAsync_Then_ValidationException()
  {
    UpdateCustomizationPayload payload = new()
    {
      Name = _faker.Random.String(length: 999),
      Summary = new Update<string>(_faker.Random.String(length: 999))
    };

    UpdateCustomizationCommand command = new(Guid.Empty, payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.HandleAsync(command, _cancellationToken));

    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "Name");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "Summary.Value");
  }
}
