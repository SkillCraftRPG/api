using Bogus;
using Moq;
using SkillCraft.Api.Contracts;
using SkillCraft.Api.Contracts.Specializations;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Storages;
using SkillCraft.Api.Core.Talents;

namespace SkillCraft.Api.Core.Specializations.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class CreateOrReplaceSpecializationCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly UnitTestContext _context = UnitTestContext.Generate();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<ISpecializationQuerier> _specializationQuerier = new();
  private readonly Mock<ISpecializationRepository> _specializationRepository = new();
  private readonly Mock<ITalentRepository> _talentRepository = new();
  private readonly Mock<IStorageService> _storageService = new();

  private readonly CreateOrReplaceSpecializationCommandHandler _handler;

  public CreateOrReplaceSpecializationCommandHandlerTests()
  {
    _handler = new CreateOrReplaceSpecializationCommandHandler(
      _context,
      _permissionService.Object,
      _specializationQuerier.Object,
      _specializationRepository.Object,
      _storageService.Object,
      _talentRepository.Object);
  }

  [Fact(DisplayName = "It should throw EntityNotFoundException when the required talent does not exist.")]
  public async Task Given_NonExistentRequiredTalentId_When_HandleAsync_Then_EntityNotFoundException()
  {
    Talent survie = new(_context.World, new Tier(0), new Name("Survie"));
    _talentRepository.Setup(x => x.LoadAsync(It.IsAny<IEnumerable<TalentId>>(), _cancellationToken)).ReturnsAsync([]);

    _permissionService.Setup(x => x.CheckAsync(Actions.CreateSpecialization, _cancellationToken)).Returns(Task.CompletedTask);

    CreateOrReplaceSpecializationPayload payload = new()
    {
      Tier = 1,
      Name = "Éclaireur",
      Doctrine = new DoctrinePayload
      {
        Name = "Avant-garde"
      },
      Requirements = new RequirementsPayload
      {
        TalentId = survie.EntityId
      }
    };

    CreateOrReplaceSpecializationCommand command = new(payload, Id: null);
    var exception = await Assert.ThrowsAsync<EntityNotFoundException>(async () => await _handler.HandleAsync(command, _cancellationToken));

    Assert.Equal(Talent.EntityKind, exception.EntityKind);
    Assert.Equal(survie.EntityId, exception.EntityId);
    Assert.Equal("Requirements.TalentId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw SpecializationTierCannotBeChangedException when replacing and the payload tier differs from the specialization tier.")]
  public async Task Given_DifferentTier_When_Replace_Then_SpecializationTierCannotBeChangedException()
  {
    Specialization specialization = new(_context.World, new Tier(1), new Name("Éclaireur"));
    _specializationRepository.Setup(x => x.LoadAsync(specialization.Id, _cancellationToken)).ReturnsAsync(specialization);

    _permissionService.Setup(x => x.CheckAsync(Actions.Update, specialization, _cancellationToken)).Returns(Task.CompletedTask);

    CreateOrReplaceSpecializationPayload payload = new()
    {
      Tier = specialization.Tier.Value + 1,
      Name = specialization.Name.Value,
      Doctrine = new DoctrinePayload
      {
        Name = "Avant-garde"
      }
    };

    CreateOrReplaceSpecializationCommand command = new(payload, specialization.EntityId);
    var exception = await Assert.ThrowsAsync<SpecializationTierCannotBeChangedException>(async () => await _handler.HandleAsync(command, _cancellationToken));

    Assert.Equal(_context.WorldId.ToGuid(), exception.WorldId);
    Assert.Equal(specialization.EntityId, exception.SpecializationId);
    Assert.Equal(specialization.Tier.Value, exception.SpecializationTier);
    Assert.Equal(payload.Tier, exception.AttemptedTier);
    Assert.Equal("Tier", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw TalentsNotFoundException when some doctrine discounted talent IDs are not found.")]
  public async Task Given_MissingDoctrineDiscountedTalentIds_When_HandleAsync_Then_TalentsNotFoundException()
  {
    Talent armesDeTir = new(_context.World, new Tier(1), new Name("Armes de tir"));
    Talent tirRapide = new(_context.World, new Tier(1), new Name("Tir rapide"));
    _talentRepository.Setup(x => x.LoadAsync(It.IsAny<IEnumerable<TalentId>>(), _cancellationToken)).ReturnsAsync([armesDeTir, tirRapide]);

    _permissionService.Setup(x => x.CheckAsync(Actions.CreateSpecialization, _cancellationToken)).Returns(Task.CompletedTask);

    Guid missingId1 = Guid.NewGuid();
    Guid missingId2 = Guid.NewGuid();
    CreateOrReplaceSpecializationPayload payload = new()
    {
      Tier = 1,
      Name = "Éclaireur",
      Doctrine = new DoctrinePayload
      {
        Name = "Avant-garde",
        DiscountedTalentIds = [armesDeTir.EntityId, tirRapide.EntityId, missingId1, missingId2]
      }
    };

    CreateOrReplaceSpecializationCommand command = new(payload, Id: null);
    var exception = await Assert.ThrowsAsync<TalentsNotFoundException>(async () => await _handler.HandleAsync(command, _cancellationToken));

    Assert.Equal(_context.WorldId.ToGuid(), exception.WorldId);
    Assert.Equal(2, exception.TalentIds.Count);
    Assert.Contains(missingId1, exception.TalentIds);
    Assert.Contains(missingId2, exception.TalentIds);
    Assert.Equal("Doctrine.DiscountedTalentIds", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw TalentsNotFoundException when some optional talent IDs are not found.")]
  public async Task Given_MissingOptionTalentIds_When_HandleAsync_Then_TalentsNotFoundException()
  {
    Talent survie = new(_context.World, new Tier(0), new Name("Survie"));
    Talent survivalisme = new(_context.World, new Tier(1), new Name("Survivalisme"));
    _talentRepository.Setup(x => x.LoadAsync(It.IsAny<IEnumerable<TalentId>>(), _cancellationToken)).ReturnsAsync([survie, survivalisme]);

    _permissionService.Setup(x => x.CheckAsync(Actions.CreateSpecialization, _cancellationToken)).Returns(Task.CompletedTask);

    Guid missingId1 = Guid.NewGuid();
    Guid missingId2 = Guid.NewGuid();
    CreateOrReplaceSpecializationPayload payload = new()
    {
      Tier = 2,
      Name = "Chasseur",
      Doctrine = new DoctrinePayload
      {
        Name = "Voie du chasseur"
      },
      Options = new OptionsPayload
      {
        TalentIds = [survie.EntityId, survivalisme.EntityId, missingId1, missingId2]
      }
    };

    CreateOrReplaceSpecializationCommand command = new(payload, Id: null);
    var exception = await Assert.ThrowsAsync<TalentsNotFoundException>(async () => await _handler.HandleAsync(command, _cancellationToken));

    Assert.Equal(_context.WorldId.ToGuid(), exception.WorldId);
    Assert.Equal(2, exception.TalentIds.Count);
    Assert.Contains(missingId1, exception.TalentIds);
    Assert.Contains(missingId2, exception.TalentIds);
    Assert.Equal("Options.TalentIds", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task Given_InvalidPayload_When_HandleAsync_Then_ValidationException()
  {
    CreateOrReplaceSpecializationPayload payload = new()
    {
      Tier = 0,
      Name = string.Empty,
      Summary = _faker.Random.String(length: 999),
      Doctrine = new DoctrinePayload
      {
        Features = [new FeatureModel()]
      }
    };

    CreateOrReplaceSpecializationCommand command = new(payload, Id: null);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.HandleAsync(command, _cancellationToken));

    Assert.Equal(5, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Name");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "InclusiveBetweenValidator" && e.PropertyName == "Tier");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "Summary");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Doctrine.Name");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Doctrine.Features[0].Name");
  }
}
