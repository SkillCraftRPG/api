using Logitar;
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
public class CharacterExperienceIntegrationTests : IntegrationTests
{
  private readonly ICasteRepository _casteRepository;
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

  public CharacterExperienceIntegrationTests()
  {
    _casteRepository = ServiceProvider.GetRequiredService<ICasteRepository>();
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
    await _customizationRepository.SaveAsync([_fignolage, _hemophobe]);

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

  [Fact(DisplayName = "It should return null when gaining experience and the character was not found.")]
  public async Task Given_CharacterNotFound_When_GainExperience_Then_NullReturned()
  {
    Assert.Null(await _characterService.GainExperienceAsync(Guid.Empty, new GainCharacterExperiencePayload { Experience = 50 }));
  }

  [Fact(DisplayName = "It should increase a character's experience without changing the level.")]
  public async Task Given_BelowThreshold_When_GainExperience_Then_ExperienceIncreased()
  {
    GainCharacterExperiencePayload payload = new() { Experience = 50 };

    CharacterModel? character = await _characterService.GainExperienceAsync(_character.Id, payload);
    Assert.NotNull(character);

    Assert.Equal(_character.Id, character.Id);
    Assert.Equal(_character.Version + 1, character.Version);
    Assert.Equal(_character.CreatedBy, character.CreatedBy);
    Assert.Equal(_character.CreatedOn, character.CreatedOn, TimeSpan.FromMilliseconds(1));
    Assert.Equal(Actor, character.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, character.UpdatedOn, TimeSpan.FromSeconds(10));
    Assert.True(_character.UpdatedOn < character.UpdatedOn);

    Assert.Equal(50, character.Experience);
    Assert.Equal(0, character.Level);
  }

  [Fact(DisplayName = "It should increase a character's level when experience reaches the next threshold.")]
  public async Task Given_AboveThreshold_When_GainExperience_Then_LevelIncreased()
  {
    Assert.NotNull(await _characterService.GainExperienceAsync(_character.Id, new GainCharacterExperiencePayload { Experience = 50 }));

    GainCharacterExperiencePayload payload = new() { Experience = 50 };

    CharacterModel? character = await _characterService.GainExperienceAsync(_character.Id, payload);
    Assert.NotNull(character);

    Assert.Equal(_character.Id, character.Id);
    Assert.Equal(_character.Version + 2, character.Version);
    Assert.Equal(_character.CreatedBy, character.CreatedBy);
    Assert.Equal(_character.CreatedOn, character.CreatedOn, TimeSpan.FromMilliseconds(1));
    Assert.Equal(Actor, character.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, character.UpdatedOn, TimeSpan.FromSeconds(10));
    Assert.True(_character.UpdatedOn < character.UpdatedOn);

    Assert.Equal(100, character.Experience);
    Assert.Equal(1, character.Level);
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
