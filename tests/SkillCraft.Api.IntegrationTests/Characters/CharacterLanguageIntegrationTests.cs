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
public class CharacterLanguageIntegrationTests : IntegrationTests
{
  private readonly ICasteRepository _casteRepository;
  private readonly ICharacterLanguageService _characterLanguageService;
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
  private Talent _langueSupplementaire = null!;
  private Talent _orientation = null!;
  private Talent _perception = null!;
  private Talent _roublardise = null!;
  private Talent _trousses = null!;
  private Language _celfique = null!;
  private Language _commun = null!;
  private Language _sylvestre = null!;
  private Item _denier = null!;
  private CharacterModel _character = null!;

  public CharacterLanguageIntegrationTests()
  {
    _casteRepository = ServiceProvider.GetRequiredService<ICasteRepository>();
    _characterLanguageService = ServiceProvider.GetRequiredService<ICharacterLanguageService>();
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

    _celfique = LanguageBuilder.Celfique(Faker, Context.World, elfique);
    _commun = LanguageBuilder.Common(Faker, Context.World, renon);
    _sylvestre = LanguageBuilder.Sylvestre(Faker, Context.World, elfique);
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
    _langueSupplementaire = TalentBuilder.LangueSupplementaire(Faker, Context.World);
    _orientation = TalentBuilder.Orientation(Faker, Context.World);
    _perception = TalentBuilder.Perception(Faker, Context.World);
    _roublardise = TalentBuilder.Roublardise(Faker, Context.World);
    _trousses = TalentBuilder.Trousses(Faker, Context.World, _roublardise);
    await _talentRepository.SaveAsync([_artisanat, _connaissance, _furtivite, _langueSupplementaire, _orientation, _perception, _roublardise, _trousses]);

    _denier = ItemBuilder.Denier(Faker, Context.World);
    await _itemRepository.SaveAsync(_denier);

    _character = await _characterService.CreateAsync(CreateCharacterPayload());
  }

  [Fact(DisplayName = "It should return null when creating or replacing a language and the character was not found.")]
  public async Task Given_CharacterNotFound_When_CreateOrReplace_Then_NullReturned()
  {
    Assert.Null(await _characterLanguageService.CreateOrReplaceAsync(Guid.Empty, _celfique.ResourceId, CreateExtraPayload()));
  }

