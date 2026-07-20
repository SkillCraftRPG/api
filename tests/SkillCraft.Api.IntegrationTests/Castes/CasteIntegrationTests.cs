using Krakenar.Contracts.Search;
using Logitar;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Builders;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Castes;
using SkillCraft.Api.Core.Castes.Models;
using SkillCraft.Api.Core.Features;
using SkillCraft.Api.Core.Permissions;

namespace SkillCraft.Api.IntegrationTests.Castes;

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

    _caste = new CasteBuilder(Faker).WithWorld(Context.World).Build();
    _casteRepository.Add(_caste);
    await Context.SaveChangesAsync();
  }

  [Theory(DisplayName = "It should create a new caste.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_NotExist_When_CreateOrReplace_Then_Created(bool withId)
  {
    CreateOrReplaceCastePayload payload = CreateArtisanPayload();
    Guid? id = withId ? Guid.NewGuid() : null;

    CreateOrReplaceCasteResult result = await _casteService.CreateOrReplaceAsync(payload, id);
    Assert.True(result.Created);
    CasteModel caste = result.Caste;
    Assert.NotNull(caste);

    if (id.HasValue)
    {
      Assert.Equal(id.Value, caste.Id);
    }
    else
    {
      Assert.NotEqual(Guid.Empty, caste.Id);
    }
    Assert.Equal(1, caste.Version);
    Assert.Equal(Actor, caste.CreatedBy);
    Assert.Equal(DateTime.UtcNow, caste.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(caste.CreatedBy, caste.UpdatedBy);
    Assert.Equal(caste.CreatedOn, caste.UpdatedOn);

    AssertArtisan(payload, caste);
  }

  [Fact(DisplayName = "It should filter search results by skill.")]
  public async Task Given_Skill_When_Search_Then_Results()
  {
    Caste artisan = CasteBuilder.Artisan(Faker, Context.World);
    _casteRepository.Add(artisan);
    await Context.SaveChangesAsync();

    SearchCastesPayload payload = new()
    {
      Skill = Skill.Crafting
    };

    SearchResults<CasteModel> results = await _casteService.SearchAsync(payload);
    Assert.Equal(1, results.Total);

    CasteModel caste = Assert.Single(results.Items);
    Assert.Equal(artisan.Id, caste.Id);
  }

  [Fact(DisplayName = "It should read a caste by ID.")]
  public async Task Given_Id_When_Read_Then_Read()
  {
    CasteModel? caste = await _casteService.ReadAsync(_caste.Id);
    Assert.NotNull(caste);
    Assert.Equal(_caste.Id, caste.Id);
  }

  [Fact(DisplayName = "It should replace an existing caste.")]
  public async Task Given_Exists_When_CreateOrReplace_Then_Replaced()
  {
    CreateOrReplaceCastePayload payload = CreateArtisanPayload();
    Guid id = _caste.Id;

    CreateOrReplaceCasteResult result = await _casteService.CreateOrReplaceAsync(payload, id);
    Assert.False(result.Created);
    CasteModel caste = result.Caste;
    Assert.NotNull(caste);

    Assert.Equal(id, caste.Id);
    Assert.Equal(2, caste.Version);
    Assert.Equal(_caste.CreatedBy, caste.CreatedBy.Id);
    Assert.Equal(_caste.CreatedOn, caste.CreatedOn, TimeSpan.FromMilliseconds(1));
    Assert.Equal(Actor, caste.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, caste.UpdatedOn, TimeSpan.FromSeconds(10));

    AssertArtisan(payload, caste);
  }

  [Fact(DisplayName = "It should return empty search results.")]
  public async Task Given_NoMatch_When_Search_Then_EmptyResults()
  {
    Context.World = new WorldBuilder(Faker).Build();

    SearchCastesPayload payload = new();

    SearchResults<CasteModel> results = await _casteService.SearchAsync(payload);
    Assert.Equal(0, results.Total);
    Assert.Empty(results.Items);
  }

  [Fact(DisplayName = "It should return null when no caste was found.")]
  public async Task Given_NotFound_When_Read_Then_NullReturned()
  {
    Context.World = new WorldBuilder(Faker).Build();

    Assert.Null(await _casteService.ReadAsync(_caste.Id));
  }

  [Fact(DisplayName = "It should return null when the caste was not found.")]
  public async Task Given_NotFound_When_Update_Then_NullReturned()
  {
    Assert.Null(await _casteService.UpdateAsync(Guid.Empty, new UpdateCastePayload()));
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task Given_Matches_When_Search_Then_Results()
  {
    Caste artisan = CasteBuilder.Artisan(Faker, Context.World);
    Caste boheme = new CasteBuilder(Faker).WithWorld(Context.World).WithName("Bohème").Build();
    Caste milicien = new CasteBuilder(Faker).WithWorld(Context.World).WithName("Milicien").Build();
    _casteRepository.Add(artisan, boheme, milicien);
    await Context.SaveChangesAsync();

    SearchCastesPayload payload = new()
    {
      Skip = 1,
      Limit = 1
    };
    payload.Search.Operator = SearchOperator.Or;
    payload.Search.Terms.Add(new SearchTerm("%p%"));
    payload.Search.Terms.Add(new SearchTerm("%m%"));
    payload.Ids.AddRange([_caste.Id, artisan.Id, Guid.Empty, milicien.Id]);
    payload.Sort.Add(new CasteSortOption(CasteSort.Name, isDescending: true));

    SearchResults<CasteModel> results = await _casteService.SearchAsync(payload);
    Assert.Equal(2, results.Total);

    CasteModel caste = Assert.Single(results.Items);
    Assert.Equal(artisan.Id, caste.Id);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when creating a caste.")]
  public async Task Given_NotAllowed_When_Create_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder(Faker).Build();

    CreateOrReplaceCastePayload payload = CreateArtisanPayload();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _casteService.CreateOrReplaceAsync(payload));
    Assert.Equal(Context.UserId, exception.UserId);
    Assert.Equal(Actions.CreateCaste, exception.Action);
    Assert.Equal(Context.World?.Identifier.ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when replacing a caste.")]
  public async Task Given_NotAllowed_When_Replace_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder(Faker).Build();

    CreateOrReplaceCastePayload payload = CreateArtisanPayload();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _casteService.CreateOrReplaceAsync(payload, _caste.Id));
    Assert.Equal(Context.UserId, exception.UserId);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(_caste.Identifier.ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when updating a caste.")]
  public async Task Given_NotAllowed_When_Update_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder(Faker).Build();

    UpdateCastePayload payload = new();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _casteService.UpdateAsync(_caste.Id, payload));
    Assert.Equal(Context.UserId, exception.UserId);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(_caste.Identifier.ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should update an existing caste.")]
  public async Task Given_Exists_When_Update_Then_Updated()
  {
    Guid id = _caste.Id;
    CreateOrReplaceCastePayload create = CreateArtisanPayload();
    UpdateCastePayload payload = new()
    {
      Name = create.Name,
      Summary = new Optional<string>(create.Summary),
      HtmlContent = new Optional<string>(create.HtmlContent),
      Skill = new Optional<Skill?>(create.Skill),
      WealthRoll = new Optional<string>(create.WealthRoll),
      Feature = new Optional<FeatureModel>(create.Feature)
    };

    CasteModel? caste = await _casteService.UpdateAsync(id, payload);
    Assert.NotNull(caste);

    Assert.Equal(id, caste.Id);
    Assert.Equal(2, caste.Version);
    Assert.Equal(_caste.CreatedBy, caste.CreatedBy.Id);
    Assert.Equal(_caste.CreatedOn, caste.CreatedOn, TimeSpan.FromMilliseconds(1));
    Assert.Equal(Actor, caste.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, caste.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Equal(payload.Name.CleanTrim(), caste.Name);
    Assert.Equal(payload.Summary.Value?.CleanTrim(), caste.Summary);
    Assert.Equal(payload.HtmlContent.Value?.CleanTrim(), caste.HtmlContent);
    Assert.Equal(payload.Skill.Value, caste.Skill);
    Assert.Equal(payload.WealthRoll.Value?.CleanTrim()?.ToLowerInvariant(), caste.WealthRoll);
    Assert.NotNull(caste.Feature);
    Assert.Equal(payload.Feature.Value!.Name.Trim(), caste.Feature.Name);
    Assert.Equal(payload.Feature.Value.HtmlContent?.CleanTrim(), caste.Feature.HtmlContent);
  }

  private static CreateOrReplaceCastePayload CreateArtisanPayload() => new()
  {
    Name = " Artisan ",
    Summary = "  Expert des métiers manuels, membre d’une organisation d’artisans.  ",
    HtmlContent = "   L’artisan est un expert d’un procédé de transformation des matières brutes.\n\nIl peut être un boulanger, un forgeron, un orfèvre, un tisserand ou pratiquer tout genre de profession œuvrant dans la transformation des matières brutes.   ",
    Skill = Skill.Crafting,
    WealthRoll = " 8D6 ",
    Feature = new FeatureModel(
      " Professionnel ",
      "   Grâce à ses apprentissages et à ses réalisations, le personnage est membre d’une organisation de professionnels comme lui, ou il connait ces organisations.\n\nS’il ne peut subvenir à ses besoins, il n’aura aucun mal à trouver du travail grâce à ces organisations afin de couvrir minimalement ces [dépenses](/regles/equipement/depenses).\n\nCes organisations possèdent souvent un pouvoir politique important, ce qui peut l’aider à rencontrer des gens importants, à rallier des fidèles à une cause ou à mettre la main sur des matériaux rares.\n\nIl connait également la base du fonctionnement des systèmes économiques auxquels il a participé.   ")
  };

  private static void AssertArtisan(CreateOrReplaceCastePayload payload, CasteModel caste)
  {
    Assert.Equal(payload.Name.CleanTrim(), caste.Name);
    Assert.Equal(payload.Summary?.CleanTrim(), caste.Summary);
    Assert.Equal(payload.HtmlContent?.CleanTrim(), caste.HtmlContent);
    Assert.Equal(payload.Skill, caste.Skill);
    Assert.Equal(payload.WealthRoll?.CleanTrim()?.ToLowerInvariant(), caste.WealthRoll);
    Assert.NotNull(caste.Feature);
    Assert.Equal(payload.Feature!.Name.Trim(), caste.Feature.Name);
    Assert.Equal(payload.Feature.HtmlContent?.CleanTrim(), caste.Feature.HtmlContent);
  }
}
