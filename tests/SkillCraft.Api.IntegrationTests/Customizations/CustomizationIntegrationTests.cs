using Krakenar.Contracts.Search;
using Logitar;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Builders;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Actors;
using SkillCraft.Api.Core.Customizations;
using SkillCraft.Api.Core.Customizations.Models;
using SkillCraft.Api.Core.Permissions;

namespace SkillCraft.Api.IntegrationTests.Customizations;

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

    _customization = new CustomizationBuilder(Faker).WithWorld(Context.World).WithKind(CustomizationKind.Gift).Build();
    await _customizationRepository.SaveAsync(_customization);
  }

  [Theory(DisplayName = "It should create a new customization.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_NotExist_When_CreateOrReplace_Then_Created(bool withId)
  {
    CreateOrReplaceCustomizationPayload payload = CreateBaraquePayload();
    Guid? id = withId ? Guid.NewGuid() : null;

    CreateOrReplaceCustomizationResult result = await _customizationService.CreateOrReplaceAsync(payload, id);
    Assert.True(result.Created);
    CustomizationModel customization = result.Customization;
    Assert.NotNull(customization);

    if (id.HasValue)
    {
      Assert.Equal(id.Value, customization.Id);
    }
    else
    {
      Assert.NotEqual(Guid.Empty, customization.Id);
    }
    Assert.Equal(2, customization.Version);
    Assert.Equal(Actor, customization.CreatedBy);
    Assert.Equal(DateTime.UtcNow, customization.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(customization.CreatedBy, customization.UpdatedBy);
    Assert.True(customization.CreatedOn < customization.UpdatedOn);

    AssertBaraque(payload, customization);
  }

  [Fact(DisplayName = "It should read a customization by ID.")]
  public async Task Given_Id_When_Read_Then_Read()
  {
    CustomizationModel? customization = await _customizationService.ReadAsync(_customization.ResourceId);
    Assert.NotNull(customization);
    Assert.Equal(_customization.ResourceId, customization.Id);
  }

  [Fact(DisplayName = "It should replace an existing customization.")]
  public async Task Given_Exists_When_CreateOrReplace_Then_Replaced()
  {
    CreateOrReplaceCustomizationPayload payload = CreateBaraquePayload();
    Guid id = _customization.ResourceId;

    CreateOrReplaceCustomizationResult result = await _customizationService.CreateOrReplaceAsync(payload, id);
    Assert.False(result.Created);
    CustomizationModel customization = result.Customization;
    Assert.NotNull(customization);

    Assert.Equal(id, customization.Id);
    Assert.Equal(3, customization.Version);
    Assert.Equal(_customization.CreatedBy, customization.CreatedBy.GetActorId());
    Assert.Equal(_customization.CreatedOn.AsUniversalTime(), customization.CreatedOn, TimeSpan.FromMilliseconds(1));
    Assert.Equal(Actor, customization.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, customization.UpdatedOn, TimeSpan.FromSeconds(10));

    AssertBaraque(payload, customization);
  }

  [Fact(DisplayName = "It should return empty search results.")]
  public async Task Given_NoMatch_When_Search_Then_EmptyResults()
  {
    Context.World = new WorldBuilder(Faker).Build();

    SearchCustomizationsPayload payload = new();

    SearchResults<CustomizationModel> results = await _customizationService.SearchAsync(payload);
    Assert.Equal(0, results.Total);
    Assert.Empty(results.Items);
  }

  [Fact(DisplayName = "It should return null when no customization was found.")]
  public async Task Given_NotFound_When_Read_Then_NullReturned()
  {
    Context.World = new WorldBuilder(Faker).Build();

    Assert.Null(await _customizationService.ReadAsync(_customization.ResourceId));
  }

  [Fact(DisplayName = "It should return null when the customization was not found.")]
  public async Task Given_NotFound_When_Update_Then_NullReturned()
  {
    Assert.Null(await _customizationService.UpdateAsync(Guid.Empty, new UpdateCustomizationPayload()));
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task Given_Matches_When_Search_Then_Results()
  {
    Customization abruti = new CustomizationBuilder(Faker).WithWorld(Context.World).WithKind(CustomizationKind.Disability).WithName("Abruti").Build();
    Customization adresseLegendaire = new CustomizationBuilder(Faker).WithWorld(Context.World).WithKind(CustomizationKind.Gift).WithName("Adresse l�gendaire").Build();
    Customization affiniteAnimale = new CustomizationBuilder(Faker).WithWorld(Context.World).WithKind(CustomizationKind.Gift).WithName("Affinit� animale").Build();
    Customization baraque = new CustomizationBuilder(Faker).WithWorld(Context.World).WithKind(CustomizationKind.Gift).WithName("Baraqu�").Build();
    await _customizationRepository.SaveAsync([abruti, adresseLegendaire, affiniteAnimale, baraque]);

    SearchCustomizationsPayload payload = new()
    {
      Kind = CustomizationKind.Gift,
      Skip = 1,
      Limit = 1
    };
    payload.Search.Operator = SearchOperator.Or;
    payload.Search.Terms.Add(new SearchTerm("%b%"));
    payload.Search.Terms.Add(new SearchTerm("%l%"));
    payload.Ids.AddRange([_customization.ResourceId, abruti.ResourceId, adresseLegendaire.ResourceId, Guid.Empty, baraque.ResourceId]);
    payload.Sort.Add(new CustomizationSortOption(CustomizationSort.Name));

    SearchResults<CustomizationModel> results = await _customizationService.SearchAsync(payload);
    Assert.Equal(2, results.Total);

    CustomizationModel customization = Assert.Single(results.Items);
    Assert.Equal(baraque.ResourceId, customization.Id);
  }

  [Fact(DisplayName = "It should throw ImmutablePropertyException when the kind is changing.")]
  public async Task Given_DifferentKind_When_Replace_Then_ImmutablePropertyException()
  {
    CreateOrReplaceCustomizationPayload payload = new()
    {
      Kind = CustomizationKind.Disability,
      Name = " Abruti ",
      Summary = "  Limit�, maladroit et d�savantag� dans l�usage de son intellect.  "
    };

    var exception = await Assert.ThrowsAsync<ImmutablePropertyException<CustomizationKind>>(async () => await _customizationService.CreateOrReplaceAsync(payload, _customization.ResourceId));
    Assert.Equal(Context.WorldUid, exception.WorldId);
    Assert.Equal(Customization.ResourceKind, exception.ResourceKind);
    Assert.Equal(_customization.ResourceId, exception.ResourceId);
    Assert.Equal(_customization.Kind, exception.ExpectedValue);
    Assert.Equal(payload.Kind, exception.AttemptedValue);
    Assert.Equal("Kind", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when creating a customization.")]
  public async Task Given_NotAllowed_When_Create_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder(Faker).Build();

    CreateOrReplaceCustomizationPayload payload = CreateBaraquePayload();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _customizationService.CreateOrReplaceAsync(payload));
    Assert.Equal(Context.ActorId?.Value, exception.Principal);
    Assert.Equal(Actions.CreateCustomization, exception.Action);
    Assert.Null(exception.Resource);
    Assert.Equal(Context.WorldUid, exception.WorldId);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when replacing a customization.")]
  public async Task Given_NotAllowed_When_Replace_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder(Faker).Build();

    CreateOrReplaceCustomizationPayload payload = CreateBaraquePayload();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _customizationService.CreateOrReplaceAsync(payload, _customization.ResourceId));
    Assert.Equal(Context.ActorId?.Value, exception.Principal);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(_customization.Identifier.ToString(), exception.Resource);
    Assert.Equal(Context.WorldUid, exception.WorldId);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when updating a customization.")]
  public async Task Given_NotAllowed_When_Update_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder(Faker).Build();

    UpdateCustomizationPayload payload = new();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _customizationService.UpdateAsync(_customization.ResourceId, payload));
    Assert.Equal(Context.ActorId?.Value, exception.Principal);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(_customization.Identifier.ToString(), exception.Resource);
    Assert.Equal(Context.WorldUid, exception.WorldId);
  }

  [Fact(DisplayName = "It should update an existing customization.")]
  public async Task Given_Exists_When_Update_Then_Updated()
  {
    Guid id = _customization.ResourceId;
    CreateOrReplaceCustomizationPayload create = CreateBaraquePayload();
    UpdateCustomizationPayload payload = new()
    {
      Name = create.Name,
      Summary = new Optional<string>(create.Summary),
      Content = new Optional<string>(create.Content)
    };

    CustomizationModel? customization = await _customizationService.UpdateAsync(id, payload);
    Assert.NotNull(customization);

    Assert.Equal(id, customization.Id);
    Assert.Equal(3, customization.Version);
    Assert.Equal(_customization.CreatedBy, customization.CreatedBy.GetActorId());
    Assert.Equal(_customization.CreatedOn.AsUniversalTime(), customization.CreatedOn, TimeSpan.FromMilliseconds(1));
    Assert.Equal(Actor, customization.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, customization.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Equal(payload.Name.CleanTrim(), customization.Name);
    Assert.Equal(payload.Summary.Value?.CleanTrim(), customization.Summary);
    Assert.Equal(payload.Content.Value?.CleanTrim(), customization.Content);
  }

  private static CreateOrReplaceCustomizationPayload CreateBaraquePayload() => new()
  {
    Kind = CustomizationKind.Gift,
    Name = " Baraqu� ",
    Summary = "  Double port�e, avantage et d�g�ts contre objets et structures.  ",
    Content = "   Le personnage acquiert les capacit�s suivantes :\n\n- Lorsqu�il [bouscule](/regles/combat/activites/bousculer) ou repousse une cr�ature par une capacit� non magique, la distance est doubl�e.\n- Il se voit conf�rer l�[avantage](/regles/competences/tests/avantage-desavantage) � ses [tests](/regles/competences/tests) lorsqu�il tente de briser ou d�[attaquer](/regles/combat/attaque) un [objet](/regles/aventure/interaction-objets), une structure, un b�timent ou une construction non magique.\n- Il double les [points de d�g�ts](/regles/combat/degats) qu�il inflige aux objets, aux structures, aux b�timents ainsi qu�aux constructions non magiques.   "
  };

  private static void AssertBaraque(CreateOrReplaceCustomizationPayload payload, CustomizationModel customization)
  {
    Assert.Equal(payload.Kind, customization.Kind);
    Assert.Equal(payload.Name.CleanTrim(), customization.Name);
    Assert.Equal(payload.Summary?.CleanTrim(), customization.Summary);
    Assert.Equal(payload.Content?.CleanTrim(), customization.Content);
  }
}
