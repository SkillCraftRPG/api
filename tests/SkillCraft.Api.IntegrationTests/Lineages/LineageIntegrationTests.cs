using Krakenar.Contracts.Search;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Contracts;
using SkillCraft.Api.Contracts.Lineages;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Lineages;

namespace SkillCraft.Api.Lineages;

[Trait(Traits.Category, Categories.Integration)]
public class LineageIntegrationTests : IntegrationTests
{
  private readonly ILanguageRepository _languageRepository;
  private readonly ILineageRepository _lineageRepository;
  private readonly ILineageService _lineageService;

  private Language _language = null!;
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

    _language = new Language(World, new Name("Celfique"), UserId);
    await _languageRepository.SaveAsync(_language);

    _lineage = new Lineage(World, new Name("Lineage"), parent: null, UserId);
    await _lineageRepository.SaveAsync(_lineage);
  }

  [Theory(DisplayName = "It should create a new lineage.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_NotExist_When_CreateOrReplace_Then_Created(bool withId)
  {
    CreateOrReplaceLineagePayload payload = new()
    {
      ParentId = _lineage.EntityId,
      Name = "  Elfe sylvain  ",
      Summary = "  Elfe longévif, lié à la nature, aux sens affûtés et à la transe.  ",
      Description = "  Les elfes sylvains sont très protecteurs envers les milieux sauvages et non civilisés. Ils peuvent se montrer rapidement hostiles lorsque ces milieux sont menacés, ou que la nature est exploitée de manière illégitime. Ce lien avec la nature leur confère certaines capacités, par exemple un instinct plus développé, une habilité à se fondre dans la nature ainsi que des mouvements vifs et gracieux.  ",
    };
    payload.Features.Add(new FeatureModel("Camouflage naturel", "Le personnage peut tenter de [se cacher](/regles/combat/activites/cacher) d’une créature qui le voit lorsqu’il est [légèrement obscurci](/regles/aventure/environnement/vision) par un phénomène naturel tel du feuillage, de la pluie battante, une tempête de neige ou du brouillard."));
    payload.Languages.Ids.Add(_language.EntityId);
    payload.Names.Family.AddRange("Adlegor", "Charimon", "Galanodel", "Kaendere", "Liadon", "Morwen", "Nodir", "Raelden", "Sonomir", "Talath");
    payload.Names.Female.AddRange("Althaea", "Ceanoise", "Elenna", "Leshanna", "Magilin", "Naeva", "Onoraid", "Sariel", "Tibenna", "Valanthe");
    payload.Names.Male.AddRange("Aelar", "Berrian", "Erevan", "Feirion", "Hononn", "Ivellios", "Kaimear", "Peren", "Saemainn", "Therion");
    payload.Names.Unisex.AddRange("", "  Test  ", "Abc");
    payload.Names.Custom.Add(new NameCategory("Category", ["  Name  ", "Eman"]));
    Guid? id = withId ? Guid.NewGuid() : null;

    CreateOrReplaceLineageResult result = await _lineageService.CreateOrReplaceAsync(payload, id);
    Assert.True(result.Created);
    LineageModel lineage = result.Lineage;

    if (id.HasValue)
    {
      Assert.Equal(id.Value, lineage.Id);
    }
    Assert.Equal(2, lineage.Version);
    Assert.Equal(Actor, lineage.CreatedBy);
    Assert.Equal(DateTime.UtcNow, lineage.CreatedOn, TimeSpan.FromSeconds(1));
    Assert.Equal(Actor, lineage.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, lineage.UpdatedOn, TimeSpan.FromSeconds(1));

    Assert.Equal(payload.ParentId, lineage.Parent?.Id);
    Assert.Equal(payload.Name.Trim(), lineage.Name);
    Assert.Equal(payload.Summary.Trim(), lineage.Summary);
    Assert.Equal(payload.Description.Trim(), lineage.Description);
    Assert.Equal(payload.Features, lineage.Features);

    Assert.True(payload.Languages.Ids.SequenceEqual(lineage.Languages.Items.Select(language => language.Id)));
    Assert.Equal(payload.Languages.Extra, lineage.Languages.Extra);
    Assert.Equal(payload.Languages.Text, lineage.Languages.Text);

    Assert.Equal(payload.Names.Family, lineage.Names.Family);
    Assert.Equal(payload.Names.Female, lineage.Names.Female);
    Assert.Equal(payload.Names.Male, lineage.Names.Male);
    Assert.True(new string[] { "Abc", "Test" }.SequenceEqual(lineage.Names.Unisex));
    NameCategory nameCategory = Assert.Single(lineage.Names.Custom);
    Assert.Equal("Category", nameCategory.Category);
    Assert.True(new string[] { "Eman", "Name" }.SequenceEqual(nameCategory.Names));

    Assert.Equal(payload.Speeds, lineage.Speeds);
    Assert.Equal(payload.Size, lineage.Size);
    Assert.Equal(payload.Weight, lineage.Weight);
    Assert.Equal(payload.Age, lineage.Age);
  }

  [Fact(DisplayName = "It should delete an existing lineage.")]
  public async Task Given_Exists_When_Delete_ThenDeleted()
  {
    Guid id = _lineage.EntityId;
    LineageModel? lineage = await _lineageService.DeleteAsync(id);
    Assert.NotNull(lineage);
    Assert.Equal(id, lineage.Id);
  }

  [Fact(DisplayName = "It should read an existing lineage.")]
  public async Task Given_Exists_When_Read_Then_Returned()
  {
    Guid id = _lineage.EntityId;
    LineageModel? lineage = await _lineageService.ReadAsync(id);
    Assert.NotNull(lineage);
    Assert.Equal(id, lineage.Id);
  }

  [Fact(DisplayName = "It should replace an existing lineage.")]
  public async Task Given_Exists_When_CreateOrReplace_Then_Replaced()
  {
    CreateOrReplaceLineagePayload payload = new()
    {
      Name = "  Elfe  ",
      Summary = "  Elfe longévif, lié à la nature, aux sens affûtés et à la transe.  ",
      Description = "  Les elfes sont des créatures attachées à l’ordre naturel et aux valeurs traditionnelles. Ils sont originaires d’un autre continent, où ils ont évolué en tissant des liens avec la nature et les arts occultes. À l’origine des êtres nocturnes, ils ont développé un court sommeil réparateur. On dit d’eux qu’ils sont habiles de leurs mains et que leurs sens sont aiguisés. Leur silhouette est généralement svelte et élancée. Ils sont considérés comme immortels par les autres espèces à cause de leur longévité hors du commun.  ",
      Speeds = new SpeedsModel(6, 4, 3, 2, hover: true, 1),
      Size = new SizeModel(SizeCategory.Small, "130+3d20"),
      Weight = new WeightModel("9+1d3", "12+1d4", "16+1d6", "22+1d6", "28+1d8"),
      Age = new AgeModel(30, 100, 275, 750)
    };
    payload.Features.Add(new FeatureModel("Esprit éveillé", "Le personnage ne peut être endormi de manière surnaturelle.\n\nÉgalement, il peut remplacer ses [6 heures](/regles/aventure/temps) de sommeil quotidien par 4 heures de transe.\n\nEn plus d’écourter sa [nuit de sommeil](/regles/aventure/repos/sommeil), il est en état de conscience partielle pendant cette transe, ce qui lui permet d’effectuer avec [désavantage](/regles/competences/tests/avantage-desavantage) des [tests passifs](/regles/competences/tests/passif).\n\nIl doit tout de même jumeler cette transe avec 2 heures de [halte](/regles/aventure/repos/halte) afin de compléter sa nuit de sommeil."));
    payload.Features.Add(new FeatureModel("Sens affûtés", "Le personnage peut [acquérir](/regles/talents/acquisition) [à rabais](/regles/talents/points) les talents [Orientation](/regles/talents/orientation) et [Perception](/regles/talents/perception)."));
    payload.Languages.Extra = 1;
    payload.Languages.Text = "  Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nam a tempor mi. Ut euismod lectus at tempus vehicula. Sed ornare quam quis condimentum egestas. Integer consequat nisl vitae enim mattis, vitae tempor augue condimentum. Nulla facilisi. Etiam ac turpis id mauris iaculis rutrum eget quis tellus. Nullam sed arcu pulvinar dolor maximus viverra vel quis magna. Aenean in auctor tellus. Fusce ultrices, justo porta sollicitudin pharetra, mi dui dictum sapien, in pharetra mi turpis at lectus. Praesent id tempus velit. Nam tristique vel lorem a convallis.  ";
    Guid id = _lineage.EntityId;

    CreateOrReplaceLineageResult result = await _lineageService.CreateOrReplaceAsync(payload, id);
    Assert.False(result.Created);
    LineageModel lineage = result.Lineage;

    Assert.Equal(id, lineage.Id);
    Assert.Equal(2, lineage.Version);
    Assert.Equal(Actor, lineage.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, lineage.UpdatedOn, TimeSpan.FromSeconds(1));

    Assert.Equal(payload.ParentId, lineage.Parent?.Id);
    Assert.Equal(payload.Name.Trim(), lineage.Name);
    Assert.Equal(payload.Summary.Trim(), lineage.Summary);
    Assert.Equal(payload.Description.Trim(), lineage.Description);
    Assert.Equal(payload.Features, lineage.Features);
    Assert.True(payload.Languages.Ids.SequenceEqual(lineage.Languages.Items.Select(language => language.Id)));
    Assert.Equal(payload.Languages.Extra, lineage.Languages.Extra);
    Assert.Equal(payload.Languages.Text.Trim(), lineage.Languages.Text);
    Assert.Equal(payload.Names.Family, lineage.Names.Family);
    Assert.Equal(payload.Names.Female, lineage.Names.Female);
    Assert.Equal(payload.Names.Male, lineage.Names.Male);
    Assert.Equal(payload.Names.Unisex, lineage.Names.Unisex);
    Assert.Equal(payload.Names.Custom, lineage.Names.Custom);
    Assert.Equal(payload.Names.Text, lineage.Names.Text);
    Assert.Equal(payload.Speeds, lineage.Speeds);
    Assert.Equal(payload.Size, lineage.Size);
    Assert.Equal(payload.Weight, lineage.Weight);
    Assert.Equal(payload.Age, lineage.Age);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task Given_Payload_When_Search_Then_Results()
  {
    Lineage elfe = new(World, new Name("Elfe"), parent: null, UserId);
    Lineage humain = new(World, new Name("Humain"), parent: null, UserId);
    Lineage nain = new(World, new Name("Nain"), parent: null, UserId);
    Lineage petitGens = new(World, new Name("Petit-Gens"), parent: null, UserId);
    petitGens.Size = new Size(SizeCategory.Small, new Roll("80+2d10"));
    petitGens.Update(UserId);
    await _lineageRepository.SaveAsync([elfe, humain, nain, petitGens]);

    SearchLineagesPayload payload = new()
    {
      SizeCategory = SizeCategory.Medium,
      Skip = 1,
      Limit = 1
    };
    payload.Ids.AddRange([elfe.EntityId, humain.EntityId, nain.EntityId, petitGens.EntityId, Guid.Empty]);
    payload.Search.Operator = SearchOperator.Or;
    payload.Search.Terms.AddRange([new SearchTerm("%i%")]);
    payload.Sort.Add(new LineageSortOption(LineageSort.Name, isDescending: true));

    SearchResults<LineageModel> results = await _lineageService.SearchAsync(payload);
    Assert.Equal(2, results.Total);

    LineageModel result = Assert.Single(results.Items);
    Assert.Equal(humain.EntityId, result.Id);
  }

  [Fact(DisplayName = "It should return the correct search results (LanguageId).")]
  public async Task Given_LanguageId_When_Search_Then_Results()
  {
    Lineage lineage = new(World, new Name("Elfe"), parent: null, UserId);
    lineage.SetLanguages([_language]);
    lineage.Update(UserId);
    await _lineageRepository.SaveAsync(lineage);

    SearchLineagesPayload payload = new()
    {
      LanguageId = _language.EntityId
    };
    SearchResults<LineageModel> results = await _lineageService.SearchAsync(payload);
    Assert.Equal(1, results.Total);

    LineageModel result = Assert.Single(results.Items);
    Assert.Equal(lineage.EntityId, result.Id);
  }

  [Theory(DisplayName = "It should return the correct search results (ParentId).")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_ParentId_When_Search_Then_Results(bool isParent)
  {
    Lineage lineage = new(World, new Name("Elfe sylvain"), _lineage, UserId);
    lineage.SetLanguages([_language]);
    lineage.Update(UserId);
    await _lineageRepository.SaveAsync(lineage);

    SearchLineagesPayload payload = new()
    {
      ParentId = isParent ? null : _lineage.EntityId
    };
    SearchResults<LineageModel> results = await _lineageService.SearchAsync(payload);
    Assert.Equal(1, results.Total);

    LineageModel result = Assert.Single(results.Items);
    Assert.Equal(isParent ? _lineage.EntityId : lineage.EntityId, result.Id);
  }

  [Fact(DisplayName = "It should update an existing lineage.")]
  public async Task Given_Exists_When_Update_ThenUpdated()
  {
    Guid id = _lineage.EntityId;
    UpdateLineagePayload payload = new()
    {
      Name = "  Humain  ",
      Summary = new Update<string>("  Êtres polyvalents et adaptables, maîtres de l’apprentissage rapide.  "),
      Description = new Update<string>("  Les humains représentent le commun des mortels. Répandus sur toute la surface du monde, ils s’adaptent au climat des territoires sur lesquels ils s’installent. Leur apparence physique et leurs traits psychologiques varient grandement en fonction de leur habitat et de leurs traditions. Leur capacité d’apprentissage constitue une caractéristique qui leur est unique et qui surpasse celle des autres espèces.  "),
      Features = [],
      Languages = new LanguagesPayload(ids: null, extra: 2),
      Names = new NamesModel
      {
        Family = ["Anceau", "Beaumont", "Chappelle", "Hérisson", "Langloys", "Marchant", "Petit", "Rousseau", "Touppin", "Vivien"],
        Female = ["Agnés", "Guillemette", "Jehanne", "Loyse", "Mahault", "Margot", "Marie", "Phlipote", "Plantée", "Ysabel"],
        Male = ["Arnoul", "Colin", "Estienne", "Françoys", "Gaultier", "Gillebert", "Loys", "Pasquier", "Perrin", "Thomas"],
        Unisex = ["Camille", "Swann"],
        Custom = [new NameCategory("Surnom", ["La Grande", "Le Gros"])],
        Text = "Les humains portent généralement un prénom et un nom de famille."
      },
      Speeds = new SpeedsModel(6),
      Size = new SizeModel(SizeCategory.Medium, "140+2d20"),
      Weight = new WeightModel("10+1d4", "14+1d4", "18+1d6", "24+1d6", "30+1d10"),
      Age = new AgeModel(8, 15, 30, 55)
    };
    payload.Features.Add(new FeatureModel("Apprentissage accéléré", "Le personnage débute avec 4 points d’[Apprentissage](/regles/statistiques/apprentissage) supplémentaires.\n\nIl acquiert également 1 point d’Apprentissage supplémentaire chaque fois que son [tiers](/regles/personnages/progression/tiers) augmente."));
    payload.Features.Add(new FeatureModel("Aspect", "Le personnage acquiert gratuitement le talent [Entraînement I](/regles/talents/entrainement-i)."));
    payload.Features.Add(new FeatureModel("Versatilité", "Le personnage peut [acquérir](/regles/talents/acquisition) [à rabais](/regles/talents/points) deux [talents](/regles/talents) le [formant](/regles/competences/formation) pour une [compétence](/regles/competences)."));

    LineageModel? lineage = await _lineageService.UpdateAsync(id, payload);
    Assert.NotNull(lineage);

    Assert.Equal(id, lineage.Id);
    Assert.Equal(2, lineage.Version);
    Assert.Equal(Actor, lineage.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, lineage.UpdatedOn, TimeSpan.FromSeconds(1));

    Assert.Equal(payload.Name.Trim(), lineage.Name);
    Assert.Equal(payload.Summary.Value?.Trim(), lineage.Summary);
    Assert.Equal(payload.Description.Value?.Trim(), lineage.Description);
    Assert.Equal(payload.Features, lineage.Features);
    Assert.True(payload.Languages.Ids.SequenceEqual(lineage.Languages.Items.Select(language => language.Id)));
    Assert.Equal(payload.Languages.Extra, lineage.Languages.Extra);
    Assert.Equal(payload.Languages.Text, lineage.Languages.Text);
    Assert.Equal(payload.Names.Family, lineage.Names.Family);
    Assert.Equal(payload.Names.Female, lineage.Names.Female);
    Assert.Equal(payload.Names.Male, lineage.Names.Male);
    Assert.Equal(payload.Names.Unisex, lineage.Names.Unisex);

    NameCategory category1 = Assert.Single(payload.Names.Custom);
    NameCategory category2 = Assert.Single(lineage.Names.Custom);
    Assert.Equal(category1.Category, category2.Category);
    Assert.True(category1.Names.SequenceEqual(category2.Names));

    Assert.Equal(payload.Names.Text, lineage.Names.Text);
    Assert.Equal(payload.Speeds, lineage.Speeds);
    Assert.Equal(payload.Size, lineage.Size);
    Assert.Equal(payload.Weight, lineage.Weight);
    Assert.Equal(payload.Age, lineage.Age);
  }
}
