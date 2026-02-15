using Krakenar.Contracts.Search;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Contracts;
using SkillCraft.Api.Contracts.Castes;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Castes;

namespace SkillCraft.Api.Castes;

[Trait(Traits.Category, Categories.Integration)]
public class CasteIntegrationTests : IntegrationTests
{
  private readonly ICasteRepository _casteRepository;
  private readonly ICasteService _casteService;

  private Caste _caste = null!;

  public CasteIntegrationTests() : base()
  {
    _casteRepository = ServiceProvider.GetRequiredService<ICasteRepository>();
    _casteService = ServiceProvider.GetRequiredService<ICasteService>();
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    _caste = new Caste(World, new Name("Caste"), UserId);
    await _casteRepository.SaveAsync(_caste);
  }

  [Theory(DisplayName = "It should create a new caste.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_NotExist_When_CreateOrReplace_Then_Created(bool withId)
  {
    CreateOrReplaceCastePayload payload = new()
    {
      Name = "  Amuseur  ",
      Summary = "  Artiste itinérant gagnant prestige et faveurs par ses spectacles.  ",
      Description = "  Adepte d’art, de poésie, de cirque ou de théâtre, l’amuseur ère de villages en cités afin d’offrir ses performances.\n\nBien plus qu’un simple gagne-pain, cela lui permet de former un réseau de contacts et de recueillir toutes sortes d’informations.  ",
      Skill = GameSkill.Performance,
      WealthRoll = "8d6",
      Feature = new FeatureModel("Charme du spectacle", "Le personnage trouve toujours un endroit pour se présenter en spectacle, habituellement dans [une taverne ou une auberge](/regles/equipement/services/peuplements), mais possiblement auprès d’une troupe de cirque, d’un groupe de comédiens, ou à la cour de nobles.\n\nIl peut demander [le gite ou le couvert](/regles/equipement/services/gite-couvert) en échange de divertissement.\n\nÉgalement, lorsqu’il se donne en spectacle, sa [notoriété](/regles/equipement/depenses) augmente localement, ce qui peut lui permettre d’obtenir certaines faveurs, comme une audience auprès d’un noble ou des privilèges réservés aux personnalités populaires.")
    };
    Guid? id = withId ? Guid.NewGuid() : null;

    CreateOrReplaceCasteResult result = await _casteService.CreateOrReplaceAsync(payload, id);
    Assert.True(result.Created);
    CasteModel caste = result.Caste;

    if (id.HasValue)
    {
      Assert.Equal(id.Value, caste.Id);
    }
    Assert.Equal(2, caste.Version);
    Assert.Equal(Actor, caste.CreatedBy);
    Assert.Equal(DateTime.UtcNow, caste.CreatedOn, TimeSpan.FromSeconds(1));
    Assert.Equal(Actor, caste.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, caste.UpdatedOn, TimeSpan.FromSeconds(1));

    Assert.Equal(payload.Name.Trim(), caste.Name);
    Assert.Equal(payload.Summary.Trim(), caste.Summary);
    Assert.Equal(payload.Description.Trim(), caste.Description);
    Assert.Equal(payload.Skill, caste.Skill);
    Assert.Equal(payload.WealthRoll, caste.WealthRoll);
    Assert.Equal(payload.Feature, caste.Feature);
  }

  [Fact(DisplayName = "It should delete an existing caste.")]
  public async Task Given_Exists_When_Delete_ThenDeleted()
  {
    Guid id = _caste.EntityId;
    CasteModel? caste = await _casteService.DeleteAsync(id);
    Assert.NotNull(caste);
    Assert.Equal(id, caste.Id);
  }

  [Fact(DisplayName = "It should read an existing caste.")]
  public async Task Given_Exists_When_Read_Then_Returned()
  {
    Guid id = _caste.EntityId;
    CasteModel? caste = await _casteService.ReadAsync(id);
    Assert.NotNull(caste);
    Assert.Equal(id, caste.Id);
  }

  [Fact(DisplayName = "It should replace an existing caste.")]
  public async Task Given_Exists_When_CreateOrReplace_Then_Replaced()
  {
    CreateOrReplaceCastePayload payload = new()
    {
      Name = "  Amuseur  ",
      Summary = "  Artiste itinérant gagnant prestige et faveurs par ses spectacles.  ",
      Description = "  Adepte d’art, de poésie, de cirque ou de théâtre, l’amuseur ère de villages en cités afin d’offrir ses performances.\n\nBien plus qu’un simple gagne-pain, cela lui permet de former un réseau de contacts et de recueillir toutes sortes d’informations.  ",
      Skill = GameSkill.Performance,
      WealthRoll = "8d6",
      Feature = new FeatureModel("Charme du spectacle", "Le personnage trouve toujours un endroit pour se présenter en spectacle, habituellement dans [une taverne ou une auberge](/regles/equipement/services/peuplements), mais possiblement auprès d’une troupe de cirque, d’un groupe de comédiens, ou à la cour de nobles.\n\nIl peut demander [le gite ou le couvert](/regles/equipement/services/gite-couvert) en échange de divertissement.\n\nÉgalement, lorsqu’il se donne en spectacle, sa [notoriété](/regles/equipement/depenses) augmente localement, ce qui peut lui permettre d’obtenir certaines faveurs, comme une audience auprès d’un noble ou des privilèges réservés aux personnalités populaires.")
    };
    Guid id = _caste.EntityId;

    CreateOrReplaceCasteResult result = await _casteService.CreateOrReplaceAsync(payload, id);
    Assert.False(result.Created);
    CasteModel caste = result.Caste;

    Assert.Equal(id, caste.Id);
    Assert.Equal(2, caste.Version);
    Assert.Equal(Actor, caste.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, caste.UpdatedOn, TimeSpan.FromSeconds(1));

    Assert.Equal(payload.Name.Trim(), caste.Name);
    Assert.Equal(payload.Summary.Trim(), caste.Summary);
    Assert.Equal(payload.Description.Trim(), caste.Description);
    Assert.Equal(payload.Skill, caste.Skill);
    Assert.Equal(payload.WealthRoll, caste.WealthRoll);
    Assert.Equal(payload.Feature, caste.Feature);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task Given_Payload_When_Search_Then_Results()
  {
    Caste amuseur = new(World, new Name("Amuseur"), UserId);
    amuseur.Skill = GameSkill.Performance;
    amuseur.Update(UserId);
    Caste boheme = new(World, new Name("Bohème"), UserId);
    boheme.Skill = GameSkill.Performance;
    boheme.Update(UserId);
    Caste marchand = new(World, new Name("Marchand"), UserId);
    marchand.Skill = GameSkill.Performance;
    marchand.Feature = new Feature(new Name("Négociateur"), Description: null);
    marchand.Update(UserId);
    Caste paysan = new(World, new Name("Paysan"), UserId);
    paysan.Skill = GameSkill.Performance;
    paysan.Summary = new Summary("Travailleurs endurants, humbles et solidaires, maîtres du labeur.");
    paysan.Update(UserId);
    await _casteRepository.SaveAsync([amuseur, boheme, marchand, paysan]);

    SearchCastesPayload payload = new()
    {
      Skill = GameSkill.Performance,
      Skip = 1,
      Limit = 1
    };
    payload.Ids.AddRange([_caste.EntityId, amuseur.EntityId, marchand.EntityId, paysan.EntityId, Guid.Empty]);
    payload.Search.Operator = SearchOperator.Or;
    payload.Search.Terms.AddRange([new SearchTerm("caste"), new SearchTerm("bo%"), new SearchTerm("%égo%"), new SearchTerm("%labeur.")]);
    payload.Sort.Add(new CasteSortOption(CasteSort.Name));

    SearchResults<CasteModel> results = await _casteService.SearchAsync(payload);
    Assert.Equal(2, results.Total);

    CasteModel result = Assert.Single(results.Items);
    Assert.Equal(paysan.EntityId, result.Id);
  }

  [Fact(DisplayName = "It should update an existing caste.")]
  public async Task Given_Exists_When_Update_ThenUpdated()
  {
    Guid id = _caste.EntityId;
    UpdateCastePayload payload = new()
    {
      Name = "  Amuseur  ",
      Summary = new Update<string>("  Artiste itinérant gagnant prestige et faveurs par ses spectacles.  "),
      Description = new Update<string>("  Adepte d’art, de poésie, de cirque ou de théâtre, l’amuseur ère de villages en cités afin d’offrir ses performances.\n\nBien plus qu’un simple gagne-pain, cela lui permet de former un réseau de contacts et de recueillir toutes sortes d’informations.  "),
      Skill = new Update<GameSkill?>(GameSkill.Performance),
      WealthRoll = new Update<string>("8d6"),
      Feature = new Update<FeatureModel>(new FeatureModel("Charme du spectacle", "Le personnage trouve toujours un endroit pour se présenter en spectacle, habituellement dans [une taverne ou une auberge](/regles/equipement/services/peuplements), mais possiblement auprès d’une troupe de cirque, d’un groupe de comédiens, ou à la cour de nobles.\n\nIl peut demander [le gite ou le couvert](/regles/equipement/services/gite-couvert) en échange de divertissement.\n\nÉgalement, lorsqu’il se donne en spectacle, sa [notoriété](/regles/equipement/depenses) augmente localement, ce qui peut lui permettre d’obtenir certaines faveurs, comme une audience auprès d’un noble ou des privilèges réservés aux personnalités populaires."))
    };

    CasteModel? caste = await _casteService.UpdateAsync(id, payload);
    Assert.NotNull(caste);

    Assert.Equal(id, caste.Id);
    Assert.Equal(2, caste.Version);
    Assert.Equal(Actor, caste.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, caste.UpdatedOn, TimeSpan.FromSeconds(1));

    Assert.Equal(payload.Name.Trim(), caste.Name);
    Assert.Equal(payload.Summary.Value?.Trim(), caste.Summary);
    Assert.Equal(payload.Description.Value?.Trim(), caste.Description);
    Assert.Equal(payload.Skill.Value, caste.Skill);
    Assert.Equal(payload.WealthRoll.Value, caste.WealthRoll);
    Assert.Equal(payload.Feature.Value, caste.Feature);
  }
}
