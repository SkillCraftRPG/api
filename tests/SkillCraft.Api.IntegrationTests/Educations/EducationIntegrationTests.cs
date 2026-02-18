using Krakenar.Contracts.Search;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Contracts;
using SkillCraft.Api.Contracts.Educations;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Educations;

namespace SkillCraft.Api.Educations;

[Trait(Traits.Category, Categories.Integration)]
public class EducationIntegrationTests : IntegrationTests
{
  private readonly IEducationRepository _educationRepository;
  private readonly IEducationService _educationService;

  private Education _education = null!;

  public EducationIntegrationTests() : base()
  {
    _educationRepository = ServiceProvider.GetRequiredService<IEducationRepository>();
    _educationService = ServiceProvider.GetRequiredService<IEducationService>();
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    _education = new Education(World, new Name("Education"), UserId);
    await _educationRepository.SaveAsync(_education);
  }

  [Theory(DisplayName = "It should create a new education.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_NotExist_When_CreateOrReplace_Then_Created(bool withId)
  {
    CreateOrReplaceEducationPayload payload = new()
    {
      Name = "  Classique  ",
      Summary = "  Éducation érudite, savoir étendu et réseau d’influence académique.  ",
      Description = "  Peu peuvent se vanter d’avoir reçu une éducation traditionnelle comme celle du personnage.\n\nIl a suivi un parcours scolaire conforme et sans dérogation ayant mené à une instruction de haute qualité.\n\nMalgré son manque d’expériences personnelles, son grand savoir lui permet de se débrouiller même dans les situations les plus difficiles.  ",
      Skill = GameSkill.Knowledge,
      WealthMultiplier = 12,
      Feature = new FeatureModel("Alumni", "Grâce à son parcours académique, le personnage entretient un réseau de contacts issus de divers milieux.\n\nIl peut mobiliser ces relations pour obtenir des informations, un service, une recommandation ou un appui ponctuel, selon la nature de sa formation et les circonstances.\n\nCes connaissances peuvent lui apporter un avantage social, une ressource rare ou un contact utile à la mission.")
    };
    Guid? id = withId ? Guid.NewGuid() : null;

    CreateOrReplaceEducationResult result = await _educationService.CreateOrReplaceAsync(payload, id);
    Assert.True(result.Created);
    EducationModel education = result.Education;

    if (id.HasValue)
    {
      Assert.Equal(id.Value, education.Id);
    }
    Assert.Equal(2, education.Version);
    Assert.Equal(Actor, education.CreatedBy);
    Assert.Equal(DateTime.UtcNow, education.CreatedOn, TimeSpan.FromSeconds(1));
    Assert.Equal(Actor, education.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, education.UpdatedOn, TimeSpan.FromSeconds(1));

    Assert.Equal(payload.Name.Trim(), education.Name);
    Assert.Equal(payload.Summary.Trim(), education.Summary);
    Assert.Equal(payload.Description.Trim(), education.Description);
    Assert.Equal(payload.Skill, education.Skill);
    Assert.Equal(payload.WealthMultiplier, education.WealthMultiplier);
    Assert.Equal(payload.Feature, education.Feature);
  }

  [Fact(DisplayName = "It should delete an existing education.")]
  public async Task Given_Exists_When_Delete_Then_Deleted()
  {
    Guid id = _education.EntityId;
    EducationModel? education = await _educationService.DeleteAsync(id);
    Assert.NotNull(education);
    Assert.Equal(id, education.Id);
  }

  [Fact(DisplayName = "It should read an existing education.")]
  public async Task Given_Exists_When_Read_Then_Returned()
  {
    Guid id = _education.EntityId;
    EducationModel? education = await _educationService.ReadAsync(id);
    Assert.NotNull(education);
    Assert.Equal(id, education.Id);
  }

  [Fact(DisplayName = "It should replace an existing education.")]
  public async Task Given_Exists_When_CreateOrReplace_Then_Replaced()
  {
    CreateOrReplaceEducationPayload payload = new()
    {
      Name = "  Classique  ",
      Summary = "  Éducation érudite, savoir étendu et réseau d’influence académique.  ",
      Description = "  Peu peuvent se vanter d’avoir reçu une éducation traditionnelle comme celle du personnage.\n\nIl a suivi un parcours scolaire conforme et sans dérogation ayant mené à une instruction de haute qualité.\n\nMalgré son manque d’expériences personnelles, son grand savoir lui permet de se débrouiller même dans les situations les plus difficiles.  ",
      Skill = GameSkill.Knowledge,
      WealthMultiplier = 12,
      Feature = new FeatureModel("Alumni", "Grâce à son parcours académique, le personnage entretient un réseau de contacts issus de divers milieux.\n\nIl peut mobiliser ces relations pour obtenir des informations, un service, une recommandation ou un appui ponctuel, selon la nature de sa formation et les circonstances.\n\nCes connaissances peuvent lui apporter un avantage social, une ressource rare ou un contact utile à la mission.")
    };
    Guid id = _education.EntityId;

    CreateOrReplaceEducationResult result = await _educationService.CreateOrReplaceAsync(payload, id);
    Assert.False(result.Created);
    EducationModel education = result.Education;

    Assert.Equal(id, education.Id);
    Assert.Equal(2, education.Version);
    Assert.Equal(Actor, education.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, education.UpdatedOn, TimeSpan.FromSeconds(1));

    Assert.Equal(payload.Name.Trim(), education.Name);
    Assert.Equal(payload.Summary.Trim(), education.Summary);
    Assert.Equal(payload.Description.Trim(), education.Description);
    Assert.Equal(payload.Skill, education.Skill);
    Assert.Equal(payload.WealthMultiplier, education.WealthMultiplier);
    Assert.Equal(payload.Feature, education.Feature);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task Given_Payload_When_Search_Then_Results()
  {
    Education classique = new(World, new Name("Classique"), UserId);
    classique.Skill = GameSkill.Knowledge;
    classique.Update(UserId);
    Education dansLaRue = new(World, new Name("Dans la rue"), UserId);
    dansLaRue.Skill = GameSkill.Knowledge;
    dansLaRue.Update(UserId);
    Education judicieux = new(World, new Name("Judicieux"), UserId);
    judicieux.Skill = GameSkill.Knowledge;
    judicieux.Feature = new Feature(new Name("Conseiller avisé"), Description: null);
    judicieux.Update(UserId);
    Education rebelle = new(World, new Name("Rebelle"), UserId);
    rebelle.Skill = GameSkill.Knowledge;
    rebelle.Summary = new Summary("Esprit frondeur, expert en mensonge et en diversion subtile.");
    rebelle.Update(UserId);
    await _educationRepository.SaveAsync([classique, dansLaRue, judicieux, rebelle]);

    SearchEducationsPayload payload = new()
    {
      Skill = GameSkill.Knowledge.ToString(),
      Skip = 1,
      Limit = 1
    };
    payload.Ids.AddRange([_education.EntityId, classique.EntityId, judicieux.EntityId, rebelle.EntityId, Guid.Empty]);
    payload.Search.Operator = SearchOperator.Or;
    payload.Search.Terms.AddRange([new SearchTerm("education"), new SearchTerm("%rue"), new SearchTerm("cons%"), new SearchTerm("%mensonge%")]);
    payload.Sort.Add(new EducationSortOption(EducationSort.Name));

    SearchResults<EducationModel> results = await _educationService.SearchAsync(payload);
    Assert.Equal(2, results.Total);

    EducationModel result = Assert.Single(results.Items);
    Assert.Equal(rebelle.EntityId, result.Id);
  }

  [Fact(DisplayName = "It should update an existing education.")]
  public async Task Given_Exists_When_Update_ThenUpdated()
  {
    Guid id = _education.EntityId;
    UpdateEducationPayload payload = new()
    {
      Name = "  Classique  ",
      Summary = new Update<string>("  Éducation érudite, savoir étendu et réseau d’influence académique.  "),
      Description = new Update<string>("  Peu peuvent se vanter d’avoir reçu une éducation traditionnelle comme celle du personnage.\n\nIl a suivi un parcours scolaire conforme et sans dérogation ayant mené à une instruction de haute qualité.\n\nMalgré son manque d’expériences personnelles, son grand savoir lui permet de se débrouiller même dans les situations les plus difficiles.  "),
      Skill = new Update<GameSkill?>(GameSkill.Knowledge),
      WealthMultiplier = new Update<int?>(12),
      Feature = new Update<FeatureModel>(new FeatureModel("Alumni", "Grâce à son parcours académique, le personnage entretient un réseau de contacts issus de divers milieux.\n\nIl peut mobiliser ces relations pour obtenir des informations, un service, une recommandation ou un appui ponctuel, selon la nature de sa formation et les circonstances.\n\nCes connaissances peuvent lui apporter un avantage social, une ressource rare ou un contact utile à la mission."))
    };

    EducationModel? education = await _educationService.UpdateAsync(id, payload);
    Assert.NotNull(education);

    Assert.Equal(id, education.Id);
    Assert.Equal(2, education.Version);
    Assert.Equal(Actor, education.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, education.UpdatedOn, TimeSpan.FromSeconds(1));

    Assert.Equal(payload.Name.Trim(), education.Name);
    Assert.Equal(payload.Summary.Value?.Trim(), education.Summary);
    Assert.Equal(payload.Description.Value?.Trim(), education.Description);
    Assert.Equal(payload.Skill.Value, education.Skill);
    Assert.Equal(payload.WealthMultiplier.Value, education.WealthMultiplier);
    Assert.Equal(payload.Feature.Value, education.Feature);
  }
}