  [Fact(DisplayName = "It should throw LanguageNotFoundException when the language was not found.")]
  public async Task Given_LanguageNotFound_When_CreateOrReplace_Then_LanguageNotFoundException()
  {
    var exception = await Assert.ThrowsAsync<LanguageNotFoundException>(
      async () => await _characterLanguageService.CreateOrReplaceAsync(_character.Id, Guid.Empty, CreateExtraPayload()));
    Assert.Equal(Context.WorldUid, exception.WorldId);
    Assert.Equal(Guid.Empty, exception.LanguageId);
    Assert.Equal("LanguageId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should add a language to a character.")]
  public async Task Given_NotExists_When_CreateOrReplace_Then_Created()
  {
    Assert.NotNull(await _characterLanguageService.RemoveAsync(_character.Id, _commun.ResourceId));

    CreateOrReplaceCharacterLanguagePayload payload = CreateExtraPayload();

    CreateOrReplaceCharacterLanguageResult? result = await _characterLanguageService.CreateOrReplaceAsync(_character.Id, _celfique.ResourceId, payload);
    Assert.NotNull(result);
    Assert.True(result.Created);

    CharacterModel character = result.Character;
    Assert.Equal(_character.Id, character.Id);
    Assert.Equal(4, character.Version);
    Assert.Equal(_character.CreatedBy, character.CreatedBy);
    Assert.Equal(_character.CreatedOn, character.CreatedOn, TimeSpan.FromMilliseconds(1));
    Assert.Equal(Actor, character.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, character.UpdatedOn, TimeSpan.FromSeconds(10));
    Assert.True(_character.UpdatedOn < character.UpdatedOn);

    Assert.Equal(2, character.Languages.Count);
    Assert.Contains(character.Languages, language => language.Language.Id == _sylvestre.ResourceId);

    CharacterLanguageModel language = Assert.Single(character.Languages, item => item.Language.Id == _celfique.ResourceId);
    AssertLanguage(payload, language);
    Assert.Equal(Actor, language.CreatedBy);
    Assert.Equal(DateTime.UtcNow, language.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(language.CreatedBy, language.UpdatedBy);
    Assert.Equal(language.CreatedOn, language.UpdatedOn);
  }

  [Fact(DisplayName = "It should replace an existing language.")]
  public async Task Given_Exists_When_CreateOrReplace_Then_Replaced()
  {
    CreateOrReplaceCharacterLanguagePayload create = CreateTalentPayload();
    CreateOrReplaceCharacterLanguageResult? created = await _characterLanguageService.CreateOrReplaceAsync(_character.Id, _celfique.ResourceId, create);
    Assert.NotNull(created);
    Assert.True(created.Created);
    CharacterLanguageModel existing = Assert.Single(created.Character.Languages, language => language.Language.Id == _celfique.ResourceId);

    CreateOrReplaceCharacterLanguagePayload payload = new()
    {
      Source = create.Source,
      Target = create.Target,
      Notes = "  Appris auprès d’un érudit elfe.  "
    };

    CreateOrReplaceCharacterLanguageResult? result = await _characterLanguageService.CreateOrReplaceAsync(_character.Id, _celfique.ResourceId, payload);
    Assert.NotNull(result);
    Assert.False(result.Created);

    CharacterModel character = result.Character;
    Assert.Equal(_character.Id, character.Id);
    Assert.Equal(4, character.Version);
    Assert.Equal(Actor, character.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, character.UpdatedOn, TimeSpan.FromSeconds(10));

    CharacterLanguageModel language = Assert.Single(character.Languages, item => item.Language.Id == _celfique.ResourceId);
    Assert.Equal(existing.Language.Id, language.Language.Id);
    AssertLanguage(payload, language);
    Assert.Equal(existing.CreatedBy, language.CreatedBy);
    Assert.Equal(existing.CreatedOn, language.CreatedOn, TimeSpan.FromMilliseconds(1));
    Assert.Equal(Actor, language.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, language.UpdatedOn, TimeSpan.FromSeconds(10));
    Assert.True(existing.UpdatedOn < language.UpdatedOn);
  }

  [Fact(DisplayName = "It should throw ImmutablePropertyException when the source is changing.")]
  public async Task Given_DifferentSource_When_Replace_Then_ImmutablePropertyException()
  {
    CreateOrReplaceCharacterLanguagePayload payload = CreateTalentPayload();

    var exception = await Assert.ThrowsAsync<ImmutablePropertyException<CharacterLanguageSource>>(
      async () => await _characterLanguageService.CreateOrReplaceAsync(_character.Id, _commun.ResourceId, payload));
    Assert.Equal(Context.WorldUid, exception.WorldId);
    Assert.Equal(Character.ResourceKind, exception.ResourceKind);
    Assert.Equal(_character.Id, exception.ResourceId);
    Assert.Equal(CharacterLanguageSource.Extra, exception.ExpectedValue);
    Assert.Equal(payload.Source, exception.AttemptedValue);
    Assert.Equal("Source", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ImmutablePropertyException when the target is changing.")]
  public async Task Given_DifferentTarget_When_Replace_Then_ImmutablePropertyException()
  {
    CreateOrReplaceCharacterLanguagePayload create = CreateTalentPayload();
    CreateOrReplaceCharacterLanguageResult? created = await _characterLanguageService.CreateOrReplaceAsync(_character.Id, _celfique.ResourceId, create);
    Assert.NotNull(created);

    CreateOrReplaceCharacterLanguagePayload payload = new()
    {
      Source = create.Source,
      Target = Guid.NewGuid().ToString(),
      Notes = create.Notes
    };

    var exception = await Assert.ThrowsAsync<ImmutablePropertyException<string>>(
      async () => await _characterLanguageService.CreateOrReplaceAsync(_character.Id, _celfique.ResourceId, payload));
    Assert.Equal(Context.WorldUid, exception.WorldId);
    Assert.Equal(Character.ResourceKind, exception.ResourceKind);
    Assert.Equal(_character.Id, exception.ResourceId);
    Assert.Equal(create.Target, exception.ExpectedValue);
    Assert.Equal(payload.Target, exception.AttemptedValue);
    Assert.Equal("Target", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when creating or replacing a language.")]
  public async Task Given_NotAllowed_When_CreateOrReplace_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder(Faker).Build();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(
      async () => await _characterLanguageService.CreateOrReplaceAsync(_character.Id, _celfique.ResourceId, CreateExtraPayload()));
    Assert.Equal(Context.ActorId?.Value, exception.Principal);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(new ResourceIdentifier(Character.ResourceKind, _character.Id, Context.WorldId).ToString(), exception.Resource);
    Assert.Equal(Context.WorldUid, exception.WorldId);
  }

  [Fact(DisplayName = "It should return null when removing a language and the character was not found.")]
  public async Task Given_CharacterNotFound_When_Remove_Then_NullReturned()
  {
    Assert.Null(await _characterLanguageService.RemoveAsync(Guid.Empty, _commun.ResourceId));
  }

  [Fact(DisplayName = "It should return null when the language was not found.")]
  public async Task Given_LanguageNotFound_When_Remove_Then_NullReturned()
  {
    Assert.Null(await _characterLanguageService.RemoveAsync(_character.Id, Guid.Empty));
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when removing a language.")]
  public async Task Given_NotAllowed_When_Remove_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder(Faker).Build();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(
      async () => await _characterLanguageService.RemoveAsync(_character.Id, _commun.ResourceId));
    Assert.Equal(Context.ActorId?.Value, exception.Principal);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(new ResourceIdentifier(Character.ResourceKind, _character.Id, Context.WorldId).ToString(), exception.Resource);
    Assert.Equal(Context.WorldUid, exception.WorldId);
  }

  [Fact(DisplayName = "It should remove an existing language.")]
  public async Task Given_Exists_When_Remove_Then_Removed()
  {
    CharacterModel? character = await _characterLanguageService.RemoveAsync(_character.Id, _commun.ResourceId);
    Assert.NotNull(character);

    Assert.Equal(_character.Id, character.Id);
    Assert.Equal(3, character.Version);
    Assert.Equal(Actor, character.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, character.UpdatedOn, TimeSpan.FromSeconds(10));
    Assert.True(_character.UpdatedOn < character.UpdatedOn);

    Assert.Single(character.Languages);
    Assert.Contains(character.Languages, language => language.Language.Id == _sylvestre.ResourceId);
    Assert.DoesNotContain(character.Languages, language => language.Language.Id == _commun.ResourceId);
  }

  private static void AssertLanguage(CreateOrReplaceCharacterLanguagePayload payload, CharacterLanguageModel language)
  {
    Assert.Equal(payload.Source, language.Source);
    Assert.Equal(payload.Target?.CleanTrim(), language.Target);
    Assert.Equal(payload.Notes?.Trim(), language.Notes);
  }

  private static CreateOrReplaceCharacterLanguagePayload CreateExtraPayload() => new()
  {
    Source = CharacterLanguageSource.Extra,
    Notes = "  Appris auprès d’un marchand.  "
  };

  private CreateOrReplaceCharacterLanguagePayload CreateTalentPayload()
  {
    CharacterTalentModel talent = Assert.Single(_character.Talents, item => item.Talent.Id == _langueSupplementaire.ResourceId);
    return new CreateOrReplaceCharacterLanguagePayload
    {
      Source = CharacterLanguageSource.Talent,
      Target = talent.Id.ToString(),
      Notes = "  Langue supplémentaire : Celfique.  "
    };
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
        TalentId = _langueSupplementaire.ResourceId,
        Qualifier = "Celfique",
        Discounts = [new CharacterTalentDiscountModel(CharacterTalentDiscountSource.Lineage, _elfe.ResourceId.ToString(), 2)]
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
