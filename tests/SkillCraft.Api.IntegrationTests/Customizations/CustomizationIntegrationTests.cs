using Krakenar.Contracts.Search;
using Logitar;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Builders;
using SkillCraft.Api.Core;
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
    _customizationRepository.Add(_customization);
    await Context.SaveChangesAsync();
  }

  [Theory(DisplayName = "It should create a new customization.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_NotExist_When_CreateOrReplace_Then_Created(bool withId)
  {
    CreateOrReplaceCustomizationPayload payload = new()
    {
      Kind = CustomizationKind.Gift,
      Name = " Baraqué ",
      Description = "  Double portée, avantage et dégâts contre objets et structures.  "
    };
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
    Assert.Equal(1, customization.Version);
    Assert.Equal(Actor, customization.CreatedBy);
    Assert.Equal(DateTime.UtcNow, customization.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(customization.CreatedBy, customization.UpdatedBy);
    Assert.Equal(customization.CreatedOn, customization.UpdatedOn);

    Assert.Equal(payload.Kind, customization.Kind);
    Assert.Equal(payload.Name.CleanTrim(), customization.Name);
    Assert.Equal(payload.Description?.CleanTrim(), customization.Description);
  }

  [Fact(DisplayName = "It should read a customization by ID.")]
  public async Task Given_Id_When_Read_Then_Read()
  {
    CustomizationModel? customization = await _customizationService.ReadAsync(_customization.Id);
    Assert.NotNull(customization);
    Assert.Equal(_customization.Id, customization.Id);
  }

  [Fact(DisplayName = "It should replace an existing customization.")]
  public async Task Given_Exists_When_CreateOrReplace_Then_Replaced()
  {
    CreateOrReplaceCustomizationPayload payload = new()
    {
      Kind = CustomizationKind.Gift,
      Name = " Baraqué ",
      Description = "  Double portée, avantage et dégâts contre objets et structures.  "
    };
    Guid id = _customization.Id;

    CreateOrReplaceCustomizationResult result = await _customizationService.CreateOrReplaceAsync(payload, id);
    Assert.False(result.Created);
    CustomizationModel customization = result.Customization;
    Assert.NotNull(customization);

    Assert.Equal(id, customization.Id);
    Assert.Equal(2, customization.Version);
    Assert.Equal(_customization.CreatedBy, customization.CreatedBy.Id);
    Assert.Equal(_customization.CreatedOn, customization.CreatedOn, TimeSpan.FromMilliseconds(1));
    Assert.Equal(Actor, customization.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, customization.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Equal(_customization.Kind, customization.Kind);
    Assert.Equal(payload.Name.CleanTrim(), customization.Name);
    Assert.Equal(payload.Description?.CleanTrim(), customization.Description);
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

    Assert.Null(await _customizationService.ReadAsync(_customization.Id));
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
    Customization adresseLegendaire = new CustomizationBuilder(Faker).WithWorld(Context.World).WithKind(CustomizationKind.Gift).WithName("Adresse légendaire").Build();
    Customization affiniteAnimale = new CustomizationBuilder(Faker).WithWorld(Context.World).WithKind(CustomizationKind.Gift).WithName("Affinité animale").Build();
    Customization baraque = new CustomizationBuilder(Faker).WithWorld(Context.World).WithKind(CustomizationKind.Gift).WithName("Baraqué").Build();
    _customizationRepository.Add(abruti, adresseLegendaire, affiniteAnimale, baraque);
    await Context.SaveChangesAsync();

    SearchCustomizationsPayload payload = new()
    {
      Kind = CustomizationKind.Gift,
      Skip = 1,
      Limit = 1
    };
    payload.Search.Operator = SearchOperator.Or;
    payload.Search.Terms.Add(new SearchTerm("%b%"));
    payload.Search.Terms.Add(new SearchTerm("%l%"));
    payload.Ids.AddRange([_customization.Id, abruti.Id, adresseLegendaire.Id, Guid.Empty, baraque.Id]);
    payload.Sort.Add(new CustomizationSortOption(CustomizationSort.Name));

    SearchResults<CustomizationModel> results = await _customizationService.SearchAsync(payload);
    Assert.Equal(2, results.Total);

    CustomizationModel customization = Assert.Single(results.Items);
    Assert.Equal(baraque.Id, customization.Id);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when creating a customization.")]
  public async Task Given_NotAllowed_When_Create_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder(Faker).Build();

    CreateOrReplaceCustomizationPayload payload = new()
    {
      Kind = CustomizationKind.Gift,
      Name = " Baraqué ",
      Description = "  Double portée, avantage et dégâts contre objets et structures.  "
    };

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _customizationService.CreateOrReplaceAsync(payload));
    Assert.Equal(Context.UserId, exception.UserId);
    Assert.Equal(Actions.CreateCustomization, exception.Action);
    Assert.Equal(Context.World?.Identifier.ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when replacing a customization.")]
  public async Task Given_NotAllowed_When_Replace_Then_PermissionDeniedException()
  {
    Context.World = new WorldBuilder(Faker).Build();

    CreateOrReplaceCustomizationPayload payload = new()
    {
      Kind = CustomizationKind.Gift,
      Name = " Baraqué ",
      Description = "  Double portée, avantage et dégâts contre objets et structures.  "
    };

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _customizationService.CreateOrReplaceAsync(payload, _customization.Id));
    Assert.Equal(Context.UserId, exception.UserId);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(_customization.Identifier.ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when updating a customization.")]
  public async Task Given_NotAllowed_When_Update_Then_PermissionDeniedException()
  {
    Context.World = new WorldBuilder(Faker).Build();

    UpdateCustomizationPayload payload = new();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _customizationService.UpdateAsync(_customization.Id, payload));
    Assert.Equal(Context.UserId, exception.UserId);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(_customization.Identifier.ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should update an existing customization.")]
  public async Task Given_Exists_When_Update_Then_Updated()
  {
    Guid id = _customization.Id;
    UpdateCustomizationPayload payload = new()
    {
      Name = " Baraqué ",
      Description = new Optional<string>("  Double portée, avantage et dégâts contre objets et structures.  ")
    };

    CustomizationModel? customization = await _customizationService.UpdateAsync(id, payload);
    Assert.NotNull(customization);

    Assert.Equal(id, customization.Id);
    Assert.Equal(2, customization.Version);
    Assert.Equal(_customization.CreatedBy, customization.CreatedBy.Id);
    Assert.Equal(_customization.CreatedOn, customization.CreatedOn, TimeSpan.FromMilliseconds(1));
    Assert.Equal(Actor, customization.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, customization.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Equal(payload.Name.CleanTrim(), customization.Name);
    Assert.Equal(payload.Description.Value?.CleanTrim(), customization.Description);
  }
}
