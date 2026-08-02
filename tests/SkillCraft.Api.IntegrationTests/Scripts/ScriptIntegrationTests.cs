using Krakenar.Contracts.Search;
using Logitar;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Builders;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Actors;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Scripts;
using SkillCraft.Api.Core.Scripts.Models;

namespace SkillCraft.Api.IntegrationTests.Scripts;

[Trait(Traits.Category, Categories.Integration)]
public class ScriptIntegrationTests : IntegrationTests
{
  private readonly IScriptRepository _scriptRepository;
  private readonly IScriptService _scriptService;

  private Script _script = null!;

  public ScriptIntegrationTests() : base()
  {
    _scriptRepository = ServiceProvider.GetRequiredService<IScriptRepository>();
    _scriptService = ServiceProvider.GetRequiredService<IScriptService>();
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    _script = new ScriptBuilder(Faker).WithWorld(Context.World).Build();
    await _scriptRepository.SaveAsync(_script);
  }

  [Theory(DisplayName = "It should create a new script.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_NotExist_When_CreateOrReplace_Then_Created(bool withId)
  {
    CreateOrReplaceScriptPayload payload = CreateRenonPayload();
    Guid? id = withId ? Guid.NewGuid() : null;

    CreateOrReplaceScriptResult result = await _scriptService.CreateOrReplaceAsync(payload, id);
    Assert.True(result.Created);
    ScriptModel script = result.Script;
    Assert.NotNull(script);

    if (id.HasValue)
    {
      Assert.Equal(id.Value, script.Id);
    }
    else
    {
      Assert.NotEqual(Guid.Empty, script.Id);
    }
    Assert.Equal(2, script.Version);
    Assert.Equal(Actor, script.CreatedBy);
    Assert.Equal(DateTime.UtcNow, script.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(script.CreatedBy, script.UpdatedBy);
    Assert.True(script.CreatedOn < script.UpdatedOn);

    AssertRenon(payload, script);
  }

  [Fact(DisplayName = "It should read a script by ID.")]
  public async Task Given_Id_When_Read_Then_Read()
  {
    ScriptModel? script = await _scriptService.ReadAsync(_script.ResourceId);
    Assert.NotNull(script);
    Assert.Equal(_script.ResourceId, script.Id);
  }

  [Fact(DisplayName = "It should replace an existing script.")]
  public async Task Given_Exists_When_CreateOrReplace_Then_Replaced()
  {
    CreateOrReplaceScriptPayload payload = CreateRenonPayload();
    Guid id = _script.ResourceId;

    CreateOrReplaceScriptResult result = await _scriptService.CreateOrReplaceAsync(payload, id);
    Assert.False(result.Created);
    ScriptModel script = result.Script;
    Assert.NotNull(script);

    Assert.Equal(id, script.Id);
    Assert.Equal(3, script.Version);
    Assert.Equal(_script.CreatedBy, script.CreatedBy.GetActorId());
    Assert.Equal(_script.CreatedOn.AsUniversalTime(), script.CreatedOn, TimeSpan.FromMilliseconds(1));
    Assert.Equal(Actor, script.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, script.UpdatedOn, TimeSpan.FromSeconds(10));

    AssertRenon(payload, script);
  }

  [Fact(DisplayName = "It should return empty search results.")]
  public async Task Given_NoMatch_When_Search_Then_EmptyResults()
  {
    Context.World = new WorldBuilder(Faker).Build();

    SearchScriptsPayload payload = new();

    SearchResults<ScriptModel> results = await _scriptService.SearchAsync(payload);
    Assert.Equal(0, results.Total);
    Assert.Empty(results.Items);
  }

  [Fact(DisplayName = "It should return null when no script was found.")]
  public async Task Given_NotFound_When_Read_Then_NullReturned()
  {
    Context.World = new WorldBuilder(Faker).Build();

    Assert.Null(await _scriptService.ReadAsync(_script.ResourceId));
  }

  [Fact(DisplayName = "It should return null when the script was not found.")]
  public async Task Given_NotFound_When_Update_Then_NullReturned()
  {
    Assert.Null(await _scriptService.UpdateAsync(Guid.Empty, new UpdateScriptPayload()));
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task Given_Matches_When_Search_Then_Results()
  {
    Script elfique = new ScriptBuilder(Faker).WithWorld(Context.World).WithName("Elfique").Build();
    Script montagnard = new ScriptBuilder(Faker).WithWorld(Context.World).WithName("Montagnard").Build();
    Script orrinique = new ScriptBuilder(Faker).WithWorld(Context.World).WithName("Orrinique").Build();
    await _scriptRepository.SaveAsync([elfique, montagnard, orrinique]);

    SearchScriptsPayload payload = new()
    {
      Skip = 1,
      Limit = 1
    };
    payload.Search.Terms.Add(new SearchTerm("%i%"));
    payload.Ids.AddRange([_script.ResourceId, Guid.Empty, montagnard.ResourceId, orrinique.ResourceId]);
    payload.Sort.Add(new ScriptSortOption(ScriptSort.Name, isDescending: true));

    SearchResults<ScriptModel> results = await _scriptService.SearchAsync(payload);
    Assert.Equal(2, results.Total);

    ScriptModel script = Assert.Single(results.Items);
    Assert.Equal(orrinique.ResourceId, script.Id);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when creating a script.")]
  public async Task Given_NotAllowed_When_Create_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder(Faker).Build();

    CreateOrReplaceScriptPayload payload = CreateRenonPayload();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _scriptService.CreateOrReplaceAsync(payload));
    Assert.Equal(Context.UserUid, exception.UserId);
    Assert.Equal(Actions.CreateScript, exception.Action);
    Assert.Equal(Context.World?.Identifier.ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when replacing a script.")]
  public async Task Given_NotAllowed_When_Replace_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder(Faker).Build();

    CreateOrReplaceScriptPayload payload = CreateRenonPayload();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _scriptService.CreateOrReplaceAsync(payload, _script.ResourceId));
    Assert.Equal(Context.UserUid, exception.UserId);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(_script.Identifier.ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when updating a script.")]
  public async Task Given_NotAllowed_When_Update_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder(Faker).Build();

    UpdateScriptPayload payload = new();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _scriptService.UpdateAsync(_script.ResourceId, payload));
    Assert.Equal(Context.UserUid, exception.UserId);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(_script.Identifier.ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should update an existing script.")]
  public async Task Given_Exists_When_Update_Then_Updated()
  {
    Guid id = _script.ResourceId;
    CreateOrReplaceScriptPayload create = CreateRenonPayload();
    UpdateScriptPayload payload = new()
    {
      Name = create.Name,
      Summary = new Optional<string>(create.Summary),
      Content = new Optional<string>(create.Content)
    };

    ScriptModel? script = await _scriptService.UpdateAsync(id, payload);
    Assert.NotNull(script);

    Assert.Equal(id, script.Id);
    Assert.Equal(3, script.Version);
    Assert.Equal(_script.CreatedBy, script.CreatedBy.GetActorId());
    Assert.Equal(_script.CreatedOn.AsUniversalTime(), script.CreatedOn, TimeSpan.FromMilliseconds(1));
    Assert.Equal(Actor, script.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, script.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Equal(payload.Name.CleanTrim(), script.Name);
    Assert.Equal(payload.Summary.Value?.CleanTrim(), script.Summary);
    Assert.Equal(payload.Content.Value?.CleanTrim(), script.Content);
  }

  private static CreateOrReplaceScriptPayload CreateRenonPayload() => new()
  {
    Name = " Rénon ",
    Summary = "  Alphabet unifié et standardisé, utilisé par le Commun et l’Impérial.  ",
    Content = "   L’alphabet Rénon est un système d’écriture alphabétique commun à l’ensemble du monde Rénon, utilisé aussi bien pour le [Commun](/regles/langues/commun) que pour l’[Impérial](/regles/langues/imperial). Hérité de l’écriture de l’ancien empire occidental, il a été progressivement standardisé afin d’assurer une lecture claire et cohérente sur tout le territoire. Écrit de gauche à droite, il repose sur une relation généralement stable entre les lettres et les sons, tout en conservant certaines conventions historiques. Son apparence a évolué des formes monumentales vers des écritures plus cursives et livresques, et il admet différents styles selon les usages (quotidiens, administratifs ou religieux) sans jamais se fragmenter en alphabets distincts.   "
  };

  private static void AssertRenon(CreateOrReplaceScriptPayload payload, ScriptModel script)
  {
    Assert.Equal(payload.Name.CleanTrim(), script.Name);
    Assert.Equal(payload.Summary?.CleanTrim(), script.Summary);
    Assert.Equal(payload.Content?.CleanTrim(), script.Content);
  }
}
