using Krakenar.Contracts.Search;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Contracts;
using SkillCraft.Api.Contracts.Customizations;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Customizations;

namespace SkillCraft.Api.Customizations;

[Trait(Traits.Category, Categories.Integration)]
public class CustomizationIntegrationTests : IntegrationTests
{
  private readonly ICustomizationRepository _customizationRepository;
  private readonly ICustomizationService _customizationService;

  private Customization _customization = null!;

  public CustomizationIntegrationTests() : base()
  {
    _customizationRepository = ServiceProvider.GetRequiredService<ICustomizationRepository>();
    _customizationService = ServiceProvider.GetRequiredService<ICustomizationService>();
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    _customization = new Customization(World, CustomizationKind.Disability, new Name("Disability"), UserId);
    await _customizationRepository.SaveAsync(_customization);
  }

  [Theory(DisplayName = "It should create a new customization.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_NotExist_When_CreateOrReplace_Then_Created(bool withId)
  {
    CreateOrReplaceCustomizationPayload payload = new()
    {
      Kind = CustomizationKind.Disability,
      Name = "  Abruti  ",
      Summary = "  Limité, maladroit et désavantagé dans l’usage de son intellect.  ",
      Description = "  Simple d’esprit et bon vivant ou complètement taré, le personnage est dénué de sens commun et n’est pas doté d’une bonne mémoire.\n\nSes [tests](/regles/competences/tests) d’[Intellect](/regles/attributs/intellect) et des [compétences](/regles/competences) associées ([Connaissance](/regles/competences/connaissance), [Investigation](/regles/competences/investigation), [Linguistique](/regles/competences/linguistique) et [Médecine](/regles/competences/medecine)) sont affligés du [désavantage](/regles/competences/tests/avantage-desavantage).  "
    };
    Guid? id = withId ? Guid.NewGuid() : null;

    CreateOrReplaceCustomizationResult result = await _customizationService.CreateOrReplaceAsync(payload, id);
    Assert.True(result.Created);
    CustomizationModel customization = result.Customization;

    if (id.HasValue)
    {
      Assert.Equal(id.Value, customization.Id);
    }
    Assert.Equal(2, customization.Version);
    Assert.Equal(Actor, customization.CreatedBy);
    Assert.Equal(DateTime.UtcNow, customization.CreatedOn, TimeSpan.FromSeconds(1));
    Assert.Equal(Actor, customization.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, customization.UpdatedOn, TimeSpan.FromSeconds(1));

    Assert.Equal(payload.Name.Trim(), customization.Name);
    Assert.Equal(payload.Summary.Trim(), customization.Summary);
    Assert.Equal(payload.Description.Trim(), customization.Description);
  }

  [Fact(DisplayName = "It should delete an existing customization.")]
  public async Task Given_Exists_When_Delete_ThenDeleted()
  {
    Guid id = _customization.EntityId;
    CustomizationModel? customization = await _customizationService.DeleteAsync(id);
    Assert.NotNull(customization);
    Assert.Equal(id, customization.Id);
  }

  [Fact(DisplayName = "It should read an existing customization.")]
  public async Task Given_Exists_When_Read_Then_Returned()
  {
    Guid id = _customization.EntityId;
    CustomizationModel? customization = await _customizationService.ReadAsync(id);
    Assert.NotNull(customization);
    Assert.Equal(id, customization.Id);
  }

  [Fact(DisplayName = "It should replace an existing customization.")]
  public async Task Given_Exists_When_CreateOrReplace_Then_Replaced()
  {
    CreateOrReplaceCustomizationPayload payload = new()
    {
      Kind = CustomizationKind.Disability,
      Name = "  Abruti  ",
      Summary = "  Limité, maladroit et désavantagé dans l’usage de son intellect.  ",
      Description = "  Simple d’esprit et bon vivant ou complètement taré, le personnage est dénué de sens commun et n’est pas doté d’une bonne mémoire.\n\nSes [tests](/regles/competences/tests) d’[Intellect](/regles/attributs/intellect) et des [compétences](/regles/competences) associées ([Connaissance](/regles/competences/connaissance), [Investigation](/regles/competences/investigation), [Linguistique](/regles/competences/linguistique) et [Médecine](/regles/competences/medecine)) sont affligés du [désavantage](/regles/competences/tests/avantage-desavantage).  "
    };
    Guid id = _customization.EntityId;

    CreateOrReplaceCustomizationResult result = await _customizationService.CreateOrReplaceAsync(payload, id);
    Assert.False(result.Created);
    CustomizationModel customization = result.Customization;

    Assert.Equal(id, customization.Id);
    Assert.Equal(2, customization.Version);
    Assert.Equal(Actor, customization.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, customization.UpdatedOn, TimeSpan.FromSeconds(1));

    Assert.Equal(payload.Name.Trim(), customization.Name);
    Assert.Equal(payload.Summary.Trim(), customization.Summary);
    Assert.Equal(payload.Description.Trim(), customization.Description);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task Given_Payload_When_Search_Then_Results()
  {
    Customization aigrefin = new(World, CustomizationKind.Gift, new Name("Aigrefin"), UserId);
    Customization efface = new(World, CustomizationKind.Gift, new Name("Effacé"), UserId);
    Customization feroce = new(World, CustomizationKind.Gift, new Name("Féroce"), UserId);
    Customization richesse = new(World, CustomizationKind.Gift, new Name("Richesse"), UserId);
    richesse.Summary = new Summary("Multiplie la richesse de départ pour un statut aisé ou noble.");
    richesse.Update(UserId);
    await _customizationRepository.SaveAsync([aigrefin, efface, feroce, richesse]);

    SearchCustomizationsPayload payload = new()
    {
      Kind = CustomizationKind.Gift,
      Skip = 1,
      Limit = 1
    };
    payload.Ids.AddRange([_customization.EntityId, aigrefin.EntityId, feroce.EntityId, richesse.EntityId, Guid.Empty]);
    payload.Search.Operator = SearchOperator.Or;
    payload.Search.Terms.AddRange([new SearchTerm("disa%"), new SearchTerm("%é%")]);
    payload.Sort.Add(new CustomizationSortOption(CustomizationSort.Name));

    SearchResults<CustomizationModel> results = await _customizationService.SearchAsync(payload);
    Assert.Equal(2, results.Total);

    CustomizationModel result = Assert.Single(results.Items);
    Assert.Equal(richesse.EntityId, result.Id);
  }

  [Fact(DisplayName = "It should update an existing customization.")]
  public async Task Given_Exists_When_Update_ThenUpdated()
  {
    Guid id = _customization.EntityId;
    UpdateCustomizationPayload payload = new()
    {
      Name = "  Abruti  ",
      Summary = new Update<string>("  Limité, maladroit et désavantagé dans l’usage de son intellect.  "),
      Description = new Update<string>("  Simple d’esprit et bon vivant ou complètement taré, le personnage est dénué de sens commun et n’est pas doté d’une bonne mémoire.\n\nSes [tests](/regles/competences/tests) d’[Intellect](/regles/attributs/intellect) et des [compétences](/regles/competences) associées ([Connaissance](/regles/competences/connaissance), [Investigation](/regles/competences/investigation), [Linguistique](/regles/competences/linguistique) et [Médecine](/regles/competences/medecine)) sont affligés du [désavantage](/regles/competences/tests/avantage-desavantage).  ")
    };

    CustomizationModel? customization = await _customizationService.UpdateAsync(id, payload);
    Assert.NotNull(customization);

    Assert.Equal(id, customization.Id);
    Assert.Equal(2, customization.Version);
    Assert.Equal(Actor, customization.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, customization.UpdatedOn, TimeSpan.FromSeconds(1));

    Assert.Equal(payload.Name.Trim(), customization.Name);
    Assert.Equal(payload.Summary.Value?.Trim(), customization.Summary);
    Assert.Equal(payload.Description.Value?.Trim(), customization.Description);
  }
}
