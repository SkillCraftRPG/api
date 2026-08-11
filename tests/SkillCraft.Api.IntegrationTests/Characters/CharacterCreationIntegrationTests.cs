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
using SkillCraft.Api.Core.Scripts;
using SkillCraft.Api.Core.Talents;

namespace SkillCraft.Api.IntegrationTests.Characters;

[Trait(Traits.Category, Categories.Integration)]
public class CharacterCreationIntegrationTests : IntegrationTests
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

  private Script _elfique = null!;
  private Script _renon = null!;
  private Language _celfique = null!;
  private Language _commun = null!;
  private Language _sylvestre = null!;
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
  private Item _denier = null!;

  public CharacterCreationIntegrationTests()
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

    _elfique = ScriptBuilder.Elfique(Faker, Context.World);
    _renon = ScriptBuilder.Renon(Faker, Context.World);
    await _scriptRepository.SaveAsync([_elfique, _renon]);

    _celfique = LanguageBuilder.Celfique(Faker, Context.World, _elfique);
    _commun = LanguageBuilder.Common(Faker, Context.World, _renon);
    _sylvestre = LanguageBuilder.Sylvestre(Faker, Context.World, _elfique);
    await _languageRepository.SaveAsync([_celfique, _commun, _sylvestre]);

    _elfe = LineageBuilder.Elfe(Faker, Context.World);
    _hautElfe = LineageBuilder.HautElfe(Faker, Context.World, _elfe, _celfique);
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

  [Fact(DisplayName = "It should create a new character.")]
  public async Task Given_Payload_When_Create_Then_Created()
  {
    CreateCharacterPayload payload = CreatePayload();

    CharacterModel character = await _characterService.CreateAsync(payload);

    Assert.NotEqual(Guid.Empty, character.Id);
    Assert.Equal(1, character.Version);
    Assert.Equal(Actor, character.CreatedBy);
    Assert.Equal(DateTime.UtcNow, character.UpdatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(character.CreatedBy, character.UpdatedBy);
    Assert.Equal(character.CreatedOn, character.UpdatedOn);

    Assert.Equal(payload.Name.Trim(), character.Name);
    Assert.Equal(payload.DominantHand, character.DominantHand);

    Assert.Equal(0, character.Tier);
    Assert.Equal(0, character.Level);
    Assert.Equal(0, character.Experience);

    Assert.Equal(_hautElfe.ResourceId, character.Lineage.Id);
    Assert.Equal(_elfe.ResourceId, character.Lineage.Parent?.Id);
    Assert.Equal(_artisan.ResourceId, character.Caste.Id);
    Assert.Equal(_judicieux.ResourceId, character.Education.Id);

    Assert.Equal(payload.Appearance, character.Appearance);
    Assert.Equal(payload.Alignment, character.Alignment);
    Assert.Equal(payload.Personality, character.Personality);
    Assert.Equal(payload.Background, character.Background);

    AssertAttribute(character.Attributes.Dexterity, payload.Attributes.Dexterity);
    AssertAttribute(character.Attributes.Health, payload.Attributes.Health);
    AssertAttribute(character.Attributes.Intellect, payload.Attributes.Intellect);
    AssertAttribute(character.Attributes.Senses, payload.Attributes.Senses);
    AssertAttribute(character.Attributes.Vigor, payload.Attributes.Vigor);

    AssertStatistic(character.Statistics.Dodge, 12);
    AssertStatistic(character.Statistics.Initiative, -2);
    AssertStatistic(character.Statistics.Learning, 6);
    AssertStatistic(character.Statistics.Load, 30);
    AssertStatistic(character.Statistics.Power, 3);
    AssertStatistic(character.Statistics.Precision, 9);
    AssertStatistic(character.Statistics.Stamina, 25);
    AssertStatistic(character.Statistics.Stratagem, 7);
    AssertStatistic(character.Statistics.Strength, 1);
    AssertStatistic(character.Statistics.Vitality, 25);

    AssertSkill(character.Skills.Acrobatics, 0, 0, 2);
    AssertSkill(character.Skills.Athletics, 0, 0, -2);
    AssertSkill(character.Skills.Crafting, 1, 1, 2);
    AssertSkill(character.Skills.Deception, 0, 0, 0);
    AssertSkill(character.Skills.Diplomacy, 0, 0, 0);
    AssertSkill(character.Skills.Discipline, 0, 0, 0);
    AssertSkill(character.Skills.Insight, 0, 0, -1);
    AssertSkill(character.Skills.Investigation, 0, 0, 1);
    AssertSkill(character.Skills.Knowledge, 1, 1, 1);
    AssertSkill(character.Skills.Linguistics, 0, 0, 1);
    AssertSkill(character.Skills.Medicine, 0, 0, 1);
    AssertSkill(character.Skills.Melee, 0, 0, -2);
    AssertSkill(character.Skills.Occultism, 0, 0, -1);
    AssertSkill(character.Skills.Orientation, 1, 1, 2);
    AssertSkill(character.Skills.Perception, 1, 1, -1);
    AssertSkill(character.Skills.Performance, 0, 0, 0);
    AssertSkill(character.Skills.Resistance, 0, 0, 0);
    AssertSkill(character.Skills.Stealth, 1, 1, 2);
    AssertSkill(character.Skills.Survival, 0, 0, -1);
    AssertSkill(character.Skills.Thievery, 1, 1, 2);

    AssertSpeed(character.Speeds.Walk, 6);
    AssertSpeed(character.Speeds.Climb, 0);
    AssertSpeed(character.Speeds.Swim, 0);
    AssertSpeed(character.Speeds.Fly, 0);
    Assert.False(character.Speeds.Hover);
    AssertSpeed(character.Speeds.Burrow, 0);

    Assert.Equal(0, character.Vitality);
    Assert.Equal(0, character.Stamina);
    Assert.Equal(0, character.BloodAlcoholContent);
    Assert.Equal(0, character.Intoxication);
    Assert.Equal(0, character.Hope);

    Assert.Equal(2, character.Customizations.Count);
    Assert.Contains(character.Customizations, customization => customization.Id == _fignolage.ResourceId);
    Assert.Contains(character.Customizations, customization => customization.Id == _hemophobe.ResourceId);

    Assert.Equal(2, character.Languages.Count);
    Assert.Contains(character.Languages, language => language.Language.Id == _commun.ResourceId
      && language.Source == CharacterLanguageSource.Extra && language.Target is null && language.Notes is null
      && language.CreatedBy.Equals(Actor) && language.UpdatedBy.Equals(Actor)
      && language.CreatedOn == character.CreatedOn && language.UpdatedOn == language.CreatedOn);
    Assert.Contains(character.Languages, language => language.Language.Id == _sylvestre.ResourceId
      && language.Source == CharacterLanguageSource.Extra && language.Target is null && language.Notes is null
      && language.CreatedBy.Equals(Actor) && language.UpdatedBy.Equals(Actor)
      && language.CreatedOn == character.CreatedOn && language.UpdatedOn == language.CreatedOn);

    Assert.Equal(7, character.Talents.Count);
    Assert.Contains(character.Talents, talent => talent.Id != Guid.Empty && talent.Talent.Id == _artisanat.ResourceId
      && talent.Qualifier is null && talent.Notes is null && talent.Discounts.Count == 0 && talent.Cost == 2
      && talent.CreatedBy.Equals(Actor) && talent.UpdatedBy.Equals(Actor)
      && talent.CreatedOn == character.CreatedOn && talent.UpdatedOn == talent.CreatedOn);
    Assert.Contains(character.Talents, talent => talent.Id != Guid.Empty && talent.Talent.Id == _connaissance.ResourceId
      && talent.Qualifier is null && talent.Notes is null && talent.Discounts.Count == 0 && talent.Cost == 2
      && talent.CreatedBy.Equals(Actor) && talent.UpdatedBy.Equals(Actor)
      && talent.CreatedOn == character.CreatedOn && talent.UpdatedOn == talent.CreatedOn);
    Assert.Contains(character.Talents, talent => talent.Id != Guid.Empty && talent.Talent.Id == _furtivite.ResourceId
      && talent.Qualifier is null && talent.Notes is null && talent.Discounts.Count == 0 && talent.Cost == 2
      && talent.CreatedBy.Equals(Actor) && talent.UpdatedBy.Equals(Actor)
      && talent.CreatedOn == character.CreatedOn && talent.UpdatedOn == talent.CreatedOn);
    Assert.Contains(character.Talents, talent => talent.Id != Guid.Empty && talent.Talent.Id == _orientation.ResourceId
      && talent.Qualifier is null && talent.Notes is null
      && AssertCharacterTalentDiscount(Assert.Single(talent.Discounts), CharacterTalentDiscountSource.Lineage, _elfe.ResourceId.ToString(), amount: 1) && talent.Cost == 1
      && talent.CreatedBy.Equals(Actor) && talent.UpdatedBy.Equals(Actor)
      && talent.CreatedOn == character.CreatedOn && talent.UpdatedOn == talent.CreatedOn);
    Assert.Contains(character.Talents, talent => talent.Id != Guid.Empty && talent.Talent.Id == _perception.ResourceId
      && talent.Qualifier is null && talent.Notes is null
      && AssertCharacterTalentDiscount(Assert.Single(talent.Discounts), CharacterTalentDiscountSource.Lineage, _elfe.ResourceId.ToString(), amount: 1) && talent.Cost == 1
      && talent.CreatedBy.Equals(Actor) && talent.UpdatedBy.Equals(Actor)
      && talent.CreatedOn == character.CreatedOn && talent.UpdatedOn == talent.CreatedOn);
    Assert.Contains(character.Talents, talent => talent.Id != Guid.Empty && talent.Talent.Id == _roublardise.ResourceId
      && talent.Qualifier is null && talent.Notes is null && talent.Discounts.Count == 0 && talent.Cost == 2
      && talent.CreatedBy.Equals(Actor) && talent.UpdatedBy.Equals(Actor)
      && talent.CreatedOn == character.CreatedOn && talent.UpdatedOn == talent.CreatedOn);
    Assert.Contains(character.Talents, talent => talent.Id != Guid.Empty && talent.Talent.Id == _trousses.ResourceId
      && talent.Qualifier is null && talent.Notes is null && talent.Discounts.Count == 0 && talent.Cost == 2
      && talent.CreatedBy.Equals(Actor) && talent.UpdatedBy.Equals(Actor)
      && talent.CreatedOn == character.CreatedOn && talent.UpdatedOn == talent.CreatedOn);

    Assert.Equal(0, character.Points.Attributes);
    Assert.Equal(0, character.Points.Skills);
    Assert.Equal(0, character.Points.Talents);
  }

  [Fact(DisplayName = "It should throw CasteNotFoundException when the caste was not found.")]
  public async Task Given_CasteNotFound_When_Create_Then_CasteNotFoundException()
  {
    CreateCharacterPayload payload = CreatePayload();
    payload.CasteId = Guid.Empty;

    var exception = await Assert.ThrowsAsync<CasteNotFoundException>(async () => await _characterService.CreateAsync(payload));
    Assert.Equal(Context.WorldUid, exception.WorldId);
    Assert.Equal(payload.CasteId, exception.CasteId);
    Assert.Equal("CasteId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw CustomizationsNotFoundException when some customizations were not found.")]
  public async Task Given_CustomizationsNotFound_When_Create_Then_CustomizationsNotFoundException()
  {
    CreateCharacterPayload payload = CreatePayload();
    payload.CustomizationIds = [Guid.Empty];

    var exception = await Assert.ThrowsAsync<CustomizationsNotFoundException>(async () => await _characterService.CreateAsync(payload));
    Assert.Equal(Context.WorldUid, exception.WorldId);
    Assert.True(payload.CustomizationIds.SequenceEqual(exception.CustomizationIds));
    Assert.Equal("CustomizationIds", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw EducationNotFoundException when the lineage was not found.")]
  public async Task Given_LineageNotFound_When_Create_Then_EducationNotFoundException()
  {
    CreateCharacterPayload payload = CreatePayload();
    payload.EducationId = Guid.Empty;

    var exception = await Assert.ThrowsAsync<EducationNotFoundException>(async () => await _characterService.CreateAsync(payload));
    Assert.Equal(Context.WorldUid, exception.WorldId);
    Assert.Equal(payload.EducationId, exception.EducationId);
    Assert.Equal("EducationId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ItemNotFoundException when the currency was not found.")]
  public async Task Given_CurrencyNotFound_When_Create_Then_ItemNotFoundException()
  {
    CreateCharacterPayload payload = CreatePayload();
    Assert.NotNull(payload.StartingWealth);
    payload.StartingWealth.CurrencyId = Guid.Empty;

    var exception = await Assert.ThrowsAsync<ItemNotFoundException>(async () => await _characterService.CreateAsync(payload));
    Assert.Equal(Context.WorldUid, exception.WorldId);
    Assert.Equal(payload.StartingWealth.CurrencyId, exception.ItemId);
    Assert.Equal("StartingWealth.CurrencyId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw LanguagesNotFoundException when some languages were not found.")]
  public async Task Given_LanguagesNotFound_When_Create_Then_LanguagesNotFoundException()
  {
    CreateCharacterPayload payload = CreatePayload();
    payload.LanguageIds = [Guid.Empty];

    var exception = await Assert.ThrowsAsync<LanguagesNotFoundException>(async () => await _characterService.CreateAsync(payload));
    Assert.Equal(Context.WorldUid, exception.WorldId);
    Assert.True(payload.LanguageIds.SequenceEqual(exception.LanguageIds));
    Assert.Equal("LanguageIds", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw LineageNotFoundException when the lineage was not found.")]
  public async Task Given_LineageNotFound_When_Create_Then_LineageNotFoundException()
  {
    CreateCharacterPayload payload = CreatePayload();
    payload.LineageId = Guid.Empty;

    var exception = await Assert.ThrowsAsync<LineageNotFoundException>(async () => await _characterService.CreateAsync(payload));
    Assert.Equal(Context.WorldUid, exception.WorldId);
    Assert.Equal(payload.LineageId, exception.LineageId);
    Assert.Equal("LineageId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw TalentsNotFoundException when some languages were not found.")]
  public async Task Given_LanguagesNotFound_When_Create_Then_TalentsNotFoundException()
  {
    CreateCharacterPayload payload = CreatePayload();
    Assert.NotEmpty(payload.Talents);
    payload.Talents.First().TalentId = Guid.Empty;

    var exception = await Assert.ThrowsAsync<TalentsNotFoundException>(async () => await _characterService.CreateAsync(payload));
    Assert.Equal(Context.WorldUid, exception.WorldId);
    Assert.Equal(Guid.Empty, Assert.Single(exception.TalentIds));
    Assert.Equal("Talents", exception.PropertyName);
  }

  private static void AssertAttribute(CharacterAttributeModel attribute, int starting, int progression = 0, int bonus = 0, int? total = null)
  {
    Assert.Equal(starting, attribute.Starting);
    Assert.Equal(progression, attribute.Progression);
    Assert.Equal(bonus, attribute.Bonus);
    Assert.Equal(total ?? (starting + progression + bonus), attribute.Total);
  }
  private static void AssertSkill(CharacterSkillModel skill, int rank, int talents = 0, int attribute = 0, int bonus = 0, int? total = null)
  {
    Assert.Equal(rank, skill.Rank);
    Assert.Equal(talents, skill.Talents);
    Assert.Equal(attribute, skill.Attribute);
    Assert.Equal(bonus, skill.Bonus);
    Assert.Equal(total ?? ((talents < 1 ? (rank / 2) : rank) + talents + attribute + bonus), skill.Total);
  }
  private static void AssertStatistic(CharacterStatisticModel statistic, int @base, int bonus = 0, int? total = null)
  {
    Assert.Equal(@base, statistic.Base);
    Assert.Equal(bonus, statistic.Bonus);
    Assert.Equal(total ?? (@base + bonus), statistic.Total);
  }
  private static void AssertSpeed(CharacterSpeedModel speed, int lineage, int bonus = 0, int encumbrance = 0, int? total = null)
  {
    Assert.Equal(lineage, speed.Lineage);
    Assert.Equal(bonus, speed.Bonus);
    Assert.Equal(encumbrance, speed.Encumbrance);
    Assert.Equal(total ?? (lineage + bonus + encumbrance), speed.Total);
  }

  private static bool AssertCharacterTalentDiscount(CharacterTalentDiscountModel discount, CharacterTalentDiscountSource source, string target, int amount)
  {
    return discount.Source == source && discount.Target == target && discount.Amount == amount;
  }

  private CreateCharacterPayload CreatePayload()
  {
    return new CreateCharacterPayload
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
        new AddCharacterTalentPayload
        {
          TalentId = _artisanat.ResourceId
        },
        new AddCharacterTalentPayload
        {
          TalentId = _connaissance.ResourceId
        },
        new AddCharacterTalentPayload
        {
          TalentId = _furtivite.ResourceId
        },
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
        new AddCharacterTalentPayload
        {
          TalentId = _roublardise.ResourceId
        },
        new AddCharacterTalentPayload
        {
          TalentId = _trousses.ResourceId
        }
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
      Background = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nullam molestie lectus nec consectetur condimentum. Donec quis sollicitudin lectus. Duis blandit dapibus pharetra. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Phasellus dolor neque, elementum sit amet massa eu, venenatis ornare purus. Vestibulum quis urna nec mi cursus aliquet. Phasellus et finibus arcu. Fusce et lorem viverra ipsum bibendum sodales. Vivamus sit amet dui ornare, fringilla massa vitae, porta justo. Suspendisse vitae ligula vitae orci iaculis laoreet id tristique tellus. Mauris vel augue egestas, tincidunt velit eget, sagittis nibh. Vestibulum egestas dictum mattis. Nulla consequat dolor nibh, maximus egestas risus tincidunt quis. Donec interdum ipsum urna, id fringilla lorem auctor id.",
      StartingWealth = new StartingWealthPayload
      {
        CurrencyId = _denier.ResourceId,
        Quantity = 300
      }
    };
  }
}
