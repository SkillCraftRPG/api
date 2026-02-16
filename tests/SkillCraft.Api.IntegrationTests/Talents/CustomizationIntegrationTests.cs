using Krakenar.Contracts.Search;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Contracts;
using SkillCraft.Api.Contracts.Talents;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Talents;

namespace SkillCraft.Api.Talents;

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

    _melee = new Talent(World, new Tier(0), new Name("Mêlée"), UserId)
    {
      Skill = GameSkill.Melee
    };
    _melee.Update(UserId);
    _talent = new Talent(World, new Tier(0), new Name("Talent"), UserId);
    await _talentRepository.SaveAsync([_melee, _talent]);
  }

  [Theory(DisplayName = "It should create a new talent.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_NotExist_When_CreateOrReplace_Then_Created(bool withId)
  {
    CreateOrReplaceTalentPayload payload = new()
    {
      Tier = 0,
      Name = "  Formation martiale  ",
      Summary = "  Accorde la maîtrise des armes et armures moyennes en combat.  ",
      Description = "  Le personnage acquiert les capacités suivantes :\n\n- Il est [formé](/regles/equipement/armes/formation) au maniement des [armes martiales](/regles/equipement/armes/martiales) de mêlée.\n- Il est [formé](/regles/equipement/armures/formation) au port des [armures moyennes](/regles/equipement/armures/moyennes) et à l’utilisation des [boucliers moyens](/regles/equipement/boucliers).\n- Lorsqu’il dégaine ou rengaine une arme, il peut en faire de même avec un bouclier en [action libre](/regles/combat/deroulement/tour).  ",
      AllowMultiplePurchases = false,
      Skill = GameSkill.Melee,
      RequiredTalentId = _melee.EntityId
    };
    Guid? id = withId ? Guid.NewGuid() : null;

    CreateOrReplaceTalentResult result = await _talentService.CreateOrReplaceAsync(payload, id);
    Assert.True(result.Created);
    TalentModel talent = result.Talent;

    if (id.HasValue)
    {
      Assert.Equal(id.Value, talent.Id);
    }
    Assert.Equal(2, talent.Version);
    Assert.Equal(Actor, talent.CreatedBy);
    Assert.Equal(DateTime.UtcNow, talent.CreatedOn, TimeSpan.FromSeconds(1));
    Assert.Equal(Actor, talent.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, talent.UpdatedOn, TimeSpan.FromSeconds(1));

    Assert.Equal(payload.Tier, talent.Tier);
    Assert.Equal(payload.Name.Trim(), talent.Name);
    Assert.Equal(payload.Summary.Trim(), talent.Summary);
    Assert.Equal(payload.Description.Trim(), talent.Description);
    Assert.Equal(payload.AllowMultiplePurchases, talent.AllowMultiplePurchases);
    Assert.Equal(payload.Skill, talent.Skill);
    Assert.Equal(payload.RequiredTalentId, talent.RequiredTalent?.Id);
  }

  [Fact(DisplayName = "It should delete an existing talent.")]
  public async Task Given_Exists_When_Delete_ThenDeleted()
  {
    Guid id = _talent.EntityId;
    TalentModel? talent = await _talentService.DeleteAsync(id);
    Assert.NotNull(talent);
    Assert.Equal(id, talent.Id);
  }

  [Fact(DisplayName = "It should read an existing talent.")]
  public async Task Given_Exists_When_Read_Then_Returned()
  {
    Guid id = _talent.EntityId;
    TalentModel? talent = await _talentService.ReadAsync(id);
    Assert.NotNull(talent);
    Assert.Equal(id, talent.Id);
  }

  [Fact(DisplayName = "It should replace an existing talent.")]
  public async Task Given_Exists_When_CreateOrReplace_Then_Replaced()
  {
    CreateOrReplaceTalentPayload payload = new()
    {
      Tier = 0,
      Name = "  Formation martiale  ",
      Summary = "  Accorde la maîtrise des armes et armures moyennes en combat.  ",
      Description = "  Le personnage acquiert les capacités suivantes :\n\n- Il est [formé](/regles/equipement/armes/formation) au maniement des [armes martiales](/regles/equipement/armes/martiales) de mêlée.\n- Il est [formé](/regles/equipement/armures/formation) au port des [armures moyennes](/regles/equipement/armures/moyennes) et à l’utilisation des [boucliers moyens](/regles/equipement/boucliers).\n- Lorsqu’il dégaine ou rengaine une arme, il peut en faire de même avec un bouclier en [action libre](/regles/combat/deroulement/tour).  ",
      AllowMultiplePurchases = false,
      Skill = GameSkill.Melee,
      RequiredTalentId = _melee.EntityId
    };
    Guid id = _talent.EntityId;

    CreateOrReplaceTalentResult result = await _talentService.CreateOrReplaceAsync(payload, id);
    Assert.False(result.Created);
    TalentModel talent = result.Talent;

    Assert.Equal(id, talent.Id);
    Assert.Equal(2, talent.Version);
    Assert.Equal(Actor, talent.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, talent.UpdatedOn, TimeSpan.FromSeconds(1));

    Assert.Equal(payload.Tier, talent.Tier);
    Assert.Equal(payload.Name.Trim(), talent.Name);
    Assert.Equal(payload.Summary.Trim(), talent.Summary);
    Assert.Equal(payload.Description.Trim(), talent.Description);
    Assert.Equal(payload.AllowMultiplePurchases, talent.AllowMultiplePurchases);
    Assert.Equal(payload.Skill, talent.Skill);
    Assert.Equal(payload.RequiredTalentId, talent.RequiredTalent?.Id);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task Given_Payload_When_Search_Then_Results()
  {
    Talent acrobaties = new(World, new Tier(0), new Name("Acrobaties"), UserId);
    Talent connaissance = new(World, new Tier(0), new Name("Connaissance"), UserId);
    Talent discipline = new(World, new Tier(0), new Name("Discipline"), UserId);
    Talent tromperie = new(World, new Tier(0), new Name("Tromperie"), UserId);
    await _talentRepository.SaveAsync([acrobaties, connaissance, discipline, tromperie]);

    SearchTalentsPayload payload = new()
    {
      Skip = 1,
      Limit = 1
    };
    payload.Ids.AddRange([acrobaties.EntityId, connaissance.EntityId, discipline.EntityId, tromperie.EntityId, Guid.Empty]);
    payload.Search.Operator = SearchOperator.Or;
    payload.Search.Terms.AddRange([new SearchTerm("tal%"), new SearchTerm("%is%"), new SearchTerm("%rie")]);
    payload.Sort.Add(new TalentSortOption(TalentSort.Name));

    SearchResults<TalentModel> results = await _talentService.SearchAsync(payload);
    Assert.Equal(3, results.Total);

    TalentModel result = Assert.Single(results.Items);
    Assert.Equal(discipline.EntityId, result.Id);
  }

  [Theory(DisplayName = "It should return the correct search results (AllowMultiplePurchases).")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_AllowMultiplePurchases_When_Search_Then_Results(bool allowMultiplePurchases)
  {
    Talent competence = new(World, new Tier(0), new Name("Compétence"), UserId);
    competence.AllowMultiplePurchases = true;
    competence.Update(UserId);
    await _talentRepository.SaveAsync(competence);

    SearchTalentsPayload payload = new()
    {
      AllowMultiplePurchases = allowMultiplePurchases
    };
    SearchResults<TalentModel> results = await _talentService.SearchAsync(payload);

    if (allowMultiplePurchases)
    {
      Assert.Equal(1, results.Total);

      TalentModel result = Assert.Single(results.Items);
      Assert.Equal(competence.EntityId, result.Id);
    }
    else
    {
      Assert.Equal(2, results.Total);
      Assert.All(results.Items, talent => Assert.NotEqual(competence.EntityId, talent.Id));
    }
  }

  [Theory(DisplayName = "It should return the correct search results (Skill).")]
  [InlineData("AnY")]
  [InlineData("  none  ")]
  [InlineData("Melee")]
  public async Task Given_Skill_When_Search_Then_Results(string skill)
  {
    Talent acrobaties = new(World, new Tier(0), new Name("Acrobaties"), UserId);
    acrobaties.Skill = GameSkill.Acrobatics;
    acrobaties.Update(UserId);
    await _talentRepository.SaveAsync([acrobaties]);

    SearchTalentsPayload payload = new()
    {
      Skill = skill
    };
    SearchResults<TalentModel> results = await _talentService.SearchAsync(payload);

    if (Enum.TryParse(skill, out GameSkill skillValue))
    {
      Assert.Equal(1, results.Total);

      TalentModel result = Assert.Single(results.Items);
      Assert.Equal(skillValue, result.Skill);
    }
    if (skill.Trim().Equals("any", StringComparison.InvariantCultureIgnoreCase))
    {
      Assert.Equal(2, results.Total);
      Assert.All(results.Items, talent => Assert.NotNull(talent.Skill));
    }
    else if (skill.Trim().Equals("none", StringComparison.InvariantCultureIgnoreCase))
    {
      Assert.Equal(1, results.Total);

      TalentModel result = Assert.Single(results.Items);
      Assert.Null(result.Skill);
    }
  }

  [Theory(DisplayName = "It should return the correct search results (RequiredTalent).")]
  [InlineData("AnY")]
  [InlineData("  none  ")]
  [InlineData("Formation martiale")]
  public async Task Given_RequiredTalent_When_Search_Then_Results(string requiredTalent)
  {
    _talent.Name = new Name("Formation martiale");
    _talent.SetRequiredTalent(_melee);
    _talent.Update(UserId);
    Talent manoeuvresDeCombat = new(World, new Tier(1), new Name("Manœuvres de combat"), UserId);
    manoeuvresDeCombat.SetRequiredTalent(_talent);
    manoeuvresDeCombat.Update(UserId);
    await _talentRepository.SaveAsync([_talent, manoeuvresDeCombat]);

    SearchTalentsPayload payload = new()
    {
      RequiredTalent = requiredTalent == _talent.Name.Value ? _talent.EntityId.ToString() : requiredTalent
    };
    SearchResults<TalentModel> results = await _talentService.SearchAsync(payload);

    if (requiredTalent.Trim().Equals("any", StringComparison.InvariantCultureIgnoreCase))
    {
      Assert.Equal(2, results.Total);
      Assert.All(results.Items, talent => Assert.NotNull(talent.RequiredTalent));
    }
    else if (requiredTalent.Trim().Equals("none", StringComparison.InvariantCultureIgnoreCase))
    {
      Assert.Equal(1, results.Total);

      TalentModel result = Assert.Single(results.Items);
      Assert.Null(result.RequiredTalent);
    }
    else
    {
      Assert.Equal(1, results.Total);

      TalentModel result = Assert.Single(results.Items);
      Assert.Equal(requiredTalent.Trim(), result.RequiredTalent?.Name, ignoreCase: true);
    }
  }

  [Fact(DisplayName = "It should return the correct search results (Tiers).")]
  public async Task Given_Tiers_When_Search_Then_Results()
  {
    Talent manoeuvresDeCombat = new(World, new Tier(1), new Name("Manœuvres de combat"), UserId);
    Talent poigneDeFer = new(World, new Tier(1), new Name("Poigne de fer"), UserId);
    await _talentRepository.SaveAsync([manoeuvresDeCombat, poigneDeFer]);

    SearchTalentsPayload payload = new()
    {
      Tiers = [1, 2, 3]
    };
    SearchResults<TalentModel> results = await _talentService.SearchAsync(payload);

    Assert.Equal(2, results.Total);
    Assert.Contains(results.Items, talent => talent.Id == manoeuvresDeCombat.EntityId);
    Assert.Contains(results.Items, talent => talent.Id == poigneDeFer.EntityId);
  }

  [Fact(DisplayName = "It should update an existing talent.")]
  public async Task Given_Exists_When_Update_ThenUpdated()
  {
    Guid id = _talent.EntityId;
    UpdateTalentPayload payload = new()
    {
      Name = "  Formation martiale  ",
      Summary = new Update<string>("  Accorde la maîtrise des armes et armures moyennes en combat.  "),
      Description = new Update<string>("  Le personnage acquiert les capacités suivantes :\n\n- Il est [formé](/regles/equipement/armes/formation) au maniement des [armes martiales](/regles/equipement/armes/martiales) de mêlée.\n- Il est [formé](/regles/equipement/armures/formation) au port des [armures moyennes](/regles/equipement/armures/moyennes) et à l’utilisation des [boucliers moyens](/regles/equipement/boucliers).\n- Lorsqu’il dégaine ou rengaine une arme, il peut en faire de même avec un bouclier en [action libre](/regles/combat/deroulement/tour).  "),
      AllowMultiplePurchases = false,
      Skill = new Update<GameSkill?>(GameSkill.Melee),
      RequiredTalentId = new Update<Guid?>(_melee.EntityId)
    };

    TalentModel? talent = await _talentService.UpdateAsync(id, payload);
    Assert.NotNull(talent);

    Assert.Equal(id, talent.Id);
    Assert.Equal(2, talent.Version);
    Assert.Equal(Actor, talent.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, talent.UpdatedOn, TimeSpan.FromSeconds(1));

    Assert.Equal(payload.Name.Trim(), talent.Name);
    Assert.Equal(payload.Summary.Value?.Trim(), talent.Summary);
    Assert.Equal(payload.Description.Value?.Trim(), talent.Description);
    Assert.Equal(payload.AllowMultiplePurchases, talent.AllowMultiplePurchases);
    Assert.Equal(payload.Skill.Value, talent.Skill);
    Assert.Equal(payload.RequiredTalentId.Value, talent.RequiredTalent?.Id);
  }
}
