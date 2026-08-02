using Krakenar.Contracts.Search;
using Logitar;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Builders;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Actors;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Talents;
using SkillCraft.Api.Core.Talents.Models;

namespace SkillCraft.Api.IntegrationTests.Talents;

[Trait(Traits.Category, Categories.Integration)]
public class TalentIntegrationTests : IntegrationTests
{
  private readonly ITalentRepository _talentRepository;
  private readonly ITalentService _talentService;

  private Talent _melee = null!;
  private Talent _talent = null!;

  public TalentIntegrationTests() : base()
  {
    _talentRepository = ServiceProvider.GetRequiredService<ITalentRepository>();
    _talentService = ServiceProvider.GetRequiredService<ITalentService>();
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    _melee = TalentBuilder.Melee(Faker, Context.World);
    _talent = new TalentBuilder(Faker).WithWorld(Context.World).Build();
    await _talentRepository.SaveAsync([_melee, _talent]);
  }

  [Theory(DisplayName = "It should create a new talent.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_NotExist_When_CreateOrReplace_Then_Created(bool withId)
  {
    CreateOrReplaceTalentPayload payload = CreateFormationMartialePayload();
    Guid? id = withId ? Guid.NewGuid() : null;

    CreateOrReplaceTalentResult result = await _talentService.CreateOrReplaceAsync(payload, id);
    Assert.True(result.Created);
    TalentModel talent = result.Talent;
    Assert.NotNull(talent);

    if (id.HasValue)
    {
      Assert.Equal(id.Value, talent.Id);
    }
    else
    {
      Assert.NotEqual(Guid.Empty, talent.Id);
    }
    Assert.Equal(4, talent.Version);
    Assert.Equal(Actor, talent.CreatedBy);
    Assert.Equal(DateTime.UtcNow, talent.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(talent.CreatedBy, talent.UpdatedBy);
    Assert.True(talent.CreatedOn < talent.UpdatedOn);

    AssertFormationMartiale(payload, talent);
  }

  [Fact(DisplayName = "It should filter search results by allow multiple purchases.")]
  public async Task Given_AllowMultiplePurchases_When_Search_Then_Results()
  {
    Talent competence = TalentBuilder.Competence(Faker, Context.World);
    await _talentRepository.SaveAsync(competence);

    SearchTalentsPayload payload = new()
    {
      AllowMultiplePurchases = true
    };

    SearchResults<TalentModel> results = await _talentService.SearchAsync(payload);
    Assert.Equal(1, results.Total);

    TalentModel talent = Assert.Single(results.Items);
    Assert.Equal(competence.ResourceId, talent.Id);
  }

  [Fact(DisplayName = "It should filter search results by required talent ID.")]
  public async Task Given_RequiredTalentId_When_Search_Then_Results()
  {
    Talent formationMartiale = TalentBuilder.FormationMartiale(Faker, Context.World, _melee);
    await _talentRepository.SaveAsync(formationMartiale);

    SearchTalentsPayload payload = new()
    {
      RequiredTalentId = _melee.ResourceId
    };

    SearchResults<TalentModel> results = await _talentService.SearchAsync(payload);
    Assert.Equal(1, results.Total);

    TalentModel talent = Assert.Single(results.Items);
    Assert.Equal(formationMartiale.ResourceId, talent.Id);
  }

  [Fact(DisplayName = "It should filter search results by skill.")]
  public async Task Given_Skill_When_Search_Then_Results()
  {
    SearchTalentsPayload payload = new()
    {
      Skill = Skill.Melee
    };

    SearchResults<TalentModel> results = await _talentService.SearchAsync(payload);
    Assert.Equal(1, results.Total);

    TalentModel talent = Assert.Single(results.Items);
    Assert.Equal(_melee.ResourceId, talent.Id);
  }

  [Fact(DisplayName = "It should filter search results by tiers.")]
  public async Task Given_Tiers_When_Search_Then_Results()
  {
    Talent charge = TalentBuilder.Charge(Faker, Context.World, _melee);
    await _talentRepository.SaveAsync(charge);

    SearchTalentsPayload payload = new()
    {
      Tiers = [charge.Tier.Value]
    };

    SearchResults<TalentModel> results = await _talentService.SearchAsync(payload);
    Assert.Equal(1, results.Total);

    TalentModel talent = Assert.Single(results.Items);
    Assert.Equal(charge.ResourceId, talent.Id);
  }

  [Fact(DisplayName = "It should read a talent by ID.")]
  public async Task Given_Id_When_Read_Then_Read()
  {
    TalentModel? talent = await _talentService.ReadAsync(_talent.ResourceId);
    Assert.NotNull(talent);
    Assert.Equal(_talent.ResourceId, talent.Id);
  }

  [Fact(DisplayName = "It should replace an existing talent.")]
  public async Task Given_Exists_When_CreateOrReplace_Then_Replaced()
  {
    CreateOrReplaceTalentPayload payload = CreateFormationMartialePayload();
    Guid id = _talent.ResourceId;

    CreateOrReplaceTalentResult result = await _talentService.CreateOrReplaceAsync(payload, id);
    Assert.False(result.Created);
    TalentModel talent = result.Talent;
    Assert.NotNull(talent);

    Assert.Equal(id, talent.Id);
    Assert.Equal(5, talent.Version);
    Assert.Equal(_talent.CreatedBy, talent.CreatedBy.GetActorId());
    Assert.Equal(_talent.CreatedOn.AsUniversalTime(), talent.CreatedOn, TimeSpan.FromMilliseconds(1));
    Assert.Equal(Actor, talent.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, talent.UpdatedOn, TimeSpan.FromSeconds(10));

    AssertFormationMartiale(payload, talent);
  }

  [Fact(DisplayName = "It should return empty search results.")]
  public async Task Given_NoMatch_When_Search_Then_EmptyResults()
  {
    Context.World = new WorldBuilder(Faker).Build();

    SearchTalentsPayload payload = new();

    SearchResults<TalentModel> results = await _talentService.SearchAsync(payload);
    Assert.Equal(0, results.Total);
    Assert.Empty(results.Items);
  }

  [Fact(DisplayName = "It should return null when no talent was found.")]
  public async Task Given_NotFound_When_Read_Then_NullReturned()
  {
    Context.World = new WorldBuilder(Faker).Build();

    Assert.Null(await _talentService.ReadAsync(_talent.ResourceId));
  }

  [Fact(DisplayName = "It should return null when the talent was not found.")]
  public async Task Given_NotFound_When_Update_Then_NullReturned()
  {
    Assert.Null(await _talentService.UpdateAsync(Guid.Empty, new UpdateTalentPayload()));
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task Given_Matches_When_Search_Then_Results()
  {
    Talent competence = TalentBuilder.Competence(Faker, Context.World);
    Talent charge = TalentBuilder.Charge(Faker, Context.World, _melee);
    await _talentRepository.SaveAsync([competence, charge]);

    SearchTalentsPayload payload = new()
    {
      Skip = 1,
      Limit = 1
    };
    payload.Search.Operator = SearchOperator.Or;
    payload.Search.Terms.Add(new SearchTerm("%bonus%"));
    payload.Search.Terms.Add(new SearchTerm("%courant%"));
    payload.Search.Terms.Add(new SearchTerm("%melee%"));
    payload.Ids.AddRange([_talent.ResourceId, Guid.Empty, competence.ResourceId, charge.ResourceId]);
    payload.Sort.Add(new TalentSortOption(TalentSort.Name, isDescending: true));

    SearchResults<TalentModel> results = await _talentService.SearchAsync(payload);
    Assert.Equal(2, results.Total);

    TalentModel talent = Assert.Single(results.Items);
    Assert.Equal(charge.ResourceId, talent.Id);
  }

  [Fact(DisplayName = "It should throw InvalidTalentSkillException when updating a talent.")]
  public async Task Given_AllowMultiplePurchasesAndSkill_When_Update_Then_InvalidTalentSkillException()
  {
    Talent talent = TalentBuilder.Competence(Faker, Context.World);
    await _talentRepository.SaveAsync(talent);

    UpdateTalentPayload payload = new()
    {
      Skill = new Optional<Skill?>(Skill.Melee)
    };

    var exception = await Assert.ThrowsAsync<InvalidTalentSkillException>(async () => await _talentService.UpdateAsync(talent.ResourceId, payload));
    Assert.Equal(Context.WorldUid, exception.WorldId);
    Assert.Equal(talent.ResourceId, exception.TalentId);
    Assert.Equal(Skill.Melee, exception.AttemptedSkill);
  }

  [Fact(DisplayName = "It should throw InvalidRequiredTalentException when updating a talent.")]
  public async Task Given_RequiredTalentTierGreater_When_Update_Then_InvalidRequiredTalentException()
  {
    Talent charge = TalentBuilder.Charge(Faker, Context.World, _melee);
    await _talentRepository.SaveAsync(charge);

    UpdateTalentPayload payload = new()
    {
      RequiredTalentId = new Optional<Guid?>(charge.ResourceId)
    };

    var exception = await Assert.ThrowsAsync<InvalidRequiredTalentException>(async () => await _talentService.UpdateAsync(_melee.ResourceId, payload));
    Assert.Equal(Context.WorldUid, exception.WorldId);
    Assert.Equal(_melee.ResourceId, exception.RequiringTalentId);
    Assert.Equal(charge.ResourceId, exception.RequiredTalentId);
    Assert.Equal(_melee.Tier.Value, exception.RequiringTalentTier);
    Assert.Equal(charge.Tier.Value, exception.RequiredTalentTier);
  }

  [Fact(DisplayName = "It should throw ImmutablePropertyException when the tier is changing.")]
  public async Task Given_DifferentTier_When_Replace_Then_ImmutablePropertyException()
  {
    CreateOrReplaceTalentPayload payload = CreateFormationMartialePayload();
    payload.Tier = 1;

    var exception = await Assert.ThrowsAsync<ImmutablePropertyException<int>>(async () => await _talentService.CreateOrReplaceAsync(payload, _talent.ResourceId));
    Assert.Equal(Context.WorldUid, exception.WorldId);
    Assert.Equal(Talent.ResourceKind, exception.ResourceKind);
    Assert.Equal(_talent.ResourceId, exception.ResourceId);
    Assert.Equal(_talent.Tier.Value, exception.ExpectedValue);
    Assert.Equal(payload.Tier, exception.AttemptedValue);
    Assert.Equal("Tier", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw TalentNotFoundException when creating a talent.")]
  public async Task Given_RequiredTalentNotFound_When_Create_Then_TalentNotFoundException()
  {
    CreateOrReplaceTalentPayload payload = CreateFormationMartialePayload();
    payload.RequiredTalentId = Guid.Empty;

    var exception = await Assert.ThrowsAsync<TalentNotFoundException>(async () => await _talentService.CreateOrReplaceAsync(payload));
    Assert.Equal(Context.WorldUid, exception.WorldId);
    Assert.Equal(payload.RequiredTalentId.Value, exception.TalentId);
    Assert.Equal("RequiredTalentId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw TalentNotFoundException when replacing a talent.")]
  public async Task Given_RequiredTalentNotFound_When_Replace_Then_TalentNotFoundException()
  {
    CreateOrReplaceTalentPayload payload = CreateFormationMartialePayload();
    payload.RequiredTalentId = Guid.Empty;

    var exception = await Assert.ThrowsAsync<TalentNotFoundException>(async () => await _talentService.CreateOrReplaceAsync(payload, _talent.ResourceId));
    Assert.Equal(Context.WorldUid, exception.WorldId);
    Assert.Equal(payload.RequiredTalentId.Value, exception.TalentId);
    Assert.Equal("RequiredTalentId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw TalentNotFoundException when updating a talent.")]
  public async Task Given_RequiredTalentNotFound_When_Update_Then_TalentNotFoundException()
  {
    UpdateTalentPayload payload = new()
    {
      RequiredTalentId = new Optional<Guid?>(Guid.Empty)
    };

    var exception = await Assert.ThrowsAsync<TalentNotFoundException>(async () => await _talentService.UpdateAsync(_talent.ResourceId, payload));
    Assert.Equal(Context.WorldUid, exception.WorldId);
    Assert.Equal(payload.RequiredTalentId.Value, exception.TalentId);
    Assert.Equal("RequiredTalentId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when creating a talent.")]
  public async Task Given_NotAllowed_When_Create_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder(Faker).Build();

    CreateOrReplaceTalentPayload payload = CreateFormationMartialePayload();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _talentService.CreateOrReplaceAsync(payload));
    Assert.Equal(Context.UserUid, exception.UserId);
    Assert.Equal(Actions.CreateTalent, exception.Action);
    Assert.Equal(Context.World?.Identifier.ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when replacing a talent.")]
  public async Task Given_NotAllowed_When_Replace_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder(Faker).Build();

    CreateOrReplaceTalentPayload payload = CreateFormationMartialePayload();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _talentService.CreateOrReplaceAsync(payload, _talent.ResourceId));
    Assert.Equal(Context.UserUid, exception.UserId);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(_talent.Identifier.ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when updating a talent.")]
  public async Task Given_NotAllowed_When_Update_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder(Faker).Build();

    UpdateTalentPayload payload = new();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _talentService.UpdateAsync(_talent.ResourceId, payload));
    Assert.Equal(Context.UserUid, exception.UserId);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(_talent.Identifier.ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should update an existing talent.")]
  public async Task Given_Exists_When_Update_Then_Updated()
  {
    Guid id = _talent.ResourceId;
    CreateOrReplaceTalentPayload create = CreateFormationMartialePayload();
    UpdateTalentPayload payload = new()
    {
      Name = create.Name,
      Summary = new Optional<string>(create.Summary),
      Content = new Optional<string>(create.Content),
      AllowMultiplePurchases = create.AllowMultiplePurchases,
      Skill = new Optional<Skill?>(create.Skill),
      RequiredTalentId = new Optional<Guid?>(create.RequiredTalentId)
    };

    TalentModel? talent = await _talentService.UpdateAsync(id, payload);
    Assert.NotNull(talent);

    Assert.Equal(id, talent.Id);
    Assert.Equal(5, talent.Version);
    Assert.Equal(_talent.CreatedBy, talent.CreatedBy.GetActorId());
    Assert.Equal(_talent.CreatedOn.AsUniversalTime(), talent.CreatedOn, TimeSpan.FromMilliseconds(1));
    Assert.Equal(Actor, talent.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, talent.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Equal(payload.Name.CleanTrim(), talent.Name);
    Assert.Equal(payload.Summary.Value?.CleanTrim(), talent.Summary);
    Assert.Equal(payload.Content.Value?.CleanTrim(), talent.Content);
    Assert.Equal(payload.AllowMultiplePurchases, talent.AllowMultiplePurchases);
    Assert.Equal(payload.Skill.Value, talent.Skill);
    Assert.NotNull(talent.RequiredTalent);
    Assert.Equal(_melee.ResourceId, talent.RequiredTalent.Id);
  }

  private CreateOrReplaceTalentPayload CreateFormationMartialePayload() => new()
  {
    Tier = 0,
    Name = " Formation martiale ",
    Summary = "  Accorde la maîtrise des armes et armures moyennes en combat.  ",
    Content = "   Le personnage acquiert les capacités suivantes :\n\n- Il est [formé](/regles/equipement/armes/formation) au maniement des [armes martiales](/regles/equipement/armes/martiales) de mêlée.\n- Il est [formé](/regles/equipement/armures/formation) au port des [armures moyennes](/regles/equipement/armures/moyennes) et à l’utilisation des [boucliers moyens](/regles/equipement/boucliers).\n- Lorsqu’il dégaine ou rengaine une arme, il peut en faire de même avec un bouclier en [action libre](/regles/combat/deroulement/tour).   ",
    AllowMultiplePurchases = false,
    Skill = Skill.Melee,
    RequiredTalentId = _melee.ResourceId
  };

  private void AssertFormationMartiale(CreateOrReplaceTalentPayload payload, TalentModel talent)
  {
    Assert.Equal(payload.Tier, talent.Tier);
    Assert.Equal(payload.Name.CleanTrim(), talent.Name);
    Assert.Equal(payload.Summary?.CleanTrim(), talent.Summary);
    Assert.Equal(payload.Content?.CleanTrim(), talent.Content);
    Assert.Equal(payload.AllowMultiplePurchases, talent.AllowMultiplePurchases);
    Assert.Equal(payload.Skill, talent.Skill);
    Assert.NotNull(talent.RequiredTalent);
    Assert.Equal(_melee.ResourceId, talent.RequiredTalent.Id);
  }
}
