using Krakenar.Contracts.Search;
using Logitar;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Builders;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Actors;
using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Languages.Models;
using SkillCraft.Api.Core.Lineages;
using SkillCraft.Api.Core.Lineages.Models;
using SkillCraft.Api.Core.Permissions;

namespace SkillCraft.Api.IntegrationTests.Lineages;

[Trait(Traits.Category, Categories.Integration)]
public class LineageIntegrationTests : IntegrationTests
{
  private readonly ILanguageRepository _languageRepository;
  private readonly ILineageRepository _lineageRepository;
  private readonly ILineageService _lineageService;

  private Language _celfique = null!;
  private Lineage _elfe = null!;
  private Lineage _lineage = null!;

  public LineageIntegrationTests() : base()
  {
    _languageRepository = ServiceProvider.GetRequiredService<ILanguageRepository>();
    _lineageRepository = ServiceProvider.GetRequiredService<ILineageRepository>();
    _lineageService = ServiceProvider.GetRequiredService<ILineageService>();
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    _celfique = LanguageBuilder.Celfique(Faker, Context.World);
    await _languageRepository.SaveAsync(_celfique);

    _elfe = LineageBuilder.Elfe(Faker, Context.World);
    await _lineageRepository.SaveAsync(_elfe);

    _lineage = new LineageBuilder(Faker).WithWorld(Context.World).Build();
    await _lineageRepository.SaveAsync(_lineage);
  }

