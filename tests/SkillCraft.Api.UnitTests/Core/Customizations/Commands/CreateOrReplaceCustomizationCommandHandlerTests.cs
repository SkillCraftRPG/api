using Bogus;
using Moq;
using SkillCraft.Api.Contracts.Customizations;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Storages;

namespace SkillCraft.Api.Core.Customizations.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class CreateOrReplaceCustomizationCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly UnitTestContext _context = UnitTestContext.Generate();
  private readonly Mock<ICustomizationQuerier> _customizationQuerier = new();
  private readonly Mock<ICustomizationRepository> _customizationRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<IStorageService> _storageService = new();

  private readonly CreateOrReplaceCustomizationCommandHandler _handler;

  public CreateOrReplaceCustomizationCommandHandlerTests()
  {
    _handler = new CreateOrReplaceCustomizationCommandHandler(
      _context,
      _customizationQuerier.Object,
      _customizationRepository.Object,
      _permissionService.Object,
      _storageService.Object);
  }

  [Fact(DisplayName = "It should throw CannotChangeCustomizationKindException when changing the customization kind.")]
  public async Task Given_DifferentKind_When_HandleAsync_Then_CannotChangeCustomizationKindException()
  {
    Customization customization = new(_context.World, CustomizationKind.Gift, new Name("Gift"));
    _customizationRepository.Setup(x => x.LoadAsync(customization.Id, _cancellationToken)).ReturnsAsync(customization);

    CreateOrReplaceCustomizationPayload payload = new()
    {
      Kind = CustomizationKind.Disability,
      Name = "  Abruti  ",
      Summary = "  Limité, maladroit et désavantagé dans l’usage de son intellect.  ",
      Description = "  Simple d’esprit et bon vivant ou complètement taré, le personnage est dénué de sens commun et n’est pas doté d’une bonne mémoire.\n\nSes [tests](/regles/competences/tests) d’[Intellect](/regles/attributs/intellect) et des [compétences](/regles/competences) associées ([Connaissance](/regles/competences/connaissance), [Investigation](/regles/competences/investigation), [Linguistique](/regles/competences/linguistique) et [Médecine](/regles/competences/medecine)) sont affligés du [désavantage](/regles/competences/tests/avantage-desavantage).  "
    };

    CreateOrReplaceCustomizationCommand command = new(payload, customization.EntityId);
    var exception = await Assert.ThrowsAsync<CannotChangeCustomizationKindException>(async () => await _handler.HandleAsync(command, _cancellationToken));

    Assert.Equal(customization.WorldId.ToGuid(), exception.WorldId);
    Assert.Equal(customization.EntityId, exception.CustomizationId);
    Assert.Equal(customization.Kind, exception.CustomizationKind);
    Assert.Equal(payload.Kind, exception.AttemptedKind);
    Assert.Equal("Kind", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task Given_InvalidPayload_When_HandleAsync_Then_ValidationException()
  {
    CreateOrReplaceCustomizationPayload payload = new()
    {
      Kind = (CustomizationKind)(-1),
      Summary = _faker.Random.String(length: 999)
    };

    CreateOrReplaceCustomizationCommand command = new(payload, Id: null);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.HandleAsync(command, _cancellationToken));

    Assert.Equal(3, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "EnumValidator" && e.PropertyName == "Kind");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Name");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "Summary");
  }
}
