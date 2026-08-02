using Krakenar.Contracts.Search;
using Logitar;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Builders;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Actors;
using SkillCraft.Api.Core.Educations;
using SkillCraft.Api.Core.Educations.Models;
using SkillCraft.Api.Core.Features;
using SkillCraft.Api.Core.Permissions;

namespace SkillCraft.Api.IntegrationTests.Educations;

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

    _education = new EducationBuilder(Faker).WithWorld(Context.World).Build();
    await _educationRepository.SaveAsync(_education);
  }

  [Theory(DisplayName = "It should create a new education.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_NotExist_When_CreateOrReplace_Then_Created(bool withId)
  {
    CreateOrReplaceEducationPayload payload = CreateJudicieuxPayload();
    Guid? id = withId ? Guid.NewGuid() : null;

    CreateOrReplaceEducationResult result = await _educationService.CreateOrReplaceAsync(payload, id);
    Assert.True(result.Created);
    EducationModel education = result.Education;
    Assert.NotNull(education);

    if (id.HasValue)
    {
      Assert.Equal(id.Value, education.Id);
    }
    else
    {
      Assert.NotEqual(Guid.Empty, education.Id);
    }
    Assert.Equal(3, education.Version);
    Assert.Equal(Actor, education.CreatedBy);
    Assert.Equal(DateTime.UtcNow, education.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(education.CreatedBy, education.UpdatedBy);
    Assert.True(education.CreatedOn < education.UpdatedOn);

    AssertJudicieux(payload, education);
  }

  [Fact(DisplayName = "It should filter search results by skill.")]
  public async Task Given_Skill_When_Search_Then_Results()
  {
    Education judicieux = EducationBuilder.Judicieux(Faker, Context.World);
    await _educationRepository.SaveAsync(judicieux);

    SearchEducationsPayload payload = new()
    {
      Skill = Skill.Orientation
    };

    SearchResults<EducationModel> results = await _educationService.SearchAsync(payload);
    Assert.Equal(1, results.Total);

    EducationModel education = Assert.Single(results.Items);
    Assert.Equal(judicieux.ResourceId, education.Id);
  }

  [Fact(DisplayName = "It should read an education by ID.")]
  public async Task Given_Id_When_Read_Then_Read()
  {
    EducationModel? education = await _educationService.ReadAsync(_education.ResourceId);
    Assert.NotNull(education);
    Assert.Equal(_education.ResourceId, education.Id);
  }

  [Fact(DisplayName = "It should replace an existing education.")]
  public async Task Given_Exists_When_CreateOrReplace_Then_Replaced()
  {
    CreateOrReplaceEducationPayload payload = CreateJudicieuxPayload();
    Guid id = _education.ResourceId;

    CreateOrReplaceEducationResult result = await _educationService.CreateOrReplaceAsync(payload, id);
    Assert.False(result.Created);
    EducationModel education = result.Education;
    Assert.NotNull(education);

    Assert.Equal(id, education.Id);
    Assert.Equal(4, education.Version);
    Assert.Equal(_education.CreatedBy, education.CreatedBy.GetActorId());
    Assert.Equal(_education.CreatedOn.AsUniversalTime(), education.CreatedOn, TimeSpan.FromMilliseconds(1));
    Assert.Equal(Actor, education.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, education.UpdatedOn, TimeSpan.FromSeconds(10));

    AssertJudicieux(payload, education);
  }

  [Fact(DisplayName = "It should return empty search results.")]
  public async Task Given_NoMatch_When_Search_Then_EmptyResults()
  {
    Context.World = new WorldBuilder(Faker).Build();

    SearchEducationsPayload payload = new();

    SearchResults<EducationModel> results = await _educationService.SearchAsync(payload);
    Assert.Equal(0, results.Total);
    Assert.Empty(results.Items);
  }

  [Fact(DisplayName = "It should return null when no education was found.")]
  public async Task Given_NotFound_When_Read_Then_NullReturned()
  {
    Context.World = new WorldBuilder(Faker).Build();

    Assert.Null(await _educationService.ReadAsync(_education.ResourceId));
  }

  [Fact(DisplayName = "It should return null when the education was not found.")]
  public async Task Given_NotFound_When_Update_Then_NullReturned()
  {
    Assert.Null(await _educationService.UpdateAsync(Guid.Empty, new UpdateEducationPayload()));
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task Given_Matches_When_Search_Then_Results()
  {
    Education classique = new EducationBuilder(Faker).WithWorld(Context.World).WithName("Classique").Build();
    Education judicieux = EducationBuilder.Judicieux(Faker, Context.World);
    Education rebelle = new EducationBuilder(Faker).WithWorld(Context.World).WithName("Rebelle").Build();
    await _educationRepository.SaveAsync([classique, judicieux, rebelle]);

    SearchEducationsPayload payload = new()
    {
      Skip = 1,
      Limit = 1
    };
    payload.Search.Terms.Add(new SearchTerm("%c%"));
    payload.Ids.AddRange([Guid.Empty, classique.ResourceId, judicieux.ResourceId, rebelle.ResourceId]);
    payload.Sort.Add(new EducationSortOption(EducationSort.Name));

    SearchResults<EducationModel> results = await _educationService.SearchAsync(payload);
    Assert.Equal(2, results.Total);

    EducationModel education = Assert.Single(results.Items);
    Assert.Equal(judicieux.ResourceId, education.Id);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when creating an education.")]
  public async Task Given_NotAllowed_When_Create_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder(Faker).Build();

    CreateOrReplaceEducationPayload payload = CreateJudicieuxPayload();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _educationService.CreateOrReplaceAsync(payload));
    Assert.Equal(Context.ActorId?.Value, exception.Principal);
    Assert.Equal(Actions.CreateEducation, exception.Action);
    Assert.Null(exception.Resource);
    Assert.Equal(Context.WorldUid, exception.WorldId);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when replacing an education.")]
  public async Task Given_NotAllowed_When_Replace_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder(Faker).Build();

    CreateOrReplaceEducationPayload payload = CreateJudicieuxPayload();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _educationService.CreateOrReplaceAsync(payload, _education.ResourceId));
    Assert.Equal(Context.ActorId?.Value, exception.Principal);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(_education.Identifier.ToString(), exception.Resource);
    Assert.Equal(Context.WorldUid, exception.WorldId);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when updating an education.")]
  public async Task Given_NotAllowed_When_Update_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder(Faker).Build();

    UpdateEducationPayload payload = new();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _educationService.UpdateAsync(_education.ResourceId, payload));
    Assert.Equal(Context.ActorId?.Value, exception.Principal);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(_education.Identifier.ToString(), exception.Resource);
    Assert.Equal(Context.WorldUid, exception.WorldId);
  }

  [Fact(DisplayName = "It should update an existing education.")]
  public async Task Given_Exists_When_Update_Then_Updated()
  {
    Guid id = _education.ResourceId;
    CreateOrReplaceEducationPayload create = CreateJudicieuxPayload();
    UpdateEducationPayload payload = new()
    {
      Name = create.Name,
      Summary = new Optional<string>(create.Summary),
      Content = new Optional<string>(create.Content),
      Skill = new Optional<Skill?>(create.Skill),
      WealthMultiplier = new Optional<int?>(create.WealthMultiplier),
      Feature = new Optional<FeatureModel>(create.Feature)
    };

    EducationModel? education = await _educationService.UpdateAsync(id, payload);
    Assert.NotNull(education);

    Assert.Equal(id, education.Id);
    Assert.Equal(4, education.Version);
    Assert.Equal(_education.CreatedBy, education.CreatedBy.GetActorId());
    Assert.Equal(_education.CreatedOn.AsUniversalTime(), education.CreatedOn, TimeSpan.FromMilliseconds(1));
    Assert.Equal(Actor, education.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, education.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Equal(payload.Name.CleanTrim(), education.Name);
    Assert.Equal(payload.Summary.Value?.CleanTrim(), education.Summary);
    Assert.Equal(payload.Content.Value?.CleanTrim(), education.Content);
    Assert.Equal(payload.Skill.Value, education.Skill);
    Assert.Equal(payload.WealthMultiplier.Value, education.WealthMultiplier);
    Assert.NotNull(education.Feature);
    Assert.Equal(payload.Feature.Value!.Name.Trim(), education.Feature.Name);
    Assert.Equal(payload.Feature.Value.Content?.CleanTrim(), education.Feature.Content);
  }

  private static CreateOrReplaceEducationPayload CreateJudicieuxPayload() => new()
  {
    Name = " Judicieux ",
    Summary = "  Esprit pos� et analytique, pr�t � guider par des d�cisions avis�es.  ",
    Content = "   Peu importe le mode de vie dans lequel il a �t� �lev�, le personnage prend des d�cisions sens�es et �clair�es au moment opportun.\n\nIl saisit les opportunit�s et on lui demande souvent conseil.\n\nIl sait mettre en ex�cution des plans complexes et trier les informations pertinentes.   ",
    Skill = Skill.Orientation,
    WealthMultiplier = 10,
    Feature = new FeatureModel(
      " Conseiller avis� ",
      "   La nature calme et analytique du personnage lui permet d��tre reconnu rapidement pour son jugement s�r.\n\nIl peut ajouter un bonus �gal � son [tiers](/regles/personnages/progression/tiers) (minimum 1) � ses [tests](/regles/competences/tests) d�[Intuition](/regles/competences/intuition) ou d�[Investigation](/regles/competences/investigation) effectu�s afin de comprendre un plan, �valuer un risque ou choisir la meilleure approche de mani�re objective.\n\n�galement, il ajoute �galement ce bonus � ses tests de [Diplomatie](/regles/competences/diplomatie) effectu�s afin de convaincre un individue rationnel.   ")
  };

  private static void AssertJudicieux(CreateOrReplaceEducationPayload payload, EducationModel education)
  {
    Assert.Equal(payload.Name.CleanTrim(), education.Name);
    Assert.Equal(payload.Summary?.CleanTrim(), education.Summary);
    Assert.Equal(payload.Content?.CleanTrim(), education.Content);
    Assert.Equal(payload.Skill, education.Skill);
    Assert.Equal(payload.WealthMultiplier, education.WealthMultiplier);
    Assert.NotNull(education.Feature);
    Assert.Equal(payload.Feature!.Name.Trim(), education.Feature.Name);
    Assert.Equal(payload.Feature.Content?.CleanTrim(), education.Feature.Content);
  }
}