  [Theory(DisplayName = "It should create a new lineage.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_NotExist_When_CreateOrReplace_Then_Created(bool withId)
  {
    CreateOrReplaceLineagePayload payload = CreateHautElfePayload();
    Guid? id = withId ? Guid.NewGuid() : null;

    CreateOrReplaceLineageResult result = await _lineageService.CreateOrReplaceAsync(payload, id);
    Assert.True(result.Created);
    LineageModel lineage = result.Lineage;
    Assert.NotNull(lineage);

    if (id.HasValue)
    {
      Assert.Equal(id.Value, lineage.Id);
    }
    else
    {
      Assert.NotEqual(Guid.Empty, lineage.Id);
    }
    Assert.Equal(4, lineage.Version);
    Assert.Equal(Actor, lineage.CreatedBy);
    Assert.Equal(DateTime.UtcNow, lineage.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(lineage.CreatedBy, lineage.UpdatedBy);
    Assert.True(lineage.CreatedOn < lineage.UpdatedOn);

    AssertHautElfe(payload, lineage);
  }

  [Fact(DisplayName = "It should filter search results by parent ID.")]
  public async Task Given_ParentId_When_Search_Then_Results()
  {
    Lineage hautElfe = LineageBuilder.HautElfe(Faker, Context.World, _elfe, _celfique);
    await _lineageRepository.SaveAsync(hautElfe);

    SearchLineagesPayload payload = new()
    {
      ParentId = _elfe.ResourceId
    };

    SearchResults<LineageModel> results = await _lineageService.SearchAsync(payload);
    Assert.Equal(1, results.Total);

    LineageModel lineage = Assert.Single(results.Items);
    Assert.Equal(hautElfe.ResourceId, lineage.Id);
  }

  [Fact(DisplayName = "It should filter search results by size category.")]
  public async Task Given_SizeCategory_When_Search_Then_Results()
  {
    Lineage nain = LineageBuilder.Nain(Faker, Context.World);
    Lineage petit = new LineageBuilder(Faker).WithWorld(Context.World).WithName("Gnome").WithSize(SizeCategory.Small, "90+2d10").Build();
    await _lineageRepository.SaveAsync([nain, petit]);

    SearchLineagesPayload payload = new()
    {
      SizeCategory = SizeCategory.Small
    };

    SearchResults<LineageModel> results = await _lineageService.SearchAsync(payload);
    Assert.Equal(1, results.Total);

    LineageModel lineage = Assert.Single(results.Items);
    Assert.Equal(petit.ResourceId, lineage.Id);
  }

  [Fact(DisplayName = "It should read a lineage by ID.")]
  public async Task Given_Id_When_Read_Then_Read()
  {
    LineageModel? lineage = await _lineageService.ReadAsync(_lineage.ResourceId);
    Assert.NotNull(lineage);
    Assert.Equal(_lineage.ResourceId, lineage.Id);
  }

  [Fact(DisplayName = "It should replace an existing lineage.")]
  public async Task Given_Exists_When_CreateOrReplace_Then_Replaced()
  {
    CreateOrReplaceLineagePayload payload = CreateHumainPayload();
    Guid id = _lineage.ResourceId;

    CreateOrReplaceLineageResult result = await _lineageService.CreateOrReplaceAsync(payload, id);
    Assert.False(result.Created);
    LineageModel lineage = result.Lineage;
    Assert.NotNull(lineage);

    Assert.Equal(id, lineage.Id);
    Assert.Equal(7, lineage.Version);
    Assert.Equal(_lineage.CreatedBy, lineage.CreatedBy.GetActorId());
    Assert.Equal(_lineage.CreatedOn.AsUniversalTime(), lineage.CreatedOn, TimeSpan.FromMilliseconds(1));
    Assert.Equal(Actor, lineage.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, lineage.UpdatedOn, TimeSpan.FromSeconds(10));

    AssertHumain(payload, lineage);
  }

  [Fact(DisplayName = "It should return empty search results.")]
  public async Task Given_NoMatch_When_Search_Then_EmptyResults()
  {
    Context.World = new WorldBuilder(Faker).Build();

    SearchLineagesPayload payload = new();

    SearchResults<LineageModel> results = await _lineageService.SearchAsync(payload);
    Assert.Equal(0, results.Total);
    Assert.Empty(results.Items);
  }

  [Fact(DisplayName = "It should return null when no lineage was found.")]
  public async Task Given_NotFound_When_Read_Then_NullReturned()
  {
    Context.World = new WorldBuilder(Faker).Build();

    Assert.Null(await _lineageService.ReadAsync(_lineage.ResourceId));
  }

  [Fact(DisplayName = "It should return null when the language was not found.")]
  public async Task Given_NotFound_When_Update_Then_NullReturned()
  {
    Assert.Null(await _lineageService.UpdateAsync(Guid.Empty, new UpdateLineagePayload()));
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task Given_Matches_When_Search_Then_Results()
  {
    Lineage humain = LineageBuilder.Humain(Faker, Context.World);
    Lineage nain = LineageBuilder.Nain(Faker, Context.World);
    await _lineageRepository.SaveAsync([humain, nain]);

    SearchLineagesPayload payload = new()
    {
      Skip = 1,
      Limit = 1
    };
    payload.Search.Operator = SearchOperator.Or;
    payload.Search.Terms.Add(new SearchTerm("%humain%"));
    payload.Search.Terms.Add(new SearchTerm("%nain%"));
    payload.Search.Terms.Add(new SearchTerm("%elfe%"));
    payload.Ids.AddRange([_lineage.ResourceId, Guid.Empty, humain.ResourceId, nain.ResourceId]);
    payload.Sort.Add(new LineageSortOption(LineageSort.Name, isDescending: true));

    SearchResults<LineageModel> results = await _lineageService.SearchAsync(payload);
    Assert.Equal(2, results.Total);

    LineageModel lineage = Assert.Single(results.Items);
    Assert.Equal(humain.ResourceId, lineage.Id);
  }

  [Fact(DisplayName = "It should throw InvalidParentLineageException when the parent has a parent.")]
  public async Task Given_ParentHasParent_When_Create_Then_InvalidParentLineageException()
  {
    Lineage hautElfe = LineageBuilder.HautElfe(Faker, Context.World, _elfe, _celfique);
    await _lineageRepository.SaveAsync(hautElfe);

    CreateOrReplaceLineagePayload payload = CreateHautElfePayload();
    payload.ParentId = hautElfe.ResourceId;
    payload.Name = " Sous-ethnie ";

    var exception = await Assert.ThrowsAsync<InvalidParentLineageException>(async () => await _lineageService.CreateOrReplaceAsync(payload));
    Assert.Equal(Context.WorldUid, exception.WorldId);
    Assert.Equal(hautElfe.ResourceId, exception.ParentId);
    Assert.Equal("ParentId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw LanguagesNotFoundException when creating a lineage.")]
  public async Task Given_LanguagesNotFound_When_Create_Then_LanguagesNotFoundException()
  {
    CreateOrReplaceLineagePayload payload = CreateHautElfePayload();
    payload.Languages.Ids = [Guid.Empty];

    var exception = await Assert.ThrowsAsync<LanguagesNotFoundException>(async () => await _lineageService.CreateOrReplaceAsync(payload));
    Assert.Equal(Context.WorldUid, exception.WorldId);
    Assert.Contains(Guid.Empty, exception.LanguageIds);
    Assert.Equal("Languages.Ids", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw LanguagesNotFoundException when replacing a lineage.")]
  public async Task Given_LanguagesNotFound_When_Replace_Then_LanguagesNotFoundException()
  {
    CreateOrReplaceLineagePayload payload = CreateHumainPayload();
    payload.Languages.Ids = [Guid.Empty];

    var exception = await Assert.ThrowsAsync<LanguagesNotFoundException>(async () => await _lineageService.CreateOrReplaceAsync(payload, _lineage.ResourceId));
    Assert.Equal(Context.WorldUid, exception.WorldId);
    Assert.Contains(Guid.Empty, exception.LanguageIds);
    Assert.Equal("Languages.Ids", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw LanguagesNotFoundException when updating a lineage.")]
  public async Task Given_LanguagesNotFound_When_Update_Then_LanguagesNotFoundException()
  {
    UpdateLineagePayload payload = new()
    {
      Languages = new LineageLanguagesPayload
      {
        Ids = [Guid.Empty],
        Extra = 1
      }
    };

    var exception = await Assert.ThrowsAsync<LanguagesNotFoundException>(async () => await _lineageService.UpdateAsync(_lineage.ResourceId, payload));
    Assert.Equal(Context.WorldUid, exception.WorldId);
    Assert.Contains(Guid.Empty, exception.LanguageIds);
    Assert.Equal("Languages.Ids", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw LineageNotFoundException when creating a lineage.")]
  public async Task Given_ParentNotFound_When_Create_Then_LineageNotFoundException()
  {
    CreateOrReplaceLineagePayload payload = CreateHautElfePayload();
    payload.ParentId = Guid.Empty;

    var exception = await Assert.ThrowsAsync<LineageNotFoundException>(async () => await _lineageService.CreateOrReplaceAsync(payload));
    Assert.Equal(Context.WorldUid, exception.WorldId);
    Assert.Equal(payload.ParentId.Value, exception.LineageId);
    Assert.Equal("ParentId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when creating a lineage.")]
  public async Task Given_NotAllowed_When_Create_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder(Faker).Build();

    CreateOrReplaceLineagePayload payload = CreateHautElfePayload();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _lineageService.CreateOrReplaceAsync(payload));
    Assert.Equal(Context.UserUid, exception.UserId);
    Assert.Equal(Actions.CreateLineage, exception.Action);
    Assert.Equal(Context.World?.Identifier.ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when replacing a lineage.")]
  public async Task Given_NotAllowed_When_Replace_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder(Faker).Build();

    CreateOrReplaceLineagePayload payload = CreateHumainPayload();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _lineageService.CreateOrReplaceAsync(payload, _lineage.ResourceId));
    Assert.Equal(Context.UserUid, exception.UserId);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(_lineage.Identifier.ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when updating a lineage.")]
  public async Task Given_NotAllowed_When_Update_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder(Faker).Build();

    UpdateLineagePayload payload = new();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _lineageService.UpdateAsync(_lineage.ResourceId, payload));
    Assert.Equal(Context.UserUid, exception.UserId);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(_lineage.Identifier.ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should update an existing lineage.")]
  public async Task Given_Exists_When_Update_Then_Updated()
  {
    Guid id = _lineage.ResourceId;
    CreateOrReplaceLineagePayload create = CreateHumainPayload();
    UpdateLineagePayload payload = new()
    {
      Name = create.Name,
      Summary = new Optional<string>(create.Summary),
      Content = new Optional<string>(create.Content),
      Languages = create.Languages,
      Names = create.Names,
      Speeds = create.Speeds,
      Size = create.Size,
      Weight = create.Weight,
      Age = create.Age
    };

    LineageModel? lineage = await _lineageService.UpdateAsync(id, payload);
    Assert.NotNull(lineage);

    Assert.Equal(id, lineage.Id);
    Assert.Equal(7, lineage.Version);
    Assert.Equal(_lineage.CreatedBy, lineage.CreatedBy.GetActorId());
    Assert.Equal(_lineage.CreatedOn.AsUniversalTime(), lineage.CreatedOn, TimeSpan.FromMilliseconds(1));
    Assert.Equal(Actor, lineage.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, lineage.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Equal(payload.Name.CleanTrim(), lineage.Name);
    Assert.Equal(payload.Summary.Value?.CleanTrim(), lineage.Summary);
    Assert.Equal(payload.Content.Value?.CleanTrim(), lineage.Content);
    AssertHumain(create, lineage);
  }

  private CreateOrReplaceLineagePayload CreateHautElfePayload() => new()
  {
    ParentId = _elfe.ResourceId,
    Name = " Haut-Elfe ",
    Summary = "  Héritiers stellaires de royaumes elfiques raffinés et érudits.  ",
    Content = "   Les Hauts-Elfes privilégient l’ordre, l’érudition et la stabilité, qu’ils considèrent comme les fondations de toute civilisation durable. Héritiers d’un antique royaume forestier fondé dans l’Ouest de la Sarénie il y a plus de deux millénaires, ils ont développé une culture raffinée où astrologie, magie et savoir occupent une place centrale. Leur expansion vers les Triskîles mena à la fondation de royaumes sur Alnar et Ellesdales, bien que cette dernière région se soit fragmentée au fil des siècles en principautés rivales. Diplomates et marchands influents, les Hauts-Elfes entretiennent généralement de bonnes relations avec les peuples civilisés, mais les ambitions territoriales des Dallois menacent désormais l’équilibre fragile d’Ellesdales. Nés sous des constellations considérées sacrées, ils portent souvent sur eux le symbole de l’étoile ayant marqué leur destinée.   ",
    Languages = new LineageLanguagesPayload
    {
      Ids = [_celfique.ResourceId],
      Extra = 1
    },
    Names = new LineageNamesModel
    {
      Family = [" Adlegor ", " Charimon ", " Galanodel ", " Kaendere ", " Liadon ", " Morwen ", " Nodir ", " Raelden ", " Sonomir ", " Talath "],
      Female = [" Althaea ", " Ceanoise ", " Elenna ", " Leshanna ", " Magilin ", " Naeva ", " Onoraid ", " Sariel ", " Tibenna ", " Valanthe "],
      Male = [" Aelar ", " Berrian ", " Erevan ", " Feirion ", " Hononn ", " Ivellios ", " Kaimear ", " Peren ", " Saemainn ", " Therion "]
    }
  };

  private static CreateOrReplaceLineagePayload CreateHumainPayload() => new()
  {
    Name = " Humain ",
    Summary = "  Espèce adaptable et ambitieuse héritière d’un empire fragmenté.  ",
    Content = "   Les humains représentent le commun des mortels. Répandus aux quatre coins du monde, ils s’adaptent avec aisance aux climats, aux cultures et aux bouleversements qui façonnent les civilisations. Héritiers d’un vaste empire ayant autrefois dominé la majeure partie de l’Ouespéro, ils vivent désormais au sein d’une mosaïque de royaumes, de cités libres et d’empires revendiquant un héritage souvent contesté. Leur culture mêle traditions impériales, foi organisée et anciennes coutumes guerrières, tandis que leur courte espérance de vie nourrit leur ambition et leur désir d’accomplissement. Perçus comme éphémères mais résilients, ils occupent une place centrale dans les alliances, les conflits et les échanges du monde connu.   ",
    Languages = new LineageLanguagesPayload
    {
      Extra = 2
    },
    Names = new LineageNamesModel
    {
      Content = "   Les humains portent généralement un prénom et un nom de famille.   "
    },
    Speeds = new LineageSpeedsModel
    {
      Walk = 6
    },
    Size = new LineageSizeModel
    {
      Category = SizeCategory.Medium,
      Height = "140+2d20"
    },
    Weight = new LineageWeightModel
    {
      Malnutrition = "10+1d4",
      Skinny = "14+1d4",
      Normal = "18+1d6",
      Overweight = "24+1d6",
      Obese = "30+1d10"
    },
    Age = new LineageAgeModel(8, 15, 30, 55)
  };

  private void AssertHautElfe(CreateOrReplaceLineagePayload payload, LineageModel lineage)
  {
    Assert.Equal(payload.Name.CleanTrim(), lineage.Name);
    Assert.Equal(payload.Summary?.CleanTrim(), lineage.Summary);
    Assert.Equal(payload.Content?.CleanTrim(), lineage.Content);

    Assert.Equal(payload.Languages.Extra, lineage.Languages.Extra);
    Assert.Equal(payload.Languages.Content?.CleanTrim(), lineage.Languages.Content);
    LanguageModel language = Assert.Single(lineage.Languages.Granted);
    Assert.Equal(_celfique.ResourceId, language.Id);

    Assert.Equal(CleanNames(payload.Names.Family), lineage.Names.Family);
    Assert.Equal(CleanNames(payload.Names.Female), lineage.Names.Female);
    Assert.Equal(CleanNames(payload.Names.Male), lineage.Names.Male);
    Assert.Empty(lineage.Names.Unisex);
    Assert.Empty(lineage.Names.Custom);
    Assert.Equal(payload.Names.Content?.CleanTrim(), lineage.Names.Content);
  }

  private static void AssertHumain(CreateOrReplaceLineagePayload payload, LineageModel lineage)
  {
    Assert.Equal(payload.Name.CleanTrim(), lineage.Name);
    Assert.Equal(payload.Summary?.CleanTrim(), lineage.Summary);
    Assert.Equal(payload.Content?.CleanTrim(), lineage.Content);

    Assert.Empty(lineage.Languages.Granted);
    Assert.Equal(payload.Languages.Extra, lineage.Languages.Extra);
    Assert.Equal(payload.Languages.Content?.CleanTrim(), lineage.Languages.Content);

    Assert.Empty(lineage.Names.Family);
    Assert.Empty(lineage.Names.Female);
    Assert.Empty(lineage.Names.Male);
    Assert.Empty(lineage.Names.Unisex);
    Assert.Empty(lineage.Names.Custom);
    Assert.Equal(payload.Names.Content?.CleanTrim(), lineage.Names.Content);

    Assert.Equal(payload.Speeds.Walk, lineage.Speeds.Walk);
    Assert.Equal(payload.Speeds.Climb, lineage.Speeds.Climb);
    Assert.Equal(payload.Speeds.Swim, lineage.Speeds.Swim);
    Assert.Equal(payload.Speeds.Fly, lineage.Speeds.Fly);
    Assert.Equal(payload.Speeds.Hover, lineage.Speeds.Hover);
    Assert.Equal(payload.Speeds.Burrow, lineage.Speeds.Burrow);

    Assert.Equal(payload.Size.Category, lineage.Size.Category);
    Assert.Equal(payload.Size.Height, lineage.Size.Height);

    Assert.Equal(payload.Weight.Malnutrition, lineage.Weight.Malnutrition);
    Assert.Equal(payload.Weight.Skinny, lineage.Weight.Skinny);
    Assert.Equal(payload.Weight.Normal, lineage.Weight.Normal);
    Assert.Equal(payload.Weight.Overweight, lineage.Weight.Overweight);
    Assert.Equal(payload.Weight.Obese, lineage.Weight.Obese);

    Assert.Equal(payload.Age.Teenager, lineage.Age.Teenager);
    Assert.Equal(payload.Age.Adult, lineage.Age.Adult);
    Assert.Equal(payload.Age.Mature, lineage.Age.Mature);
    Assert.Equal(payload.Age.Venerable, lineage.Age.Venerable);
  }

  private static List<string> CleanNames(IEnumerable<string> names) => [.. names
    .Where(name => !string.IsNullOrWhiteSpace(name))
    .Select(name => name.Trim())
    .OrderBy(name => name)
    .Distinct()];
}
