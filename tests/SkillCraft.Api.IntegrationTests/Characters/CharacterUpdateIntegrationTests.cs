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
public class CharacterUpdateIntegrationTests : IntegrationTests
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

  public CharacterUpdateIntegrationTests()
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
  }

  [Fact(DisplayName = "It should return null when the character was not found.")]
  public async Task Given_NotFound_When_Update_Then_NullReturned()
  {
    Assert.Null(await _characterService.UpdateAsync(Guid.Empty, new UpdateCharacterPayload()));
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when updating a character.")]
  public async Task Given_NotAllowed_When_Update_Then_PermissionDeniedException()
  {
    CharacterModel created = await _characterService.CreateAsync(CreatePayload());

    Context.User = new UserBuilder(Faker).Build();

    UpdateCharacterPayload payload = new();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _characterService.UpdateAsync(created.Id, payload));
    Assert.Equal(Context.ActorId?.Value, exception.Principal);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(new ResourceIdentifier(Character.ResourceKind, created.Id, Context.WorldId).ToString(), exception.Resource);
    Assert.Equal(Context.WorldUid, exception.WorldId);
  }

  [Fact(DisplayName = "It should update an existing character.")]
  public async Task Given_Exists_When_Update_Then_Updated()
  {
    CharacterModel created = await _characterService.CreateAsync(CreatePayload());

    UpdateCharacterPayload payload = new()
    {
      Name = "  Aelar Moonwhisper  ",
      DominantHand = new Optional<DominantHand?>(DominantHand.Left),
      Appearance = new CharacterAppearance(170, 510, 62, " Pale ", " Blue ", " Blond "),
      Alignment = new Optional<Alignment?>(Alignment.ChaoticGood),
      Personality = new CharacterPersonality(
        "Je préfère l’ombre aux regards.",
        "La liberté n’a de sens que partagée.",
        "Je fuis les engagements durables."),
      Background = new Optional<string>("  Nouveau passé pour le personnage.  "),
      Vitality = new CharacterVitalityModel
      {
        Current = 18,
        Temporary = 4,
        Stun = 2
      },
      Stamina = 12,
      Hope = new CharacterHopeModel
      {
        Current = 2,
        Maximum = 3
      },
      BloodAlcoholContent = 1,
      Intoxication = 2
    };

    CharacterModel? character = await _characterService.UpdateAsync(created.Id, payload);
    Assert.NotNull(character);

    Assert.Equal(created.Id, character.Id);
    Assert.Equal(5, character.Version);
    Assert.Equal(created.CreatedBy, character.CreatedBy);
    Assert.Equal(created.CreatedOn, character.CreatedOn, TimeSpan.FromMilliseconds(1));
    Assert.Equal(Actor, character.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, character.UpdatedOn, TimeSpan.FromSeconds(10));
    Assert.True(created.UpdatedOn < character.UpdatedOn);

    Assert.Equal(payload.Name.CleanTrim(), character.Name);
    Assert.Equal(payload.DominantHand?.Value, character.DominantHand);
    Assert.Equal(payload.Appearance.Height, character.Appearance.Height);
    Assert.Equal(payload.Appearance.Weight, character.Appearance.Weight);
    Assert.Equal(payload.Appearance.Age, character.Appearance.Age);
    Assert.Equal(payload.Appearance.Skin, character.Appearance.Skin);
    Assert.Equal(payload.Appearance.Eyes, character.Appearance.Eyes);
    Assert.Equal(payload.Appearance.Hair, character.Appearance.Hair);
    Assert.Equal(payload.Alignment?.Value, character.Alignment);
    Assert.Equal(payload.Personality.Traits, character.Personality.Traits);
    Assert.Equal(payload.Personality.Ideals, character.Personality.Ideals);
    Assert.Equal(payload.Personality.Flaws, character.Personality.Flaws);
    Assert.Equal(payload.Background?.Value?.Trim(), character.Background);

    Assert.Equal(payload.Vitality.Current, character.Vitality.Current);
    Assert.Equal(payload.Vitality.Temporary, character.Vitality.Temporary);
    Assert.Equal(payload.Vitality.Stun, character.Vitality.Stun);
    Assert.Equal(payload.Stamina, character.Stamina);
    Assert.Equal(payload.Hope.Current, character.Hope.Current);
    Assert.Equal(payload.Hope.Maximum, character.Hope.Maximum);
    Assert.Equal(payload.BloodAlcoholContent, character.BloodAlcoholContent);
    Assert.Equal(payload.Intoxication, character.Intoxication);
  }

  [Fact(DisplayName = "It should update only the status of an existing character.")]
  public async Task Given_Status_When_Update_Then_Updated()
  {
    CharacterModel created = await _characterService.CreateAsync(CreatePayload());

    UpdateCharacterPayload payload = new()
    {
      Vitality = new CharacterVitalityModel
      {
        Current = 10,
        Temporary = 3,
        Stun = 1
      },
      Stamina = 8,
      Hope = new CharacterHopeModel
      {
        Current = 1,
        Maximum = 2
      },
      BloodAlcoholContent = 3,
      Intoxication = 4
    };

    CharacterModel? character = await _characterService.UpdateAsync(created.Id, payload);
    Assert.NotNull(character);

    Assert.Equal(created.Id, character.Id);
    Assert.Equal(3, character.Version);
    Assert.Equal(created.Name, character.Name);
    Assert.Equal(created.DominantHand, character.DominantHand);
    Assert.Equal(created.Appearance, character.Appearance);
    Assert.Equal(created.Alignment, character.Alignment);
    Assert.Equal(created.Personality, character.Personality);
    Assert.Equal(created.Background, character.Background);
    Assert.Equal(Actor, character.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, character.UpdatedOn, TimeSpan.FromSeconds(10));
    Assert.True(created.UpdatedOn < character.UpdatedOn);

    Assert.Equal(payload.Vitality.Current, character.Vitality.Current);
    Assert.Equal(payload.Vitality.Temporary, character.Vitality.Temporary);
    Assert.Equal(payload.Vitality.Stun, character.Vitality.Stun);
    Assert.Equal(payload.Stamina, character.Stamina);
    Assert.Equal(payload.Hope.Current, character.Hope.Current);
    Assert.Equal(payload.Hope.Maximum, character.Hope.Maximum);
    Assert.Equal(payload.BloodAlcoholContent, character.BloodAlcoholContent);
    Assert.Equal(payload.Intoxication, character.Intoxication);
  }

  private CreateCharacterPayload CreatePayload() => new()
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
