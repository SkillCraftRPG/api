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
public class CharacterModifierIntegrationTests : IntegrationTests
{
  private readonly ICasteRepository _casteRepository;
  private readonly ICharacterModifierService _characterModifierService;
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

  public CharacterModifierIntegrationTests()
  {
    _casteRepository = ServiceProvider.GetRequiredService<ICasteRepository>();
    _characterModifierService = ServiceProvider.GetRequiredService<ICharacterModifierService>();
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

  [Fact(DisplayName = "It should return null when creating or replacing a modifier and the character was not found.")]
  public async Task Given_CharacterNotFound_When_CreateOrReplace_Then_NullReturned()
  {
    Assert.Null(await _characterModifierService.CreateOrReplaceAsync(Guid.Empty, CreateDexterityPayload()));
  }

  [Theory(DisplayName = "It should add a modifier to a character.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_NotExist_When_CreateOrReplace_Then_Created(bool withId)
  {
    CreateOrReplaceCharacterModifierPayload payload = CreateDexterityPayload();
    Guid? id = withId ? Guid.NewGuid() : null;

    CreateOrReplaceCharacterModifierResult? result = await _characterModifierService.CreateOrReplaceAsync(_character.Id, payload, id);
    Assert.NotNull(result);
    Assert.True(result.Created);

    CharacterModel character = result.Character;
    Assert.Equal(_character.Id, character.Id);
    Assert.Equal(3, character.Version);
    Assert.Equal(_character.CreatedBy, character.CreatedBy);
    Assert.Equal(_character.CreatedOn, character.CreatedOn, TimeSpan.FromMilliseconds(1));
    Assert.Equal(Actor, character.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, character.UpdatedOn, TimeSpan.FromSeconds(10));
    Assert.True(_character.UpdatedOn < character.UpdatedOn);

    CharacterModifierModel modifier = Assert.Single(character.Modifiers);
    if (id.HasValue)
    {
      Assert.Equal(id.Value, modifier.Id);
    }
    else
    {
      Assert.NotEqual(Guid.Empty, modifier.Id);
    }
    AssertModifier(payload, modifier);
    Assert.Equal(Actor, modifier.CreatedBy);
    Assert.Equal(DateTime.UtcNow, modifier.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(modifier.CreatedBy, modifier.UpdatedBy);
    Assert.Equal(modifier.CreatedOn, modifier.UpdatedOn);

    Assert.Equal(1, character.Attributes.Dexterity.Modifiers);
    Assert.Equal(3, character.Attributes.Dexterity.Total);
  }

  [Fact(DisplayName = "It should replace an existing modifier.")]
  public async Task Given_Exists_When_CreateOrReplace_Then_Replaced()
  {
    CreateOrReplaceCharacterModifierPayload create = CreateDexterityPayload();
    CreateOrReplaceCharacterModifierResult? created = await _characterModifierService.CreateOrReplaceAsync(_character.Id, create);
    Assert.NotNull(created);
    CharacterModifierModel existing = Assert.Single(created.Character.Modifiers);

    CreateOrReplaceCharacterModifierPayload payload = new()
    {
      Kind = create.Kind,
      Target = create.Target,
      Value = 2,
      Name = "  Grâce féerique  ",
      Notes = "  Un don des elfes.  "
    };

    CreateOrReplaceCharacterModifierResult? result = await _characterModifierService.CreateOrReplaceAsync(_character.Id, payload, existing.Id);
    Assert.NotNull(result);
    Assert.False(result.Created);

    CharacterModel character = result.Character;
    Assert.Equal(_character.Id, character.Id);
    Assert.Equal(4, character.Version);
    Assert.Equal(Actor, character.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, character.UpdatedOn, TimeSpan.FromSeconds(10));

    CharacterModifierModel modifier = Assert.Single(character.Modifiers);
    Assert.Equal(existing.Id, modifier.Id);
    AssertModifier(payload, modifier);
    Assert.Equal(existing.CreatedBy, modifier.CreatedBy);
    Assert.Equal(existing.CreatedOn, modifier.CreatedOn, TimeSpan.FromMilliseconds(1));
    Assert.Equal(Actor, modifier.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, modifier.UpdatedOn, TimeSpan.FromSeconds(10));
    Assert.True(existing.UpdatedOn < modifier.UpdatedOn);

    Assert.Equal(2, character.Attributes.Dexterity.Modifiers);
    Assert.Equal(4, character.Attributes.Dexterity.Total);
  }

  [Fact(DisplayName = "It should throw ImmutablePropertyException when the kind is changing.")]
  public async Task Given_DifferentKind_When_Replace_Then_ImmutablePropertyException()
  {
    CreateOrReplaceCharacterModifierPayload create = CreateDexterityPayload();
    CreateOrReplaceCharacterModifierResult? created = await _characterModifierService.CreateOrReplaceAsync(_character.Id, create);
    Assert.NotNull(created);
    CharacterModifierModel existing = Assert.Single(created.Character.Modifiers);

    CreateOrReplaceCharacterModifierPayload payload = new()
    {
      Kind = CharacterModifierKind.Skill,
      Target = nameof(Skill.Stealth),
      Value = create.Value
    };

    var exception = await Assert.ThrowsAsync<ImmutablePropertyException<CharacterModifierKind>>(
      async () => await _characterModifierService.CreateOrReplaceAsync(_character.Id, payload, existing.Id));
    Assert.Equal(Context.WorldUid, exception.WorldId);
    Assert.Equal(Character.ResourceKind, exception.ResourceKind);
    Assert.Equal(_character.Id, exception.ResourceId);
    Assert.Equal(create.Kind, exception.ExpectedValue);
    Assert.Equal(payload.Kind, exception.AttemptedValue);
    Assert.Equal("Kind", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ImmutablePropertyException when the target is changing.")]
  public async Task Given_DifferentTarget_When_Replace_Then_ImmutablePropertyException()
  {
    CreateOrReplaceCharacterModifierPayload create = CreateDexterityPayload();
    CreateOrReplaceCharacterModifierResult? created = await _characterModifierService.CreateOrReplaceAsync(_character.Id, create);
    Assert.NotNull(created);
    CharacterModifierModel existing = Assert.Single(created.Character.Modifiers);

    CreateOrReplaceCharacterModifierPayload payload = new()
    {
      Kind = create.Kind,
      Target = nameof(GameAttribute.Health),
      Value = create.Value
    };

    var exception = await Assert.ThrowsAsync<ImmutablePropertyException<string>>(
      async () => await _characterModifierService.CreateOrReplaceAsync(_character.Id, payload, existing.Id));
    Assert.Equal(Context.WorldUid, exception.WorldId);
    Assert.Equal(Character.ResourceKind, exception.ResourceKind);
    Assert.Equal(_character.Id, exception.ResourceId);
    Assert.Equal(nameof(GameAttribute.Dexterity), exception.ExpectedValue);
    Assert.Equal(nameof(GameAttribute.Health), exception.AttemptedValue);
    Assert.Equal("Target", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when creating or replacing a modifier.")]
  public async Task Given_NotAllowed_When_CreateOrReplace_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder(Faker).Build();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(
      async () => await _characterModifierService.CreateOrReplaceAsync(_character.Id, CreateDexterityPayload()));
    Assert.Equal(Context.ActorId?.Value, exception.Principal);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(new ResourceIdentifier(Character.ResourceKind, _character.Id, Context.WorldId).ToString(), exception.Resource);
    Assert.Equal(Context.WorldUid, exception.WorldId);
  }

  [Fact(DisplayName = "It should return null when removing a modifier and the character was not found.")]
  public async Task Given_CharacterNotFound_When_Remove_Then_NullReturned()
  {
    Assert.Null(await _characterModifierService.RemoveAsync(Guid.Empty, Guid.NewGuid()));
  }

  [Fact(DisplayName = "It should return null when the modifier was not found.")]
  public async Task Given_ModifierNotFound_When_Remove_Then_NullReturned()
  {
    Assert.Null(await _characterModifierService.RemoveAsync(_character.Id, Guid.Empty));
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when removing a modifier.")]
  public async Task Given_NotAllowed_When_Remove_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder(Faker).Build();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(
      async () => await _characterModifierService.RemoveAsync(_character.Id, Guid.NewGuid()));
    Assert.Equal(Context.ActorId?.Value, exception.Principal);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(new ResourceIdentifier(Character.ResourceKind, _character.Id, Context.WorldId).ToString(), exception.Resource);
    Assert.Equal(Context.WorldUid, exception.WorldId);
  }

  [Fact(DisplayName = "It should remove an existing modifier.")]
  public async Task Given_Exists_When_Remove_Then_Removed()
  {
    CreateOrReplaceCharacterModifierResult? created = await _characterModifierService.CreateOrReplaceAsync(_character.Id, CreateDexterityPayload());
    Assert.NotNull(created);
    CharacterModifierModel existing = Assert.Single(created.Character.Modifiers);

    CharacterModel? character = await _characterModifierService.RemoveAsync(_character.Id, existing.Id);
    Assert.NotNull(character);

    Assert.Equal(_character.Id, character.Id);
    Assert.Equal(4, character.Version);
    Assert.Equal(Actor, character.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, character.UpdatedOn, TimeSpan.FromSeconds(10));
    Assert.Empty(character.Modifiers);
    Assert.Equal(0, character.Attributes.Dexterity.Modifiers);
    Assert.Equal(2, character.Attributes.Dexterity.Total);
  }

  private static void AssertModifier(CreateOrReplaceCharacterModifierPayload payload, CharacterModifierModel modifier)
  {
    Assert.Equal(payload.Kind, modifier.Kind);
    Assert.Equal(payload.Target, modifier.Target);
    Assert.Equal(payload.Value, modifier.Value);
    Assert.Equal(payload.Name?.CleanTrim(), modifier.Name);
    Assert.Equal(payload.Notes?.Trim(), modifier.Notes);
  }

  private static CreateOrReplaceCharacterModifierPayload CreateDexterityPayload() => new()
  {
    Kind = CharacterModifierKind.Attribute,
    Target = nameof(GameAttribute.Dexterity),
    Value = 1,
    Name = "  Bénédiction  ",
    Notes = "  Un don de dextérité.  "
  };

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
