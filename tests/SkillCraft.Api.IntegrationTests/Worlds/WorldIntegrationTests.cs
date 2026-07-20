using Krakenar.Contracts;
using Krakenar.Contracts.Search;
using Logitar;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Builders;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Worlds;
using SkillCraft.Api.Core.Worlds.Models;

namespace SkillCraft.Api.IntegrationTests.Worlds;

[Trait(Traits.Category, Categories.Integration)]
public class WorldIntegrationTests : IntegrationTests
{
  private readonly IWorldRepository _worldRepository;
  private readonly IWorldService _worldService;

  private World _world = null!;

  public WorldIntegrationTests() : base()
  {
    _worldRepository = ServiceProvider.GetRequiredService<IWorldRepository>();
    _worldService = ServiceProvider.GetRequiredService<IWorldService>();
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    _world = new WorldBuilder(Faker).WithOwner(Context.User).WithKey("the-old-world").WithName("The Old World").Build();
    _worldRepository.Add(_world);
    await Context.SaveChangesAsync();
  }

  [Theory(DisplayName = "It should create a new world.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_NotExist_When_CreateOrReplace_Then_Created(bool withId)
  {
    CreateOrReplaceWorldPayload payload = new()
    {
      Key = "The-New-World",
      Name = " The New World ",
      HtmlContent = "  This is the new world.  "
    };
    Guid? id = withId ? Guid.NewGuid() : null;

    CreateOrReplaceWorldResult result = await _worldService.CreateOrReplaceAsync(payload, id);
    Assert.True(result.Created);
    WorldModel world = result.World;
    Assert.NotNull(world);

    if (id.HasValue)
    {
      Assert.Equal(id.Value, world.Id);
    }
    else
    {
      Assert.NotEqual(Guid.Empty, world.Id);
    }
    Assert.Equal(1, world.Version);
    Assert.Equal(Actor, world.CreatedBy);
    Assert.Equal(DateTime.UtcNow, world.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(world.CreatedBy, world.UpdatedBy);
    Assert.Equal(world.CreatedOn, world.UpdatedOn);

    Assert.Equal(SlugHelper.Format(payload.Key), world.Key);
    Assert.Equal(payload.Name?.CleanTrim(), world.Name);
    Assert.Equal(payload.HtmlContent?.CleanTrim(), world.HtmlContent);
  }

  [Fact(DisplayName = "It should read a world by ID.")]
  public async Task Given_Id_When_Read_Then_Read()
  {
    WorldModel? world = await _worldService.ReadAsync(_world.Id);
    Assert.NotNull(world);
    Assert.Equal(_world.Id, world.Id);
  }

  [Fact(DisplayName = "It should read a world by key.")]
  public async Task Given_Key_When_Read_Then_Read()
  {
    WorldModel? world = await _worldService.ReadAsync(id: null, _world.Key);
    Assert.NotNull(world);
    Assert.Equal(_world.Id, world.Id);
  }

  [Fact(DisplayName = "It should replace an existing world.")]
  public async Task Given_Exists_When_CreateOrReplace_Then_Replaced()
  {
    CreateOrReplaceWorldPayload payload = new()
    {
      Key = "The-New-World",
      Name = " The New World ",
      HtmlContent = "  This is the new world.  "
    };
    Guid id = _world.Id;

    CreateOrReplaceWorldResult result = await _worldService.CreateOrReplaceAsync(payload, id);
    Assert.False(result.Created);
    WorldModel world = result.World;
    Assert.NotNull(world);

    Assert.Equal(id, world.Id);
    Assert.Equal(2, world.Version);
    Assert.Equal(_world.CreatedBy, world.CreatedBy.Id);
    Assert.Equal(_world.CreatedOn, world.CreatedOn, TimeSpan.FromMilliseconds(1));
    Assert.Equal(Actor, world.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, world.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Equal(SlugHelper.Format(payload.Key), world.Key);
    Assert.Equal(payload.Name?.CleanTrim(), world.Name);
    Assert.Equal(payload.HtmlContent?.CleanTrim(), world.HtmlContent);
  }

  [Fact(DisplayName = "It should return empty search results.")]
  public async Task Given_NoMatch_When_Search_Then_EmptyResults()
  {
    Context.User = new UserBuilder(Faker).Build();

    SearchWorldsPayload payload = new();

    SearchResults<WorldModel> results = await _worldService.SearchAsync(payload);
    Assert.Equal(0, results.Total);
    Assert.Empty(results.Items);
  }

  [Fact(DisplayName = "It should return null when no world was found.")]
  public async Task Given_NotFound_When_Read_Then_NullReturned()
  {
    Context.User = new UserBuilder(Faker).Build();

    Assert.Null(await _worldService.ReadAsync(Context.WorldId, _world.Key));
  }

  [Fact(DisplayName = "It should return null when the world was not found.")]
  public async Task Given_NotFound_When_Update_Then_NullReturned()
  {
    Assert.Null(await _worldService.UpdateAsync(Guid.Empty, new UpdateWorldPayload()));
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task Given_Matches_When_Search_Then_Results()
  {
    World newWorld = new WorldBuilder(Faker).WithOwner(Context.User).WithKey("the-new-world").Build();
    World anotherWorld = new WorldBuilder(Faker).WithOwner(Context.User).WithKey("another-world").Build();
    _worldRepository.Add(newWorld, anotherWorld);
    await Context.SaveChangesAsync();

    SearchWorldsPayload payload = new()
    {
      Skip = 1,
      Limit = 1
    };
    payload.Search.Terms.Add(new SearchTerm("%world%"));
    payload.Ids.AddRange([Context.WorldId, _world.Id, newWorld.Id, Guid.Empty]);
    payload.Sort.Add(new WorldSortOption(WorldSort.Key, isDescending: true));

    SearchResults<WorldModel> results = await _worldService.SearchAsync(payload);
    Assert.Equal(2, results.Total);

    WorldModel world = Assert.Single(results.Items);
    Assert.Equal(newWorld.Id, world.Id);
  }

  [Fact(DisplayName = "It should throw KeyAlreadyUsedException when creating a world and the key conflicts.")]
  public async Task Given_KeyConflict_When_Create_Then_KeyAlreadyUsedException()
  {
    CreateOrReplaceWorldPayload payload = new()
    {
      Key = _world.Key
    };
    Guid id = Guid.NewGuid();

    var exception = await Assert.ThrowsAsync<KeyAlreadyUsedException>(async () => await _worldService.CreateOrReplaceAsync(payload, id));
    Assert.Null(exception.WorldId);
    Assert.Equal(World.ResourceKind, exception.ResourceKind);
    Assert.Equal(id, exception.ResourceId);
    Assert.Equal(_world.Id, exception.ConflictId);
    Assert.Equal(payload.Key, exception.AttemptedKey);
    Assert.Equal("Key", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw KeyAlreadyUsedException when replacing a world and the key conflicts.")]
  public async Task Given_KeyConflict_When_Replace_Then_KeyAlreadyUsedException()
  {
    CreateOrReplaceWorldPayload payload = new()
    {
      Key = _world.Key
    };
    Guid id = Context.WorldId;

    var exception = await Assert.ThrowsAsync<KeyAlreadyUsedException>(async () => await _worldService.CreateOrReplaceAsync(payload, id));
    Assert.Null(exception.WorldId);
    Assert.Equal(World.ResourceKind, exception.ResourceKind);
    Assert.Equal(id, exception.ResourceId);
    Assert.Equal(_world.Id, exception.ConflictId);
    Assert.Equal(payload.Key, exception.AttemptedKey);
    Assert.Equal("Key", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw KeyAlreadyUsedException when updating a world and the key conflicts.")]
  public async Task Given_KeyConflict_When_Update_Then_KeyAlreadyUsedException()
  {
    UpdateWorldPayload payload = new()
    {
      Key = _world.Key
    };
    Guid id = Context.WorldId;

    var exception = await Assert.ThrowsAsync<KeyAlreadyUsedException>(async () => await _worldService.UpdateAsync(id, payload));
    Assert.Null(exception.WorldId);
    Assert.Equal(World.ResourceKind, exception.ResourceKind);
    Assert.Equal(id, exception.ResourceId);
    Assert.Equal(_world.Id, exception.ConflictId);
    Assert.Equal(payload.Key, exception.AttemptedKey);
    Assert.Equal("Key", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when creating a world.")]
  public async Task Given_NotAllowed_When_Create_Then_PermissionDeniedException()
  {
    World world2 = new WorldBuilder(Faker).WithOwner(Context.User).WithKey("world-2").Build();
    World world3 = new WorldBuilder(Faker).WithOwner(Context.User).WithKey("world-3").Build();
    _worldRepository.Add(world2, world3);
    await Context.SaveChangesAsync();

    CreateOrReplaceWorldPayload payload = new()
    {
      Key = "The-New-World",
      Name = " The New World ",
      HtmlContent = "  This is the new world.  "
    };

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _worldService.CreateOrReplaceAsync(payload));
    Assert.Equal(Context.UserId, exception.UserId);
    Assert.Equal(Actions.CreateWorld, exception.Action);
    Assert.Null(exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when replacing a world.")]
  public async Task Given_NotAllowed_When_Replace_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder(Faker).Build();

    CreateOrReplaceWorldPayload payload = new()
    {
      Key = "The-New-World",
      Name = " The New World ",
      HtmlContent = "  This is the new world.  "
    };

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _worldService.CreateOrReplaceAsync(payload, _world.Id));
    Assert.Equal(Context.UserId, exception.UserId);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(_world.Identifier.ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when udpating a world.")]
  public async Task Given_NotAllowed_When_Update_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder(Faker).Build();

    UpdateWorldPayload payload = new();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _worldService.UpdateAsync(_world.Id, payload));
    Assert.Equal(Context.UserId, exception.UserId);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(_world.Identifier.ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw TooManyResultsException when many worlds were read.")]
  public async Task Given_ManyFound_When_Read_Then_TooManyResultsException()
  {
    var exception = await Assert.ThrowsAsync<TooManyResultsException<WorldModel>>(async () => await _worldService.ReadAsync(Context.WorldId, _world.Key));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(2, exception.ActualCount);
  }

  [Fact(DisplayName = "It should update an existing world.")]
  public async Task Given_Exists_When_Update_Then_Updated()
  {
    Guid id = _world.Id;
    UpdateWorldPayload payload = new()
    {
      Key = "The-New-World",
      Name = new Optional<string>(" The New World "),
      HtmlContent = new Optional<string>("  This is the new world.  ")
    };

    WorldModel? world = await _worldService.UpdateAsync(id, payload);
    Assert.NotNull(world);

    Assert.Equal(id, world.Id);
    Assert.Equal(2, world.Version);
    Assert.Equal(_world.CreatedBy, world.CreatedBy.Id);
    Assert.Equal(_world.CreatedOn, world.CreatedOn, TimeSpan.FromMilliseconds(1));
    Assert.Equal(Actor, world.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, world.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Equal(SlugHelper.Format(payload.Key), world.Key);
    Assert.Equal(payload.Name.Value?.CleanTrim(), world.Name);
    Assert.Equal(payload.HtmlContent.Value?.CleanTrim(), world.HtmlContent);
  }
}
