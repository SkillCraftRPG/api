using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Builders;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Castes;
using SkillCraft.Api.Core.Characters;
using SkillCraft.Api.Core.Characters.Models;
using SkillCraft.Api.Core.Customizations;
using SkillCraft.Api.Core.Educations;
using SkillCraft.Api.Core.Items;
using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Lineages;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Scripts;
using SkillCraft.Api.Core.Talents;

namespace SkillCraft.Api.IntegrationTests.Characters;

[Trait(Traits.Category, Categories.Integration)]
public class CharacterCustomizationIntegrationTests : IntegrationTests
{
  private readonly ICasteRepository _casteRepository;
  private readonly ICharacterCustomizationService _characterCustomizationService;
  private readonly ICharacterService _characterService;
  private readonly ICustomizationRepository _customizationRepository;
  private readonly IEducationRepository _educationRepository;
  private readonly IItemRepository _itemRepository;
  private readonly ILanguageRepository _languageRepository;
  private readonly ILineageRepository _lineageRepository;
  private readonly IScriptRepository _scriptRepository;
  private readonly ITalentRepository _talentRepository;

  private Lineage _elfe = null!;
  private Lineage _hautElfe = null!;
  private Caste _artisan = null!;
  private Education _judicieux = null!;
  private Customization _fignolage = null!;
  private Customization _hemophobe = null!;
  private Customization _baraque = null!;
  private Talent _artisanat = null!;
  private Talent _connaissance = null!;
  private Talent _furtivite = null!;
  private Talent _orientation = null!;
  private Talent _perception = null!;
  private Talent _roublardise = null!;
  private Talent _trousses = null!;
  private Language _commun = null!;
  private Language _sylvestre = null!;
  private Item _denier = null!;
  private CharacterModel _character = null!;

  public CharacterCustomizationIntegrationTests()
  {
    _casteRepository = ServiceProvider.GetRequiredService<ICasteRepository>();
    _characterCustomizationService = ServiceProvider.GetRequiredService<ICharacterCustomizationService>();
    _characterService = ServiceProvider.GetRequiredService<ICharacterService>();
    _customizationRepository = ServiceProvider.GetRequiredService<ICustomizationRepository>();
    _educationRepository = ServiceProvider.GetRequiredService<IEducationRepository>();
    _itemRepository = ServiceProvider.GetRequiredService<IItemRepository>();
    _languageRepository = ServiceProvider.GetRequiredService<ILanguageRepository>();
    _lineageRepository = ServiceProvider.GetRequiredService<ILineageRepository>();
    _scriptRepository = ServiceProvider.GetRequiredService<IScriptRepository>();
    _talentRepository = ServiceProvider.GetRequiredService<ITalentRepository>();
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    Script elfique = ScriptBuilder.Elfique(Faker, Context.World);
    Script renon = ScriptBuilder.Renon(Faker, Context.World);
    await _scriptRepository.SaveAsync([elfique, renon]);

    Language celfique = LanguageBuilder.Celfique(Faker, Context.World, elfique);
    _commun = LanguageBuilder.Common(Faker, Context.World, renon);
    _sylvestre = LanguageBuilder.Sylvestre(Faker, Context.World, elfique);
    await _languageRepository.SaveAsync([celfique, _commun, _sylvestre]);

    _elfe = LineageBuilder.Elfe(Faker, Context.World);
    _hautElfe = LineageBuilder.HautElfe(Faker, Context.World, _elfe, celfique);
    await _lineageRepository.SaveAsync([_elfe, _hautElfe]);

    _artisan = CasteBuilder.Artisan(Faker, Context.World);
    await _casteRepository.SaveAsync(_artisan);

    _judicieux = EducationBuilder.Judicieux(Faker, Context.World);
    await _educationRepository.SaveAsync(_judicieux);

    _fignolage = CustomizationBuilder.Fignolage(Faker, Context.World);
    _hemophobe = CustomizationBuilder.Hemophobe(Faker, Context.World);
    _baraque = new CustomizationBuilder(Faker)
      .WithWorld(Context.World)
      .WithKind(CustomizationKind.Gift)
      .WithName("Baraqué")
      .WithSummary("Double portée, avantage et dégâts contre objets et structures.")
      .Build();
    await _customizationRepository.SaveAsync([_fignolage, _hemophobe, _baraque]);

    _artisanat = TalentBuilder.Artisanat(Faker, Context.World);
    _connaissance = TalentBuilder.Connaissance(Faker, Context.World);
    _furtivite = TalentBuilder.Furtivite(Faker, Context.World);
    _orientation = TalentBuilder.Orientation(Faker, Context.World);
    _perception = TalentBuilder.Perception(Faker, Context.World);
    _roublardise = TalentBuilder.Roublardise(Faker, Context.World);
    _trousses = TalentBuilder.Trousses(Faker, Context.World, _roublardise);
    await _talentRepository.SaveAsync([_artisanat, _connaissance, _furtivite, _orientation, _perception, _roublardise, _trousses]);

    _denier = ItemBuilder.Denier(Faker, Context.World);
    await _itemRepository.SaveAsync(_denier);

    _character = await _characterService.CreateAsync(CreateCharacterPayload());
  }

  [Fact(DisplayName = "It should return null when adding a customization and the character was not found.")]
  public async Task Given_CharacterNotFound_When_Add_Then_NullReturned()
  {
    Assert.Null(await _characterCustomizationService.AddAsync(Guid.Empty, _baraque.ResourceId));
  }

