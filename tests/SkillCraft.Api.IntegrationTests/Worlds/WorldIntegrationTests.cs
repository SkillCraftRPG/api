using Krakenar.Contracts.Search;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Contracts;
using SkillCraft.Api.Contracts.Worlds;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Worlds;

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

    _world = new World(UserId, new Name("Hyrule"));
    await _worldRepository.SaveAsync(_world);
  }

  [Theory(DisplayName = "It should create a new world.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_NotExist_When_CreateOrReplace_Then_Created(bool withId)
  {
    CreateOrReplaceWorldPayload payload = new()
    {
      Name = "  Hyrule  ",
      Description = "  I went hiking and found a lake. It was quite a surprise for me to stumble upon it. When I traveled around the country without a map, trying to find my way, stumbling on amazing things as I went, I realized how it felt to go on an adventure like this.  "
    };
    Guid? id = withId ? Guid.NewGuid() : null;

    CreateOrReplaceWorldResult result = await _worldService.CreateOrReplaceAsync(payload, id);
    Assert.True(result.Created);
    WorldModel world = result.World;

    if (id.HasValue)
    {
      Assert.Equal(id.Value, world.Id);
    }
    Assert.Equal(2, world.Version);
    Assert.Equal(Actor, world.CreatedBy);
    Assert.Equal(DateTime.UtcNow, world.CreatedOn, TimeSpan.FromSeconds(1));
    Assert.Equal(Actor, world.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, world.UpdatedOn, TimeSpan.FromSeconds(1));

    Assert.Equal(payload.Name.Trim(), world.Name);
    Assert.Equal(payload.Description.Trim(), world.Description);
  }

  [Fact(DisplayName = "It should delete an existing world.")]
  public async Task Given_Exists_When_Delete_ThenDeleted()
  {
    Guid id = _world.Id.ToGuid();
    WorldModel? world = await _worldService.DeleteAsync(id);
    Assert.NotNull(world);
    Assert.Equal(id, world.Id);
  }

  [Fact(DisplayName = "It should read an existing world.")]
  public async Task Given_Exists_When_Read_Then_Returned()
  {
    Guid id = _world.Id.ToGuid();
    WorldModel? world = await _worldService.ReadAsync(id);
    Assert.NotNull(world);
    Assert.Equal(id, world.Id);
  }

  [Fact(DisplayName = "It should replace an existing world.")]
  public async Task Given_Exists_When_CreateOrReplace_Then_Replaced()
  {
    CreateOrReplaceWorldPayload payload = new()
    {
      Name = "  Hyrule  ",
      Description = "  I went hiking and found a lake. It was quite a surprise for me to stumble upon it. When I traveled around the country without a map, trying to find my way, stumbling on amazing things as I went, I realized how it felt to go on an adventure like this.  "
    };
    Guid id = _world.Id.ToGuid();

    CreateOrReplaceWorldResult result = await _worldService.CreateOrReplaceAsync(payload, id);
    Assert.False(result.Created);
    WorldModel world = result.World;

    Assert.Equal(id, world.Id);
    Assert.Equal(2, world.Version);
    Assert.Equal(Actor, world.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, world.UpdatedOn, TimeSpan.FromSeconds(1));

    Assert.Equal(payload.Name.Trim(), world.Name);
    Assert.Equal(payload.Description.Trim(), world.Description);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task Given_Payload_When_Search_Then_Results()
  {
    SearchWorldsPayload payload = new()
    {
      Skip = 1,
      Limit = 1
    };
    payload.Sort.Add(new WorldSortOption(WorldSort.Name));

    SearchResults<WorldModel> results = await _worldService.SearchAsync(payload);
    Assert.Equal(2, results.Total);

    WorldModel result = Assert.Single(results.Items);
    Assert.Equal(World.Id.ToGuid(), result.Id);
  }

  [Fact(DisplayName = "It should update an existing world.")]
  public async Task Given_Exists_When_Update_ThenUpdated()
  {
    Guid id = _world.Id.ToGuid();
    UpdateWorldPayload payload = new()
    {
      Name = "  Hyrule  ",
      Description = new Update<string>("  I went hiking and found a lake. It was quite a surprise for me to stumble upon it. When I traveled around the country without a map, trying to find my way, stumbling on amazing things as I went, I realized how it felt to go on an adventure like this.  ")
    };

    WorldModel? world = await _worldService.UpdateAsync(id, payload);
    Assert.NotNull(world);

    Assert.Equal(id, world.Id);
    Assert.Equal(2, world.Version);
    Assert.Equal(Actor, world.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, world.UpdatedOn, TimeSpan.FromSeconds(1));

    Assert.Equal(payload.Name.Trim(), world.Name);
    Assert.Equal(payload.Description.Value?.Trim(), world.Description);
  }
}
