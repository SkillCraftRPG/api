using Bogus;
using Moq;
using SkillCraft.Api.Contracts;
using SkillCraft.Api.Contracts.Lineages;
using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Storages;

namespace SkillCraft.Api.Core.Lineages.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class UpdateLineageCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly UnitTestContext _context = UnitTestContext.Generate();
  private readonly Mock<ILineageQuerier> _lineageQuerier = new();
  private readonly Mock<ILineageRepository> _lineageRepository = new();
  private readonly Mock<ILanguageRepository> _languageRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<IStorageService> _storageService = new();

  private readonly UpdateLineageCommandHandler _handler;

  public UpdateLineageCommandHandlerTests()
  {
    _handler = new UpdateLineageCommandHandler(
      _context,
      _languageRepository.Object,
      _lineageQuerier.Object,
      _lineageRepository.Object,
      _permissionService.Object,
      _storageService.Object);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task Given_InvalidPayload_When_HandleAsync_Then_ValidationException()
  {
    UpdateLineagePayload payload = new()
    {
      Name = _faker.Random.String(length: 999),
      Summary = new Update<string>(_faker.Random.String(length: 999)),
      Features = [new FeatureModel()],
      Languages = new LanguagesPayload(extra: -1),
      Names = new NamesModel(),
      Speeds = new SpeedsModel(walk: -6, hover: true, burrow: 0),
      Size = new SizeModel((SizeCategory)(-999), "invalid"),
      Weight = new WeightModel(normal: "invalid"),
      Age = new AgeModel(30, 100, 750, 275)
    };
    payload.Names.Custom.Add(new NameCategory(_faker.Random.String(length: 999)));

    UpdateLineageCommand command = new(Guid.Empty, payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.HandleAsync(command, _cancellationToken));

    Assert.Equal(12, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "Name");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "Summary.Value");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Features[0].Name");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "GreaterThanOrEqualValidator" && e.PropertyName == "Languages.Extra");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "Names.Custom[0].Category");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "GreaterThanValidator" && e.PropertyName == "Speeds.Walk.Value");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotNullValidator" && e.PropertyName == "Speeds.Fly");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "GreaterThanValidator" && e.PropertyName == "Speeds.Burrow.Value");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "EnumValidator" && e.PropertyName == "Size.Category");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "RegularExpressionValidator" && e.PropertyName == "Size.Height");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "RegularExpressionValidator" && e.PropertyName == "Weight.Normal");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "AgeValidator" && e.PropertyName == "Age");
  }
}
