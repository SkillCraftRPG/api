using Krakenar.Contracts.Search;
using Logitar;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Builders;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Actors;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Spells;
using SkillCraft.Api.Core.Spells.Models;

namespace SkillCraft.Api.IntegrationTests.Spells;

[Trait(Traits.Category, Categories.Integration)]
public class SpellIntegrationTests : IntegrationTests
{
  private readonly ISpellRepository _spellRepository;
  private readonly ISpellService _spellService;

  private Spell _spell = null!;

  public SpellIntegrationTests() : base()
  {
    _spellRepository = ServiceProvider.GetRequiredService<ISpellRepository>();
    _spellService = ServiceProvider.GetRequiredService<ISpellService>();
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    _spell = new SpellBuilder(Faker).WithWorld(Context.World).Build();
    await _spellRepository.SaveAsync(_spell);
  }

  [Theory(DisplayName = "It should create a new spell.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_NotExist_When_CreateOrReplace_Then_Created(bool withId)
  {
    CreateOrReplaceSpellPayload payload = CreateProtectionContreLaMagiePayload();
    Guid? id = withId ? Guid.NewGuid() : null;

    CreateOrReplaceSpellResult result = await _spellService.CreateOrReplaceAsync(payload, id);
    Assert.True(result.Created);
    SpellModel spell = result.Spell;
    Assert.NotNull(spell);

    if (id.HasValue)
    {
      Assert.Equal(id.Value, spell.Id);
    }
    else
    {
      Assert.NotEqual(Guid.Empty, spell.Id);
    }
    Assert.Equal(2, spell.Version);
    Assert.Equal(Actor, spell.CreatedBy);
    Assert.Equal(DateTime.UtcNow, spell.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(spell.CreatedBy, spell.UpdatedBy);
    Assert.True(spell.CreatedOn < spell.UpdatedOn);

    Assert.Equal(payload.Tier, spell.Tier);
    Assert.Equal(payload.Name.CleanTrim(), spell.Name);
    Assert.Equal(payload.Summary?.CleanTrim(), spell.Summary);
    Assert.Equal(payload.Content?.CleanTrim(), spell.Content);
  }

  [Fact(DisplayName = "It should filter search results by tiers.")]
  public async Task Given_Tiers_When_Search_Then_Results()
  {
    Spell protection = SpellBuilder.ProtectionContreLaMagie(Faker, Context.World);
    await _spellRepository.SaveAsync(protection);

    SearchSpellsPayload payload = new()
    {
      Tiers = [protection.Tier.Value]
    };

    SearchResults<SpellModel> results = await _spellService.SearchAsync(payload);
    Assert.Equal(1, results.Total);

    SpellModel spell = Assert.Single(results.Items);
    Assert.Equal(protection.ResourceId, spell.Id);
  }

  [Fact(DisplayName = "It should read a spell by ID.")]
  public async Task Given_Id_When_Read_Then_Read()
  {
    SpellModel? spell = await _spellService.ReadAsync(_spell.ResourceId);
    Assert.NotNull(spell);
    Assert.Equal(_spell.ResourceId, spell.Id);
  }

  [Fact(DisplayName = "It should replace an existing spell.")]
  public async Task Given_Exists_When_CreateOrReplace_Then_Replaced()
  {
    CreateOrReplaceSpellPayload payload = CreateProtectionContreLaMagiePayload();
    payload.Tier = _spell.Tier.Value;
    Guid id = _spell.ResourceId;

    CreateOrReplaceSpellResult result = await _spellService.CreateOrReplaceAsync(payload, id);
    Assert.False(result.Created);
    SpellModel spell = result.Spell;
    Assert.NotNull(spell);

    Assert.Equal(id, spell.Id);
    Assert.Equal(3, spell.Version);
    Assert.Equal(_spell.CreatedBy, spell.CreatedBy.GetActorId());
    Assert.Equal(_spell.CreatedOn.AsUniversalTime(), spell.CreatedOn, TimeSpan.FromMilliseconds(1));
    Assert.Equal(Actor, spell.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, spell.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Equal(payload.Tier, spell.Tier);
    Assert.Equal(payload.Name.CleanTrim(), spell.Name);
    Assert.Equal(payload.Summary?.CleanTrim(), spell.Summary);
    Assert.Equal(payload.Content?.CleanTrim(), spell.Content);
  }

  [Fact(DisplayName = "It should return empty search results.")]
  public async Task Given_NoMatch_When_Search_Then_EmptyResults()
  {
    Context.World = new WorldBuilder(Faker).Build();

    SearchSpellsPayload payload = new();

    SearchResults<SpellModel> results = await _spellService.SearchAsync(payload);
    Assert.Equal(0, results.Total);
    Assert.Empty(results.Items);
  }

  [Fact(DisplayName = "It should return null when no spell was found.")]
  public async Task Given_NotFound_When_Read_Then_NullReturned()
  {
    Context.World = new WorldBuilder(Faker).Build();

    Assert.Null(await _spellService.ReadAsync(_spell.ResourceId));
  }

  [Fact(DisplayName = "It should return null when the spell was not found.")]
  public async Task Given_NotFound_When_Update_Then_NullReturned()
  {
    Assert.Null(await _spellService.UpdateAsync(Guid.Empty, new UpdateSpellPayload()));
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task Given_Matches_When_Search_Then_Results()
  {
    Spell flammeSacree = new SpellBuilder(Faker).WithWorld(Context.World).WithName("Flamme sacrée").Build();
    Spell miracle = new SpellBuilder(Faker).WithWorld(Context.World).WithName("Miracle").Build();
    Spell mirage = new SpellBuilder(Faker).WithWorld(Context.World).WithName("Mirage").Build();
    await _spellRepository.SaveAsync([flammeSacree, miracle, mirage]);

    SearchSpellsPayload payload = new()
    {
      Skip = 1,
      Limit = 1
    };
    payload.Search.Terms.Add(new SearchTerm("%i%"));
    payload.Ids.AddRange([_spell.ResourceId, Guid.Empty, miracle.ResourceId, mirage.ResourceId]);
    payload.Sort.Add(new SpellSortOption(SpellSort.Name, isDescending: true));

    SearchResults<SpellModel> results = await _spellService.SearchAsync(payload);
    Assert.Equal(2, results.Total);

    SpellModel spell = Assert.Single(results.Items);
    Assert.Equal(miracle.ResourceId, spell.Id);
  }

  [Fact(DisplayName = "It should throw ImmutablePropertyException when the tier is changing.")]
  public async Task Given_DifferentTier_When_Replace_Then_ImmutablePropertyException()
  {
    CreateOrReplaceSpellPayload payload = CreateProtectionContreLaMagiePayload();

    var exception = await Assert.ThrowsAsync<ImmutablePropertyException<int>>(async () => await _spellService.CreateOrReplaceAsync(payload, _spell.ResourceId));
    Assert.Equal(Context.WorldUid, exception.WorldId);
    Assert.Equal(Spell.ResourceKind, exception.ResourceKind);
    Assert.Equal(_spell.ResourceId, exception.ResourceId);
    Assert.Equal(_spell.Tier.Value, exception.ExpectedValue);
    Assert.Equal(payload.Tier, exception.AttemptedValue);
    Assert.Equal("Tier", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when creating a spell.")]
  public async Task Given_NotAllowed_When_Create_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder(Faker).Build();

    CreateOrReplaceSpellPayload payload = CreateProtectionContreLaMagiePayload();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _spellService.CreateOrReplaceAsync(payload));
    Assert.Equal(Context.UserUid, exception.UserId);
    Assert.Equal(Actions.CreateSpell, exception.Action);
    Assert.Equal(Context.World?.Identifier.ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when replacing a spell.")]
  public async Task Given_NotAllowed_When_Replace_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder(Faker).Build();

    CreateOrReplaceSpellPayload payload = CreateProtectionContreLaMagiePayload();
    payload.Tier = _spell.Tier.Value;

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _spellService.CreateOrReplaceAsync(payload, _spell.ResourceId));
    Assert.Equal(Context.UserUid, exception.UserId);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(_spell.Identifier.ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when updating a spell.")]
  public async Task Given_NotAllowed_When_Update_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder(Faker).Build();

    UpdateSpellPayload payload = new();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _spellService.UpdateAsync(_spell.ResourceId, payload));
    Assert.Equal(Context.UserUid, exception.UserId);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(_spell.Identifier.ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should update an existing spell.")]
  public async Task Given_Exists_When_Update_Then_Updated()
  {
    Guid id = _spell.ResourceId;
    CreateOrReplaceSpellPayload create = CreateProtectionContreLaMagiePayload();
    UpdateSpellPayload payload = new()
    {
      Name = create.Name,
      Summary = new Optional<string>(create.Summary),
      Content = new Optional<string>(create.Content)
    };

    SpellModel? spell = await _spellService.UpdateAsync(id, payload);
    Assert.NotNull(spell);

    Assert.Equal(id, spell.Id);
    Assert.Equal(3, spell.Version);
    Assert.Equal(_spell.CreatedBy, spell.CreatedBy.GetActorId());
    Assert.Equal(_spell.CreatedOn.AsUniversalTime(), spell.CreatedOn, TimeSpan.FromMilliseconds(1));
    Assert.Equal(Actor, spell.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, spell.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Equal(payload.Name.CleanTrim(), spell.Name);
    Assert.Equal(payload.Summary.Value?.CleanTrim(), spell.Summary);
    Assert.Equal(payload.Content.Value?.CleanTrim(), spell.Content);
  }

  private static CreateOrReplaceSpellPayload CreateProtectionContreLaMagiePayload() => new()
  {
    Tier = 1,
    Name = " Protection contre la magie ",
    Summary = "  Détection, dissipation et interruption des effets magiques adverses.  ",
    Content = "   Pouvoir défensif et utilitaire permettant de détecter la magie, dissiper les effets surnaturels actifs et interrompre les incantations ennemies en réaction.   "
  };
}
