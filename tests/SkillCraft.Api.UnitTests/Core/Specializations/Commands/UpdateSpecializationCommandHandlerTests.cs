using Bogus;
using Moq;
using SkillCraft.Api.Contracts;
using SkillCraft.Api.Contracts.Specializations;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Storages;
using SkillCraft.Api.Core.Talents;

namespace SkillCraft.Api.Core.Specializations.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class UpdateSpecializationCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly UnitTestContext _context = UnitTestContext.Generate();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<ISpecializationQuerier> _specializationQuerier = new();
  private readonly Mock<ISpecializationRepository> _specializationRepository = new();
  private readonly Mock<ITalentRepository> _talentRepository = new();
  private readonly Mock<IStorageService> _storageService = new();

  private readonly UpdateSpecializationCommandHandler _handler;

  public UpdateSpecializationCommandHandlerTests()
  {
    _handler = new UpdateSpecializationCommandHandler(
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
    Specialization specialization = new(_context.World, new Tier(2), new Name("Chasseur"));
    _specializationRepository.Setup(x => x.LoadAsync(specialization.Id, _cancellationToken)).ReturnsAsync(specialization);

    Talent survivalisme = new(_context.World, new Tier(1), new Name("Survivalisme"));
    _talentRepository.Setup(x => x.LoadAsync(It.IsAny<IEnumerable<TalentId>>(), _cancellationToken)).ReturnsAsync([]);

    _permissionService.Setup(x => x.CheckAsync(Actions.Update, specialization, _cancellationToken)).Returns(Task.CompletedTask);

    UpdateSpecializationPayload payload = new()
    {
      Requirements = new RequirementsPayload
      {
        TalentId = survivalisme.EntityId
      }
    };

    UpdateSpecializationCommand command = new(specialization.EntityId, payload);
    var exception = await Assert.ThrowsAsync<EntityNotFoundException>(async () => await _handler.HandleAsync(command, _cancellationToken));

    Assert.Equal(Talent.EntityKind, exception.EntityKind);
    Assert.Equal(survivalisme.EntityId, exception.EntityId);
    Assert.Equal("Requirements.TalentId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw TalentsNotFoundException when some doctrine discounted talent IDs are not found.")]
  public async Task Given_MissingDoctrineDiscountedTalentIds_When_HandleAsync_Then_TalentsNotFoundException()
  {
    Specialization specialization = new(_context.World, new Tier(1), new Name("Éclaireur"));
    _specializationRepository.Setup(x => x.LoadAsync(specialization.Id, _cancellationToken)).ReturnsAsync(specialization);

    Talent armesDeTir = new(_context.World, new Tier(0), new Name("Armes de tir"));
    Talent tirRapide = new(_context.World, new Tier(0), new Name("Tir rapide"));
    _talentRepository.Setup(x => x.LoadAsync(It.IsAny<IEnumerable<TalentId>>(), _cancellationToken)).ReturnsAsync([armesDeTir, tirRapide]);

    _permissionService.Setup(x => x.CheckAsync(Actions.Update, specialization, _cancellationToken)).Returns(Task.CompletedTask);

    Guid missingId1 = Guid.NewGuid();
    Guid missingId2 = Guid.NewGuid();
    UpdateSpecializationPayload payload = new()
    {
      Doctrine = new Update<DoctrinePayload>(new DoctrinePayload
      {
        Name = "Avant-garde",
        DiscountedTalentIds = [armesDeTir.EntityId, tirRapide.EntityId, missingId1, missingId2]
      })
    };

    UpdateSpecializationCommand command = new(specialization.EntityId, payload);
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
    Specialization specialization = new(_context.World, new Tier(2), new Name("Chasseur"));
    _specializationRepository.Setup(x => x.LoadAsync(specialization.Id, _cancellationToken)).ReturnsAsync(specialization);

    Talent survie = new(_context.World, new Tier(0), new Name("Survie"));
    Talent survivalisme = new(_context.World, new Tier(1), new Name("Survivalisme"));
    _talentRepository.Setup(x => x.LoadAsync(It.IsAny<IEnumerable<TalentId>>(), _cancellationToken)).ReturnsAsync([survie, survivalisme]);

    _permissionService.Setup(x => x.CheckAsync(Actions.Update, specialization, _cancellationToken)).Returns(Task.CompletedTask);

    Guid missingId1 = Guid.NewGuid();
    Guid missingId2 = Guid.NewGuid();
    UpdateSpecializationPayload payload = new()
    {
      Options = new OptionsPayload
      {
        TalentIds = [survie.EntityId, survivalisme.EntityId, missingId1, missingId2]
      }
    };

    UpdateSpecializationCommand command = new(specialization.EntityId, payload);
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
    UpdateSpecializationPayload payload = new()
    {
      Name = _faker.Random.String(length: 999),
      Summary = new Update<string>(_faker.Random.String(length: 999)),
      Doctrine = new Update<DoctrinePayload>(new DoctrinePayload
      {
        Name = string.Empty,
        Features = [new FeatureModel()]
      })
    };

    UpdateSpecializationCommand command = new(Guid.Empty, payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.HandleAsync(command, _cancellationToken));

    Assert.Equal(4, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "Name");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "Summary.Value");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Doctrine.Value.Name");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Doctrine.Value.Features[0].Name");
  }
}
