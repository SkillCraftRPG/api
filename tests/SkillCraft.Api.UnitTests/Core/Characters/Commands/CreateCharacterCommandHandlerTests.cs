using Moq;
using SkillCraft.Api.Contracts.Characters;
using SkillCraft.Api.Core.Castes;
using SkillCraft.Api.Core.Customizations;
using SkillCraft.Api.Core.Educations;
using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Lineages;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Storages;
using SkillCraft.Api.Core.Talents;

namespace SkillCraft.Api.Core.Characters.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class CreateCharacterCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ICasteRepository> _casteRepository = new();
  private readonly Mock<ICharacterQuerier> _characterQuerier = new();
  private readonly Mock<ICharacterRepository> _characterRepository = new();
  private readonly UnitTestContext _context = UnitTestContext.Generate();
  private readonly Mock<ICustomizationRepository> _customizationRepository = new();
  private readonly Mock<IEducationRepository> _educationRepository = new();
  private readonly Mock<ILanguageRepository> _languageRepository = new();
  private readonly Mock<ILineageQuerier> _lineageQuerier = new();
  private readonly Mock<ILineageRepository> _lineageRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<IStorageService> _storageService = new();
  private readonly Mock<ITalentRepository> _talentRepository = new();

  private readonly CreateCharacterCommandHandler _handler;

  private readonly Lineage _elf;
  private readonly Lineage _woodElf;
  private readonly Caste _religieux;

  public CreateCharacterCommandHandlerTests()
  {
    _handler = new(
      _casteRepository.Object,
      _characterQuerier.Object,
      _characterRepository.Object,
      _context,
      _customizationRepository.Object,
      _educationRepository.Object,
      _languageRepository.Object,
      _lineageQuerier.Object,
      _lineageRepository.Object,
      _permissionService.Object,
      _storageService.Object,
      _talentRepository.Object);

    _elf = new Lineage(_context.World, new Name("Elfe"));
    _lineageRepository.Setup(x => x.LoadAsync(_elf.Id, _cancellationToken)).ReturnsAsync(_elf);
    _lineageQuerier.Setup(x => x.HasChildrenAsync(_elf, _cancellationToken)).ReturnsAsync(true);

    _woodElf = new Lineage(_context.World, new Name("Elfe sylvain"), _elf);
    _lineageRepository.Setup(x => x.LoadAsync(_woodElf.Id, _cancellationToken)).ReturnsAsync(_woodElf);

    _religieux = new Caste(_context.World, new Name("Religion"));
    _casteRepository.Setup(x => x.LoadAsync(_religieux.Id, _cancellationToken)).ReturnsAsync(_religieux);
  }

  [Fact(DisplayName = "It should throw EntityNotFoundException when the caste was not found.")]
  public async Task Given_CasteNotFound_When_HandleAsync_Then_EntityNotFoundException()
  {
    CreateCharacterPayload payload = new()
    {
      Name = "Sir Moe Lester",
      LineageId = _woodElf.EntityId
    };
    CreateCharacterCommand command = new(payload);

    var exception = await Assert.ThrowsAsync<EntityNotFoundException>(async () => await _handler.HandleAsync(command, _cancellationToken));
    Assert.Equal(_context.WorldId.ToGuid(), exception.WorldId);
    Assert.Equal(Caste.EntityKind, exception.EntityKind);
    Assert.Equal(payload.CasteId, exception.EntityId);
    Assert.Equal("CasteId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw EntityNotFoundException when the education was not found.")]
  public async Task Given_EducationNotFound_When_HandleAsync_Then_EntityNotFoundException()
  {
    CreateCharacterPayload payload = new()
    {
      Name = "Sir Moe Lester",
      LineageId = _woodElf.EntityId,
      CasteId = _religieux.EntityId
    };
    CreateCharacterCommand command = new(payload);

    var exception = await Assert.ThrowsAsync<EntityNotFoundException>(async () => await _handler.HandleAsync(command, _cancellationToken));
    Assert.Equal(_context.WorldId.ToGuid(), exception.WorldId);
    Assert.Equal(Education.EntityKind, exception.EntityKind);
    Assert.Equal(payload.EducationId, exception.EntityId);
    Assert.Equal("EducationId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw EntityNotFoundException when the lineage was not found.")]
  public async Task Given_LineageNotFound_When_HandleAsync_Then_EntityNotFoundException()
  {
    CreateCharacterPayload payload = new()
    {
      Name = "Sir Moe Lester"
    };
    CreateCharacterCommand command = new(payload);

    var exception = await Assert.ThrowsAsync<EntityNotFoundException>(async () => await _handler.HandleAsync(command, _cancellationToken));
    Assert.Equal(_context.WorldId.ToGuid(), exception.WorldId);
    Assert.Equal(Lineage.EntityKind, exception.EntityKind);
    Assert.Equal(payload.LineageId, exception.EntityId);
    Assert.Equal("LineageId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw InvalidLineageException when the lineage has children.")]
  public async Task Given_LineageHasChildren_When_HandleAsync_Then_InvalidLineageException()
  {
    CreateCharacterPayload payload = new()
    {
      Name = "Sir Moe Lester",
      LineageId = _elf.EntityId
    };
    CreateCharacterCommand command = new(payload);

    var exception = await Assert.ThrowsAsync<InvalidLineageException>(async () => await _handler.HandleAsync(command, _cancellationToken));
    Assert.Equal(_context.WorldId.ToGuid(), exception.WorldId);
    Assert.Equal(payload.LineageId, exception.LineageId);
    Assert.Equal("LineageId", exception.PropertyName);
  }
}