  [Fact(DisplayName = "It should throw CustomizationNotFoundException when the customization was not found.")]
  public async Task Given_CustomizationNotFound_When_Add_Then_CustomizationNotFoundException()
  {
    var exception = await Assert.ThrowsAsync<CustomizationNotFoundException>(
      async () => await _characterCustomizationService.AddAsync(_character.Id, Guid.Empty));
    Assert.Equal(Context.WorldUid, exception.WorldId);
    Assert.Equal(Guid.Empty, exception.CustomizationId);
    Assert.Equal("CustomizationId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should return the character unchanged when the customization was already added.")]
  public async Task Given_Exists_When_Add_Then_NoOp()
  {
    long version = _character.Version;

    CharacterModel? character = await _characterCustomizationService.AddAsync(_character.Id, _fignolage.ResourceId);
    Assert.NotNull(character);

    Assert.Equal(_character.Id, character.Id);
    Assert.Equal(version, character.Version);
    Assert.Equal(_character.UpdatedOn, character.UpdatedOn, TimeSpan.FromMilliseconds(1));
    Assert.Equal(2, character.Customizations.Count);
    Assert.Contains(character.Customizations, customization => customization.Id == _fignolage.ResourceId);
    Assert.Contains(character.Customizations, customization => customization.Id == _hemophobe.ResourceId);
  }

  [Fact(DisplayName = "It should add a customization to a character.")]
  public async Task Given_NotExists_When_Add_Then_Added()
  {
    CharacterModel? character = await _characterCustomizationService.AddAsync(_character.Id, _baraque.ResourceId);
    Assert.NotNull(character);

    Assert.Equal(_character.Id, character.Id);
    Assert.Equal(3, character.Version);
    Assert.Equal(_character.CreatedBy, character.CreatedBy);
    Assert.Equal(_character.CreatedOn, character.CreatedOn, TimeSpan.FromMilliseconds(1));
    Assert.Equal(Actor, character.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, character.UpdatedOn, TimeSpan.FromSeconds(10));
    Assert.True(_character.UpdatedOn < character.UpdatedOn);

    Assert.Equal(3, character.Customizations.Count);
    Assert.Contains(character.Customizations, customization => customization.Id == _fignolage.ResourceId);
    Assert.Contains(character.Customizations, customization => customization.Id == _hemophobe.ResourceId);
    Assert.Contains(character.Customizations, customization => customization.Id == _baraque.ResourceId);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when adding a customization.")]
  public async Task Given_NotAllowed_When_Add_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder(Faker).Build();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(
      async () => await _characterCustomizationService.AddAsync(_character.Id, _baraque.ResourceId));
    Assert.Equal(Context.ActorId?.Value, exception.Principal);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(new ResourceIdentifier(Character.ResourceKind, _character.Id, Context.WorldId).ToString(), exception.Resource);
    Assert.Equal(Context.WorldUid, exception.WorldId);
  }

  private CreateCharacterPayload CreateCharacterPayload() => new()
  {
    LineageId = _hautElfe.ResourceId,
    LanguageIds = [_commun.ResourceId, _sylvestre.ResourceId],
    Name = "  Ivellios Galanodel  ",
    DominantHand = DominantHand.Right,
    CustomizationIds = [_fignolage.ResourceId, _hemophobe.ResourceId],
    CasteId = _artisan.ResourceId,
    EducationId = _judicieux.ResourceId,
    Talents =
    [
      new AddCharacterTalentPayload { TalentId = _artisanat.ResourceId },
      new AddCharacterTalentPayload { TalentId = _connaissance.ResourceId },
      new AddCharacterTalentPayload { TalentId = _furtivite.ResourceId },
      new AddCharacterTalentPayload
      {
        TalentId = _orientation.ResourceId,
        Discounts = [new CharacterTalentDiscountModel(CharacterTalentDiscountSource.Lineage, _elfe.ResourceId.ToString(), 1)]
      },
      new AddCharacterTalentPayload
      {
        TalentId = _perception.ResourceId,
        Discounts = [new CharacterTalentDiscountModel(CharacterTalentDiscountSource.Lineage, _elfe.ResourceId.ToString(), 1)]
      },
      new AddCharacterTalentPayload { TalentId = _roublardise.ResourceId },
      new AddCharacterTalentPayload { TalentId = _trousses.ResourceId }
    ],
    Attributes = new StartingAttributesModel
    {
      Dexterity = 2,
      Health = 0,
      Intellect = 1,
      Senses = -1,
      Vigor = -2
    },
    Skills =
    [
      new SkillRankPayload { Skill = Skill.Crafting, Rank = 1 },
      new SkillRankPayload { Skill = Skill.Knowledge, Rank = 1 },
      new SkillRankPayload { Skill = Skill.Orientation, Rank = 1 },
      new SkillRankPayload { Skill = Skill.Perception, Rank = 1 },
      new SkillRankPayload { Skill = Skill.Stealth, Rank = 1 },
      new SkillRankPayload { Skill = Skill.Thievery, Rank = 1 }
    ],
    Appearance = new CharacterAppearanceModel
    {
      Height = 161,
      Weight = 492,
      Age = 57,
      Skin = "Blanche",
      Eyes = "Verts",
      Hair = "Roux"
    },
    Alignment = Alignment.LawfulNeutral,
    Personality = new CharacterPersonalityModel
    {
      Traits = "Je supporte mal le gaspillage, matériel ou humain.",
      Ideals = "L’innovation naît de la répétition jamais identique.",
      Flaws = "Je garde tout, incapable de jeter quoi que ce soit."
    },
    Background = "Lorem ipsum dolor sit amet.",
    StartingWealth = new StartingWealthPayload
    {
      CurrencyId = _denier.ResourceId,
      Quantity = 300
    }
  };
}
