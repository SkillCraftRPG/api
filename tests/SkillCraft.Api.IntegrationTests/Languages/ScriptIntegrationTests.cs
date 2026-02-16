using Krakenar.Contracts.Search;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Contracts;
using SkillCraft.Api.Contracts.Languages;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Scripts;

namespace SkillCraft.Api.Languages;

[Trait(Traits.Category, Categories.Integration)]
public class LanguageIntegrationTests : IntegrationTests
{
  private readonly ILanguageRepository _languageRepository;
  private readonly ILanguageService _languageService;
  private readonly IScriptRepository _scriptRepository;

  private Script _renon = null!;
  private Language _language = null!;

  public LanguageIntegrationTests() : base()
  {
    _languageRepository = ServiceProvider.GetRequiredService<ILanguageRepository>();
    _languageService = ServiceProvider.GetRequiredService<ILanguageService>();
    _scriptRepository = ServiceProvider.GetRequiredService<IScriptRepository>();
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    _renon = new Script(World, new Name("Rénon"), UserId);
    await _scriptRepository.SaveAsync(_renon);

    _language = new Language(World, new Name("Language"), UserId);
    await _languageRepository.SaveAsync(_language);
  }

  [Theory(DisplayName = "It should create a new language.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_NotExist_When_CreateOrReplace_Then_Created(bool withId)
  {
    CreateOrReplaceLanguagePayload payload = new()
    {
      Name = "  Commun  ",
      Summary = "  Langue véhiculaire pragmatique et évolutive, parlée sur tout Ouespéro.  ",
      Description = "  Le Rénon commun, souvent abrégé en _Commun_, est la langue véhiculaire la plus répandue sur le continent d’Ouespéro. Héritier direct de la langue populaire de l’ancien empire occidental, il s’est imposé comme langue du commerce, de la diplomatie et des échanges quotidiens, en particulier dans l’Ouest et le Sud du continent. Il est parlé sous six grands dialectes régionaux, mutuellement intelligibles à l’oral. Tous utilisent le même alphabet, mais diffèrent par leurs conventions orthographiques, leurs choix graphiques et leurs traditions scribales.\n\nLe Rénon commun est une langue fonctionnelle, pragmatique et évolutive, issue de la langue parlée plutôt que de la norme savante. Il privilégie l’efficacité communicative et l’intercompréhension entre peuples d’origines diverses. Il est parfaitement adapté aux usages quotidiens, commerciaux et diplomatiques, mais reste peu apte à exprimer des concepts abstraits complexes sans périphrases. Ses traits généraux incluent :\n\n- une grammaire simplifiée par rapport à la [langue impériale ancienne](/regles/langues/imperial),\n- une syntaxe plus stable, généralement sujet–verbe–objet,\n- un affaiblissement des flexions anciennes, compensé par l’usage accru de prépositions,\n- un vocabulaire composite mêlant héritage impérial, innovations populaires et emprunts régionaux.  ",
      ScriptId = _renon.EntityId,
      TypicalSpeakers = "Dans les régions anciennement impériales, il est souvent langue première. Ailleurs, il est presque toujours une langue seconde, apprise pour des raisons pratiques. Même lorsque la maîtrise est imparfaite, l’intercompréhension reste généralement suffisante. Le Rénon commun est parlé par :\r\n\r\n- la majorité des populations urbaines et côtières,\r\n- les marchands, diplomates et voyageurs,\r\n- de nombreuses communautés locales multilingues."
    };
    Guid? id = withId ? Guid.NewGuid() : null;

    CreateOrReplaceLanguageResult result = await _languageService.CreateOrReplaceAsync(payload, id);
    Assert.True(result.Created);
    LanguageModel language = result.Language;

    if (id.HasValue)
    {
      Assert.Equal(id.Value, language.Id);
    }
    Assert.Equal(2, language.Version);
    Assert.Equal(Actor, language.CreatedBy);
    Assert.Equal(DateTime.UtcNow, language.CreatedOn, TimeSpan.FromSeconds(1));
    Assert.Equal(Actor, language.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, language.UpdatedOn, TimeSpan.FromSeconds(1));

    Assert.Equal(payload.Name.Trim(), language.Name);
    Assert.Equal(payload.Summary.Trim(), language.Summary);
    Assert.Equal(payload.Description.Trim(), language.Description);
    Assert.Equal(payload.ScriptId, language.Script?.Id);
    Assert.Equal(payload.TypicalSpeakers.Trim(), language.TypicalSpeakers);
  }

  [Fact(DisplayName = "It should delete an existing language.")]
  public async Task Given_Exists_When_Delete_ThenDeleted()
  {
    Guid id = _language.EntityId;
    LanguageModel? language = await _languageService.DeleteAsync(id);
    Assert.NotNull(language);
    Assert.Equal(id, language.Id);
  }

  [Fact(DisplayName = "It should read an existing language.")]
  public async Task Given_Exists_When_Read_Then_Returned()
  {
    Guid id = _language.EntityId;
    LanguageModel? language = await _languageService.ReadAsync(id);
    Assert.NotNull(language);
    Assert.Equal(id, language.Id);
  }

  [Fact(DisplayName = "It should replace an existing language.")]
  public async Task Given_Exists_When_CreateOrReplace_Then_Replaced()
  {
    CreateOrReplaceLanguagePayload payload = new()
    {
      Name = "  Commun  ",
      Summary = "  Langue véhiculaire pragmatique et évolutive, parlée sur tout Ouespéro.  ",
      Description = "  Le Rénon commun, souvent abrégé en _Commun_, est la langue véhiculaire la plus répandue sur le continent d’Ouespéro. Héritier direct de la langue populaire de l’ancien empire occidental, il s’est imposé comme langue du commerce, de la diplomatie et des échanges quotidiens, en particulier dans l’Ouest et le Sud du continent. Il est parlé sous six grands dialectes régionaux, mutuellement intelligibles à l’oral. Tous utilisent le même alphabet, mais diffèrent par leurs conventions orthographiques, leurs choix graphiques et leurs traditions scribales.\n\nLe Rénon commun est une langue fonctionnelle, pragmatique et évolutive, issue de la langue parlée plutôt que de la norme savante. Il privilégie l’efficacité communicative et l’intercompréhension entre peuples d’origines diverses. Il est parfaitement adapté aux usages quotidiens, commerciaux et diplomatiques, mais reste peu apte à exprimer des concepts abstraits complexes sans périphrases. Ses traits généraux incluent :\n\n- une grammaire simplifiée par rapport à la [langue impériale ancienne](/regles/langues/imperial),\n- une syntaxe plus stable, généralement sujet–verbe–objet,\n- un affaiblissement des flexions anciennes, compensé par l’usage accru de prépositions,\n- un vocabulaire composite mêlant héritage impérial, innovations populaires et emprunts régionaux.  ",
      ScriptId = _renon.EntityId,
      TypicalSpeakers = "Dans les régions anciennement impériales, il est souvent langue première. Ailleurs, il est presque toujours une langue seconde, apprise pour des raisons pratiques. Même lorsque la maîtrise est imparfaite, l’intercompréhension reste généralement suffisante. Le Rénon commun est parlé par :\r\n\r\n- la majorité des populations urbaines et côtières,\r\n- les marchands, diplomates et voyageurs,\r\n- de nombreuses communautés locales multilingues."
    };
    Guid id = _language.EntityId;

    CreateOrReplaceLanguageResult result = await _languageService.CreateOrReplaceAsync(payload, id);
    Assert.False(result.Created);
    LanguageModel language = result.Language;

    Assert.Equal(id, language.Id);
    Assert.Equal(2, language.Version);
    Assert.Equal(Actor, language.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, language.UpdatedOn, TimeSpan.FromSeconds(1));

    Assert.Equal(payload.Name.Trim(), language.Name);
    Assert.Equal(payload.Summary.Trim(), language.Summary);
    Assert.Equal(payload.Description.Trim(), language.Description);
    Assert.Equal(payload.ScriptId, language.Script?.Id);
    Assert.Equal(payload.TypicalSpeakers.Trim(), language.TypicalSpeakers);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task Given_Payload_When_Search_Then_Results()
  {
    Language commun = new(World, new Name("Commun"), UserId);
    commun.SetScript(_renon);
    commun.Update(UserId);
    Language harseme = new(World, new Name("Harsème"), UserId);
    harseme.SetScript(_renon);
    harseme.Update(UserId);
    Language imperial = new(World, new Name("Impérial"), UserId);
    imperial.SetScript(_renon);
    imperial.Update(UserId);
    Language wisgorne = new(World, new Name("Wisgorne"), UserId);
    wisgorne.SetScript(_renon);
    wisgorne.Update(UserId);
    await _languageRepository.SaveAsync([commun, harseme, imperial, wisgorne]);

    SearchLanguagesPayload payload = new()
    {
      Script = new ScriptFilter(_renon.EntityId),
      Skip = 1,
      Limit = 1
    };
    payload.Ids.AddRange([_language.EntityId, commun.EntityId, imperial.EntityId, wisgorne.EntityId, Guid.Empty]);
    payload.Search.Operator = SearchOperator.Or;
    payload.Search.Terms.AddRange([new SearchTerm("Lang%"), new SearchTerm("%m%")]);
    payload.Sort.Add(new LanguageSortOption(LanguageSort.Name, isDescending: true));

    SearchResults<LanguageModel> results = await _languageService.SearchAsync(payload);
    Assert.Equal(2, results.Total);

    LanguageModel result = Assert.Single(results.Items);
    Assert.Equal(commun.EntityId, result.Id);
  }

  [Fact(DisplayName = "It should update an existing language.")]
  public async Task Given_Exists_When_Update_ThenUpdated()
  {
    Guid id = _language.EntityId;
    UpdateLanguagePayload payload = new()
    {
      Name = "  Commun  ",
      Summary = new Update<string>("  Langue véhiculaire pragmatique et évolutive, parlée sur tout Ouespéro.  "),
      Description = new Update<string>("  Le Rénon commun, souvent abrégé en _Commun_, est la langue véhiculaire la plus répandue sur le continent d’Ouespéro. Héritier direct de la langue populaire de l’ancien empire occidental, il s’est imposé comme langue du commerce, de la diplomatie et des échanges quotidiens, en particulier dans l’Ouest et le Sud du continent. Il est parlé sous six grands dialectes régionaux, mutuellement intelligibles à l’oral. Tous utilisent le même alphabet, mais diffèrent par leurs conventions orthographiques, leurs choix graphiques et leurs traditions scribales.\n\nLe Rénon commun est une langue fonctionnelle, pragmatique et évolutive, issue de la langue parlée plutôt que de la norme savante. Il privilégie l’efficacité communicative et l’intercompréhension entre peuples d’origines diverses. Il est parfaitement adapté aux usages quotidiens, commerciaux et diplomatiques, mais reste peu apte à exprimer des concepts abstraits complexes sans périphrases. Ses traits généraux incluent :\n\n- une grammaire simplifiée par rapport à la [langue impériale ancienne](/regles/langues/imperial),\n- une syntaxe plus stable, généralement sujet–verbe–objet,\n- un affaiblissement des flexions anciennes, compensé par l’usage accru de prépositions,\n- un vocabulaire composite mêlant héritage impérial, innovations populaires et emprunts régionaux.  "),
      ScriptId = new Update<Guid?>(_renon.EntityId),
      TypicalSpeakers = new Update<string>("  Dans les régions anciennement impériales, il est souvent langue première. Ailleurs, il est presque toujours une langue seconde, apprise pour des raisons pratiques. Même lorsque la maîtrise est imparfaite, l’intercompréhension reste généralement suffisante. Le Rénon commun est parlé par :\r\n\r\n- la majorité des populations urbaines et côtières,\r\n- les marchands, diplomates et voyageurs,\r\n- de nombreuses communautés locales multilingues.  ")
    };

    LanguageModel? language = await _languageService.UpdateAsync(id, payload);
    Assert.NotNull(language);

    Assert.Equal(id, language.Id);
    Assert.Equal(2, language.Version);
    Assert.Equal(Actor, language.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, language.UpdatedOn, TimeSpan.FromSeconds(1));

    Assert.Equal(payload.Name.Trim(), language.Name);
    Assert.Equal(payload.Summary.Value?.Trim(), language.Summary);
    Assert.Equal(payload.Description.Value?.Trim(), language.Description);
    Assert.Equal(payload.ScriptId.Value, language.Script?.Id);
    Assert.Equal(payload.TypicalSpeakers.Value?.Trim(), language.TypicalSpeakers);
  }
}
