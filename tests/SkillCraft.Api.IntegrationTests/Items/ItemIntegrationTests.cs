using Krakenar.Contracts.Search;
using Logitar;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Builders;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Actors;
using SkillCraft.Api.Core.Items;
using SkillCraft.Api.Core.Items.Models;
using SkillCraft.Api.Core.Permissions;

namespace SkillCraft.Api.IntegrationTests.Items;

[Trait(Traits.Category, Categories.Integration)]
public class ItemIntegrationTests : IntegrationTests
{
  private readonly IItemRepository _itemRepository;
  private readonly IItemService _itemService;

  private Item _item = null!;

  public ItemIntegrationTests() : base()
  {
    _itemRepository = ServiceProvider.GetRequiredService<IItemRepository>();
    _itemService = ServiceProvider.GetRequiredService<IItemService>();
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    _item = new ItemBuilder(Faker).WithWorld(Context.World).Build();
    await _itemRepository.SaveAsync(_item);
  }

  [Theory(DisplayName = "It should create a new item.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_NotExist_When_CreateOrReplace_Then_Created(bool withId)
  {
    CreateOrReplaceItemPayload payload = CreateCordePayload();
    Guid? id = withId ? Guid.NewGuid() : null;

    CreateOrReplaceItemResult result = await _itemService.CreateOrReplaceAsync(payload, id);
    Assert.True(result.Created);
    ItemModel item = result.Item;
    Assert.NotNull(item);

    if (id.HasValue)
    {
      Assert.Equal(id.Value, item.Id);
    }
    else
    {
      Assert.NotEqual(Guid.Empty, item.Id);
    }
    Assert.Equal(3, item.Version);
    Assert.Equal(Actor, item.CreatedBy);
    Assert.Equal(DateTime.UtcNow, item.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(item.CreatedBy, item.UpdatedBy);
    Assert.True(item.CreatedOn < item.UpdatedOn);

    AssertCorde(payload, item);
  }

  [Fact(DisplayName = "It should read an item by ID.")]
  public async Task Given_Id_When_Read_Then_Read()
  {
    ItemModel? item = await _itemService.ReadAsync(_item.ResourceId);
    Assert.NotNull(item);
    Assert.Equal(_item.ResourceId, item.Id);
  }

  [Fact(DisplayName = "It should replace an existing item.")]
  public async Task Given_Exists_When_CreateOrReplace_Then_Replaced()
  {
    CreateOrReplaceItemPayload payload = CreateCordePayload();
    Guid id = _item.ResourceId;

    CreateOrReplaceItemResult result = await _itemService.CreateOrReplaceAsync(payload, id);
    Assert.False(result.Created);
    ItemModel item = result.Item;
    Assert.NotNull(item);

    Assert.Equal(id, item.Id);
    Assert.Equal(4, item.Version);
    Assert.Equal(_item.CreatedBy, item.CreatedBy.GetActorId());
    Assert.Equal(_item.CreatedOn.AsUniversalTime(), item.CreatedOn, TimeSpan.FromMilliseconds(1));
    Assert.Equal(Actor, item.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, item.UpdatedOn, TimeSpan.FromSeconds(10));

    AssertCorde(payload, item);
  }

  [Fact(DisplayName = "It should return empty search results.")]
  public async Task Given_NoMatch_When_Search_Then_EmptyResults()
  {
    Context.World = new WorldBuilder(Faker).Build();

    SearchItemsPayload payload = new();

    SearchResults<ItemModel> results = await _itemService.SearchAsync(payload);
    Assert.Equal(0, results.Total);
    Assert.Empty(results.Items);
  }

  [Fact(DisplayName = "It should return null when no item was found.")]
  public async Task Given_NotFound_When_Read_Then_NullReturned()
  {
    Context.World = new WorldBuilder(Faker).Build();

    Assert.Null(await _itemService.ReadAsync(_item.ResourceId));
  }

  [Fact(DisplayName = "It should return null when the item was not found.")]
  public async Task Given_NotFound_When_Update_Then_NullReturned()
  {
    Assert.Null(await _itemService.UpdateAsync(Guid.Empty, new UpdateItemPayload()));
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task Given_Matches_When_Search_Then_Results()
  {
    Item abaque = ItemBuilder.Abaque(Faker, Context.World);
    Item torche = ItemBuilder.Torche(Faker, Context.World);
    Item piedDeBiche = ItemBuilder.PiedDeBiche(Faker, Context.World);
    Item grimoire = ItemBuilder.Grimoire(Faker, Context.World);
    await _itemRepository.SaveAsync([abaque, torche, piedDeBiche, grimoire]);

    SearchItemsPayload payload = new()
    {
      Skip = 1,
      Limit = 1
    };
    payload.Search.Terms.Add(new SearchTerm("%uti%"));
    payload.Ids.AddRange([abaque.ResourceId, Guid.Empty, piedDeBiche.ResourceId, grimoire.ResourceId]);
    payload.Sort.Add(new ItemSortOption(ItemSort.Name, isDescending: true));

    SearchResults<ItemModel> results = await _itemService.SearchAsync(payload);
    Assert.Equal(2, results.Total);

    ItemModel item = Assert.Single(results.Items);
    Assert.Equal(abaque.ResourceId, item.Id);
  }

  [Fact(DisplayName = "It should sort items by price with nulls last.")]
  public async Task Given_NullPrices_When_SearchByPrice_Then_NullsLast()
  {
    Item corde = ItemBuilder.Corde(Faker, Context.World);
    Item lanterne = ItemBuilder.Lanterne(Faker, Context.World);
    Item sansPrix = new ItemBuilder(Faker).WithWorld(Context.World).WithName("Amulette sans prix").Build();
    await _itemRepository.SaveAsync([corde, lanterne, sansPrix]);

    SearchItemsPayload payload = new();
    payload.Ids.AddRange([corde.ResourceId, lanterne.ResourceId, sansPrix.ResourceId]);
    payload.Sort.Add(new ItemSortOption(ItemSort.Price, isDescending: true));

    SearchResults<ItemModel> results = await _itemService.SearchAsync(payload);
    Assert.Equal(3, results.Total);
    Assert.Equal(3, results.Items.Count);

    Assert.Equal(lanterne.ResourceId, results.Items.ElementAt(0).Id);
    Assert.Equal(corde.ResourceId, results.Items.ElementAt(1).Id);
    Assert.Equal(sansPrix.ResourceId, results.Items.ElementAt(2).Id);
    Assert.Null(results.Items.ElementAt(2).Price);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when creating an item.")]
  public async Task Given_NotAllowed_When_Create_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder(Faker).Build();

    CreateOrReplaceItemPayload payload = CreateCordePayload();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _itemService.CreateOrReplaceAsync(payload));
    Assert.Equal(Context.ActorId?.Value, exception.Principal);
    Assert.Equal(Actions.CreateItem, exception.Action);
    Assert.Null(exception.Resource);
    Assert.Equal(Context.WorldUid, exception.WorldId);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when replacing an item.")]
  public async Task Given_NotAllowed_When_Replace_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder(Faker).Build();

    CreateOrReplaceItemPayload payload = CreateCordePayload();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _itemService.CreateOrReplaceAsync(payload, _item.ResourceId));
    Assert.Equal(Context.ActorId?.Value, exception.Principal);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(_item.Identifier.ToString(), exception.Resource);
    Assert.Equal(Context.WorldUid, exception.WorldId);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when updating an item.")]
  public async Task Given_NotAllowed_When_Update_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder(Faker).Build();

    UpdateItemPayload payload = new();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _itemService.UpdateAsync(_item.ResourceId, payload));
    Assert.Equal(Context.ActorId?.Value, exception.Principal);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(_item.Identifier.ToString(), exception.Resource);
    Assert.Equal(Context.WorldUid, exception.WorldId);
  }

  [Fact(DisplayName = "It should update an existing item.")]
  public async Task Given_Exists_When_Update_Then_Updated()
  {
    Guid id = _item.ResourceId;
    CreateOrReplaceItemPayload create = CreateCordePayload();
    UpdateItemPayload payload = new()
    {
      Name = create.Name,
      Summary = new Optional<string>(create.Summary),
      Content = new Optional<string>(create.Content),
      Price = new Optional<double?>(create.Price),
      Weight = new Optional<double?>(create.Weight)
    };

    ItemModel? item = await _itemService.UpdateAsync(id, payload);
    Assert.NotNull(item);

    Assert.Equal(id, item.Id);
    Assert.Equal(4, item.Version);
    Assert.Equal(_item.CreatedBy, item.CreatedBy.GetActorId());
    Assert.Equal(_item.CreatedOn.AsUniversalTime(), item.CreatedOn, TimeSpan.FromMilliseconds(1));
    Assert.Equal(Actor, item.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, item.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Equal(payload.Name.CleanTrim(), item.Name);
    Assert.Equal(payload.Summary.Value?.CleanTrim(), item.Summary);
    Assert.Equal(payload.Content.Value?.CleanTrim(), item.Content);
    Assert.Equal(payload.Price.Value, item.Price);
    Assert.Equal(payload.Weight.Value, item.Weight);
  }

  private static CreateOrReplaceItemPayload CreateCordePayload() => new()
  {
    Name = " Corde (15 mètres) ",
    Summary = "  Corde de chanvre de 15 mètres, 2 points de Vitalité.  ",
    Content = "   Une corde de chanvre dotée de 2 points de Vitalité. On peut la briser en réussissant un test d’Athlétisme de difficulté élevée. La longueur standard est de 15 mètres.   ",
    Price = 1,
    Weight = 5
  };

  private static void AssertCorde(CreateOrReplaceItemPayload payload, ItemModel item)
  {
    Assert.Equal(payload.Name.CleanTrim(), item.Name);
    Assert.Equal(payload.Summary?.CleanTrim(), item.Summary);
    Assert.Equal(payload.Content?.CleanTrim(), item.Content);
    Assert.Equal(payload.Price, item.Price);
    Assert.Equal(payload.Weight, item.Weight);
  }
}
