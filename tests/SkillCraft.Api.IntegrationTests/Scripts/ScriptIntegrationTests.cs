using Krakenar.Contracts.Search;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Contracts;
using SkillCraft.Api.Contracts.Scripts;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Scripts;

namespace SkillCraft.Api.Scripts;

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

    _script = new Script(World, new Name("Script"), UserId);
    await _scriptRepository.SaveAsync(_script);
  }

  [Theory(DisplayName = "It should create a new script.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_NotExist_When_CreateOrReplace_Then_Created(bool withId)
  {
    CreateOrReplaceScriptPayload payload = new()
    {
      Name = "  Rénon  ",
      Summary = "  Alphabet unifié et standardisé, utilisé par le Commun et l’Impérial.  ",
      Description = "  L’alphabet Rénon est un système d’écriture alphabétique commun à l’ensemble du monde Rénon, utilisé aussi bien pour le [Commun](/regles/langues/commun) que pour l’[Impérial](/regles/langues/imperial). Hérité de l’écriture de l’ancien empire occidental, il a été progressivement standardisé afin d’assurer une lecture claire et cohérente sur tout le territoire. Écrit de gauche à droite, il repose sur une relation généralement stable entre les lettres et les sons, tout en conservant certaines conventions historiques. Son apparence a évolué des formes monumentales vers des écritures plus cursives et livresques, et il admet différents styles selon les usages (quotidiens, administratifs ou religieux) sans jamais se fragmenter en alphabets distincts.  "
    };
    Guid? id = withId ? Guid.NewGuid() : null;

    CreateOrReplaceScriptResult result = await _scriptService.CreateOrReplaceAsync(payload, id);
    Assert.True(result.Created);
    ScriptModel script = result.Script;

    if (id.HasValue)
    {
      Assert.Equal(id.Value, script.Id);
    }
    Assert.Equal(2, script.Version);
    Assert.Equal(Actor, script.CreatedBy);
    Assert.Equal(DateTime.UtcNow, script.CreatedOn, TimeSpan.FromSeconds(1));
    Assert.Equal(Actor, script.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, script.UpdatedOn, TimeSpan.FromSeconds(1));

    Assert.Equal(payload.Name.Trim(), script.Name);
    Assert.Equal(payload.Summary.Trim(), script.Summary);
    Assert.Equal(payload.Description.Trim(), script.Description);
  }

  [Fact(DisplayName = "It should delete an existing script.")]
  public async Task Given_Exists_When_Delete_ThenDeleted()
  {
    Guid id = _script.EntityId;
    ScriptModel? script = await _scriptService.DeleteAsync(id);
    Assert.NotNull(script);
    Assert.Equal(id, script.Id);
  }

  [Fact(DisplayName = "It should read an existing script.")]
  public async Task Given_Exists_When_Read_Then_Returned()
  {
    Guid id = _script.EntityId;
    ScriptModel? script = await _scriptService.ReadAsync(id);
    Assert.NotNull(script);
    Assert.Equal(id, script.Id);
  }

  [Fact(DisplayName = "It should replace an existing script.")]
  public async Task Given_Exists_When_CreateOrReplace_Then_Replaced()
  {
    CreateOrReplaceScriptPayload payload = new()
    {
      Name = "  Rénon  ",
      Summary = "  Alphabet unifié et standardisé, utilisé par le Commun et l’Impérial.  ",
      Description = "  L’alphabet Rénon est un système d’écriture alphabétique commun à l’ensemble du monde Rénon, utilisé aussi bien pour le [Commun](/regles/langues/commun) que pour l’[Impérial](/regles/langues/imperial). Hérité de l’écriture de l’ancien empire occidental, il a été progressivement standardisé afin d’assurer une lecture claire et cohérente sur tout le territoire. Écrit de gauche à droite, il repose sur une relation généralement stable entre les lettres et les sons, tout en conservant certaines conventions historiques. Son apparence a évolué des formes monumentales vers des écritures plus cursives et livresques, et il admet différents styles selon les usages (quotidiens, administratifs ou religieux) sans jamais se fragmenter en alphabets distincts.  "
    };
    Guid id = _script.EntityId;

    CreateOrReplaceScriptResult result = await _scriptService.CreateOrReplaceAsync(payload, id);
    Assert.False(result.Created);
    ScriptModel script = result.Script;

    Assert.Equal(id, script.Id);
    Assert.Equal(2, script.Version);
    Assert.Equal(Actor, script.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, script.UpdatedOn, TimeSpan.FromSeconds(1));

    Assert.Equal(payload.Name.Trim(), script.Name);
    Assert.Equal(payload.Summary.Trim(), script.Summary);
    Assert.Equal(payload.Description.Trim(), script.Description);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task Given_Payload_When_Search_Then_Results()
  {
    Script elfique = new(World, new Name("Elfique"), UserId);
    Script renon = new(World, new Name("Rénon"), UserId);
    Script runique = new(World, new Name("Runique"), UserId);
    await _scriptRepository.SaveAsync([elfique, renon, runique]);

    SearchScriptsPayload payload = new()
    {
      Skip = 1,
      Limit = 1
    };
    payload.Ids.AddRange([elfique.EntityId, renon.EntityId, runique.EntityId, Guid.Empty]);
    payload.Search.Operator = SearchOperator.Or;
    payload.Search.Terms.AddRange([new SearchTerm("script"), new SearchTerm("r_n%")]);
    payload.Sort.Add(new ScriptSortOption(ScriptSort.Name));

    SearchResults<ScriptModel> results = await _scriptService.SearchAsync(payload);
    Assert.Equal(2, results.Total);

    ScriptModel result = Assert.Single(results.Items);
    Assert.Equal(runique.EntityId, result.Id);
  }

  [Fact(DisplayName = "It should update an existing script.")]
  public async Task Given_Exists_When_Update_ThenUpdated()
  {
    Guid id = _script.EntityId;
    UpdateScriptPayload payload = new()
    {
      Name = "  Rénon  ",
      Summary = new Update<string>("  Alphabet unifié et standardisé, utilisé par le Commun et l’Impérial.  "),
      Description = new Update<string>("  L’alphabet Rénon est un système d’écriture alphabétique commun à l’ensemble du monde Rénon, utilisé aussi bien pour le [Commun](/regles/langues/commun) que pour l’[Impérial](/regles/langues/imperial). Hérité de l’écriture de l’ancien empire occidental, il a été progressivement standardisé afin d’assurer une lecture claire et cohérente sur tout le territoire. Écrit de gauche à droite, il repose sur une relation généralement stable entre les lettres et les sons, tout en conservant certaines conventions historiques. Son apparence a évolué des formes monumentales vers des écritures plus cursives et livresques, et il admet différents styles selon les usages (quotidiens, administratifs ou religieux) sans jamais se fragmenter en alphabets distincts.  ")
    };

    ScriptModel? script = await _scriptService.UpdateAsync(id, payload);
    Assert.NotNull(script);

    Assert.Equal(id, script.Id);
    Assert.Equal(2, script.Version);
    Assert.Equal(Actor, script.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, script.UpdatedOn, TimeSpan.FromSeconds(1));

    Assert.Equal(payload.Name.Trim(), script.Name);
    Assert.Equal(payload.Summary.Value?.Trim(), script.Summary);
    Assert.Equal(payload.Description.Value?.Trim(), script.Description);
  }
}
