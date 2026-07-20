using Krakenar.Contracts.Search;
using Logitar;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Builders;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Languages.Models;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Scripts;

namespace SkillCraft.Api.IntegrationTests.Languages;

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

    _renon = ScriptBuilder.Renon(Faker, Context.World);
    _scriptRepository.Add(_renon);

    _language = new LanguageBuilder(Faker).WithWorld(Context.World).Build();
    _languageRepository.Add(_language);

    await Context.SaveChangesAsync();
  }

  [Theory(DisplayName = "It should create a new language.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_NotExist_When_CreateOrReplace_Then_Created(bool withId)
  {
    CreateOrReplaceLanguagePayload payload = new()
    {
      Name = " Commun ",
      Summary = "  Langue véhiculaire pragmatique et évolutive, parlée sur tout Ouespéro.  ",
      HtmlContent = "   Le Rénon commun, souvent abrégé en _Commun_, est la langue véhiculaire la plus répandue sur le continent d’Ouespéro. Héritier direct de la langue populaire de l’ancien empire occidental, il s’est imposé comme langue du commerce, de la diplomatie et des échanges quotidiens, en particulier dans l’Ouest et le Sud du continent. Il est parlé sous six grands dialectes régionaux, mutuellement intelligibles à l’oral. Tous utilisent le même alphabet, mais diffèrent par leurs conventions orthographiques, leurs choix graphiques et leurs traditions scribales.\n\nLe Rénon commun est une langue fonctionnelle, pragmatique et évolutive, issue de la langue parlée plutôt que de la norme savante. Il privilégie l’efficacité communicative et l’intercompréhension entre peuples d’origines diverses. Il est parfaitement adapté aux usages quotidiens, commerciaux et diplomatiques, mais reste peu apte à exprimer des concepts abstraits complexes sans périphrases. Ses traits généraux incluent :\n\n- une grammaire simplifiée par rapport à la [langue impériale ancienne](/regles/langues/imperial),\n- une syntaxe plus stable, généralement sujet–verbe–objet,\n- un affaiblissement des flexions anciennes, compensé par l’usage accru de prépositions,\n- un vocabulaire composite mêlant héritage impérial, innovations populaires et emprunts régionaux.   ",
      ScriptId = _renon.Id,
      TypicalSpeakers = "   Humains   "
    };
    Guid? id = withId ? Guid.NewGuid() : null;

    CreateOrReplaceLanguageResult result = await _languageService.CreateOrReplaceAsync(payload, id);
    Assert.True(result.Created);
    LanguageModel language = result.Language;
    Assert.NotNull(language);

    if (id.HasValue)
    {
      Assert.Equal(id.Value, language.Id);
    }
    else
    {
      Assert.NotEqual(Guid.Empty, language.Id);
    }
    Assert.Equal(1, language.Version);
    Assert.Equal(Actor, language.CreatedBy);
    Assert.Equal(DateTime.UtcNow, language.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(language.CreatedBy, language.UpdatedBy);
    Assert.Equal(language.CreatedOn, language.UpdatedOn);

    Assert.Equal(payload.Name.CleanTrim(), language.Name);
    Assert.Equal(payload.Summary?.CleanTrim(), language.Summary);
    Assert.Equal(payload.HtmlContent?.CleanTrim(), language.HtmlContent);

    Assert.NotNull(language.Script);
    Assert.Equal(_renon.Id, language.Script.Id);
    Assert.Equal(payload.TypicalSpeakers?.CleanTrim(), language.TypicalSpeakers);
  }

  [Fact(DisplayName = "It should filter search results by script ID.")]
  public async Task Given_ScriptId_When_Search_Then_Results()
  {
    Language commun = new LanguageBuilder(Faker).WithWorld(Context.World).WithName("Commun").WithScript(_renon).Build();
    _languageRepository.Add(commun);
    await Context.SaveChangesAsync();

    SearchLanguagesPayload payload = new()
    {
      ScriptId = _renon.Id
    };

    SearchResults<LanguageModel> results = await _languageService.SearchAsync(payload);
    Assert.Equal(1, results.Total);

    LanguageModel language = Assert.Single(results.Items);
    Assert.Equal(commun.Id, language.Id);
  }

  [Fact(DisplayName = "It should read a language by ID.")]
  public async Task Given_Id_When_Read_Then_Read()
  {
    LanguageModel? language = await _languageService.ReadAsync(_language.Id);
    Assert.NotNull(language);
    Assert.Equal(_language.Id, language.Id);
  }

  [Fact(DisplayName = "It should replace an existing language.")]
  public async Task Given_Exists_When_CreateOrReplace_Then_Replaced()
  {
    CreateOrReplaceLanguagePayload payload = new()
    {
      Name = " Commun ",
      Summary = "  Langue véhiculaire pragmatique et évolutive, parlée sur tout Ouespéro.  ",
      HtmlContent = "   Le Rénon commun, souvent abrégé en _Commun_, est la langue véhiculaire la plus répandue sur le continent d’Ouespéro. Héritier direct de la langue populaire de l’ancien empire occidental, il s’est imposé comme langue du commerce, de la diplomatie et des échanges quotidiens, en particulier dans l’Ouest et le Sud du continent. Il est parlé sous six grands dialectes régionaux, mutuellement intelligibles à l’oral. Tous utilisent le même alphabet, mais diffèrent par leurs conventions orthographiques, leurs choix graphiques et leurs traditions scribales.\n\nLe Rénon commun est une langue fonctionnelle, pragmatique et évolutive, issue de la langue parlée plutôt que de la norme savante. Il privilégie l’efficacité communicative et l’intercompréhension entre peuples d’origines diverses. Il est parfaitement adapté aux usages quotidiens, commerciaux et diplomatiques, mais reste peu apte à exprimer des concepts abstraits complexes sans périphrases. Ses traits généraux incluent :\n\n- une grammaire simplifiée par rapport à la [langue impériale ancienne](/regles/langues/imperial),\n- une syntaxe plus stable, généralement sujet–verbe–objet,\n- un affaiblissement des flexions anciennes, compensé par l’usage accru de prépositions,\n- un vocabulaire composite mêlant héritage impérial, innovations populaires et emprunts régionaux.   ",
      ScriptId = _renon.Id,
      TypicalSpeakers = "   Humains   "
    };
    Guid id = _language.Id;

    CreateOrReplaceLanguageResult result = await _languageService.CreateOrReplaceAsync(payload, id);
    Assert.False(result.Created);
    LanguageModel language = result.Language;
    Assert.NotNull(language);

    Assert.Equal(id, language.Id);
    Assert.Equal(2, language.Version);
    Assert.Equal(_language.CreatedBy, language.CreatedBy.Id);
    Assert.Equal(_language.CreatedOn, language.CreatedOn, TimeSpan.FromMilliseconds(1));
    Assert.Equal(Actor, language.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, language.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Equal(payload.Name.CleanTrim(), language.Name);
    Assert.Equal(payload.Summary?.CleanTrim(), language.Summary);
    Assert.Equal(payload.HtmlContent?.CleanTrim(), language.HtmlContent);

    Assert.NotNull(language.Script);
    Assert.Equal(_renon.Id, language.Script.Id);
    Assert.Equal(payload.TypicalSpeakers?.CleanTrim(), language.TypicalSpeakers);
  }

  [Fact(DisplayName = "It should return empty search results.")]
  public async Task Given_NoMatch_When_Search_Then_EmptyResults()
  {
    Context.World = new WorldBuilder(Faker).Build();

    SearchLanguagesPayload payload = new();

    SearchResults<LanguageModel> results = await _languageService.SearchAsync(payload);
    Assert.Equal(0, results.Total);
    Assert.Empty(results.Items);
  }

  [Fact(DisplayName = "It should return null when no language was found.")]
  public async Task Given_NotFound_When_Read_Then_NullReturned()
  {
    Context.World = new WorldBuilder(Faker).Build();

    Assert.Null(await _languageService.ReadAsync(_language.Id));
  }

  [Fact(DisplayName = "It should return null when the language was not found.")]
  public async Task Given_NotFound_When_Update_Then_NullReturned()
  {
    Assert.Null(await _languageService.UpdateAsync(Guid.Empty, new UpdateLanguagePayload()));
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task Given_Matches_When_Search_Then_Results()
  {
    Language commun = new LanguageBuilder(Faker).WithWorld(Context.World).WithName("Commun").Build();
    Language imperial = new LanguageBuilder(Faker).WithWorld(Context.World).WithName("Impérial").Build();
    Language wisgorne = new LanguageBuilder(Faker).WithWorld(Context.World).WithName("Wisgorne").Build();
    _languageRepository.Add(commun, imperial, wisgorne);
    await Context.SaveChangesAsync();

    SearchLanguagesPayload payload = new()
    {
      Skip = 1,
      Limit = 1
    };
    payload.Search.Operator = SearchOperator.Or;
    payload.Search.Terms.Add(new SearchTerm("%g%"));
    payload.Search.Terms.Add(new SearchTerm("%i%"));
    payload.Ids.AddRange([Guid.Empty, commun.Id, imperial.Id, wisgorne.Id]);
    payload.Sort.Add(new LanguageSortOption(LanguageSort.Name, isDescending: true));

    SearchResults<LanguageModel> results = await _languageService.SearchAsync(payload);
    Assert.Equal(2, results.Total);

    LanguageModel language = Assert.Single(results.Items);
    Assert.Equal(imperial.Id, language.Id);
  }

  [Fact(DisplayName = "It should throw ResourceNotFoundException when creating a language.")]
  public async Task Given_ScriptNotFound_When_Create_Then_ResourceNotFoundException()
  {
    CreateOrReplaceLanguagePayload payload = new()
    {
      Name = " Commun ",
      Summary = "  Langue véhiculaire pragmatique et évolutive, parlée sur tout Ouespéro.  ",
      HtmlContent = "   Le Rénon commun, souvent abrégé en _Commun_, est la langue véhiculaire la plus répandue sur le continent d’Ouespéro. Héritier direct de la langue populaire de l’ancien empire occidental, il s’est imposé comme langue du commerce, de la diplomatie et des échanges quotidiens, en particulier dans l’Ouest et le Sud du continent. Il est parlé sous six grands dialectes régionaux, mutuellement intelligibles à l’oral. Tous utilisent le même alphabet, mais diffèrent par leurs conventions orthographiques, leurs choix graphiques et leurs traditions scribales.\n\nLe Rénon commun est une langue fonctionnelle, pragmatique et évolutive, issue de la langue parlée plutôt que de la norme savante. Il privilégie l’efficacité communicative et l’intercompréhension entre peuples d’origines diverses. Il est parfaitement adapté aux usages quotidiens, commerciaux et diplomatiques, mais reste peu apte à exprimer des concepts abstraits complexes sans périphrases. Ses traits généraux incluent :\n\n- une grammaire simplifiée par rapport à la [langue impériale ancienne](/regles/langues/imperial),\n- une syntaxe plus stable, généralement sujet–verbe–objet,\n- un affaiblissement des flexions anciennes, compensé par l’usage accru de prépositions,\n- un vocabulaire composite mêlant héritage impérial, innovations populaires et emprunts régionaux.   ",
      ScriptId = Guid.Empty,
      TypicalSpeakers = "   Humains   "
    };

    var exception = await Assert.ThrowsAsync<ResourceNotFoundException>(async () => await _languageService.CreateOrReplaceAsync(payload));
    Assert.Equal(Context.WorldId, exception.WorldId);
    Assert.Equal(Script.ResourceKind, exception.ResourceKind);
    Assert.Equal(payload.ScriptId.Value, exception.ResourceId);
    Assert.Equal("ScriptId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ResourceNotFoundException when replacing a language.")]
  public async Task Given_ScriptNotFound_When_Replace_Then_ResourceNotFoundException()
  {
    CreateOrReplaceLanguagePayload payload = new()
    {
      Name = " Commun ",
      Summary = "  Langue véhiculaire pragmatique et évolutive, parlée sur tout Ouespéro.  ",
      HtmlContent = "   Le Rénon commun, souvent abrégé en _Commun_, est la langue véhiculaire la plus répandue sur le continent d’Ouespéro. Héritier direct de la langue populaire de l’ancien empire occidental, il s’est imposé comme langue du commerce, de la diplomatie et des échanges quotidiens, en particulier dans l’Ouest et le Sud du continent. Il est parlé sous six grands dialectes régionaux, mutuellement intelligibles à l’oral. Tous utilisent le même alphabet, mais diffèrent par leurs conventions orthographiques, leurs choix graphiques et leurs traditions scribales.\n\nLe Rénon commun est une langue fonctionnelle, pragmatique et évolutive, issue de la langue parlée plutôt que de la norme savante. Il privilégie l’efficacité communicative et l’intercompréhension entre peuples d’origines diverses. Il est parfaitement adapté aux usages quotidiens, commerciaux et diplomatiques, mais reste peu apte à exprimer des concepts abstraits complexes sans périphrases. Ses traits généraux incluent :\n\n- une grammaire simplifiée par rapport à la [langue impériale ancienne](/regles/langues/imperial),\n- une syntaxe plus stable, généralement sujet–verbe–objet,\n- un affaiblissement des flexions anciennes, compensé par l’usage accru de prépositions,\n- un vocabulaire composite mêlant héritage impérial, innovations populaires et emprunts régionaux.   ",
      ScriptId = Guid.Empty,
      TypicalSpeakers = "   Humains   "
    };

    var exception = await Assert.ThrowsAsync<ResourceNotFoundException>(async () => await _languageService.CreateOrReplaceAsync(payload, _language.Id));
    Assert.Equal(Context.WorldId, exception.WorldId);
    Assert.Equal(Script.ResourceKind, exception.ResourceKind);
    Assert.Equal(payload.ScriptId.Value, exception.ResourceId);
    Assert.Equal("ScriptId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ResourceNotFoundException when updating a language.")]
  public async Task Given_ScriptNotFound_When_Update_Then_ResourceNotFoundException()
  {
    UpdateLanguagePayload payload = new()
    {
      ScriptId = new Optional<Guid?>(Guid.Empty)
    };

    var exception = await Assert.ThrowsAsync<ResourceNotFoundException>(async () => await _languageService.UpdateAsync(_language.Id, payload));
    Assert.Equal(Context.WorldId, exception.WorldId);
    Assert.Equal(Script.ResourceKind, exception.ResourceKind);
    Assert.Equal(payload.ScriptId.Value, exception.ResourceId);
    Assert.Equal("ScriptId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when creating a language.")]
  public async Task Given_NotAllowed_When_Create_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder(Faker).Build();

    CreateOrReplaceLanguagePayload payload = new()
    {
      Name = " Commun ",
      Summary = "  Langue véhiculaire pragmatique et évolutive, parlée sur tout Ouespéro.  ",
      HtmlContent = "   Le Rénon commun, souvent abrégé en _Commun_, est la langue véhiculaire la plus répandue sur le continent d’Ouespéro. Héritier direct de la langue populaire de l’ancien empire occidental, il s’est imposé comme langue du commerce, de la diplomatie et des échanges quotidiens, en particulier dans l’Ouest et le Sud du continent. Il est parlé sous six grands dialectes régionaux, mutuellement intelligibles à l’oral. Tous utilisent le même alphabet, mais diffèrent par leurs conventions orthographiques, leurs choix graphiques et leurs traditions scribales.\n\nLe Rénon commun est une langue fonctionnelle, pragmatique et évolutive, issue de la langue parlée plutôt que de la norme savante. Il privilégie l’efficacité communicative et l’intercompréhension entre peuples d’origines diverses. Il est parfaitement adapté aux usages quotidiens, commerciaux et diplomatiques, mais reste peu apte à exprimer des concepts abstraits complexes sans périphrases. Ses traits généraux incluent :\n\n- une grammaire simplifiée par rapport à la [langue impériale ancienne](/regles/langues/imperial),\n- une syntaxe plus stable, généralement sujet–verbe–objet,\n- un affaiblissement des flexions anciennes, compensé par l’usage accru de prépositions,\n- un vocabulaire composite mêlant héritage impérial, innovations populaires et emprunts régionaux.   ",
      ScriptId = _renon.Id,
      TypicalSpeakers = "   Humains   "
    };

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _languageService.CreateOrReplaceAsync(payload));
    Assert.Equal(Context.UserId, exception.UserId);
    Assert.Equal(Actions.CreateLanguage, exception.Action);
    Assert.Equal(Context.World?.Identifier.ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when replacing a language.")]
  public async Task Given_NotAllowed_When_Replace_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder(Faker).Build();

    CreateOrReplaceLanguagePayload payload = new()
    {
      Name = " Commun ",
      Summary = "  Langue véhiculaire pragmatique et évolutive, parlée sur tout Ouespéro.  ",
      HtmlContent = "   Le Rénon commun, souvent abrégé en _Commun_, est la langue véhiculaire la plus répandue sur le continent d’Ouespéro. Héritier direct de la langue populaire de l’ancien empire occidental, il s’est imposé comme langue du commerce, de la diplomatie et des échanges quotidiens, en particulier dans l’Ouest et le Sud du continent. Il est parlé sous six grands dialectes régionaux, mutuellement intelligibles à l’oral. Tous utilisent le même alphabet, mais diffèrent par leurs conventions orthographiques, leurs choix graphiques et leurs traditions scribales.\n\nLe Rénon commun est une langue fonctionnelle, pragmatique et évolutive, issue de la langue parlée plutôt que de la norme savante. Il privilégie l’efficacité communicative et l’intercompréhension entre peuples d’origines diverses. Il est parfaitement adapté aux usages quotidiens, commerciaux et diplomatiques, mais reste peu apte à exprimer des concepts abstraits complexes sans périphrases. Ses traits généraux incluent :\n\n- une grammaire simplifiée par rapport à la [langue impériale ancienne](/regles/langues/imperial),\n- une syntaxe plus stable, généralement sujet–verbe–objet,\n- un affaiblissement des flexions anciennes, compensé par l’usage accru de prépositions,\n- un vocabulaire composite mêlant héritage impérial, innovations populaires et emprunts régionaux.   ",
      ScriptId = _renon.Id,
      TypicalSpeakers = "   Humains   "
    };

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _languageService.CreateOrReplaceAsync(payload, _language.Id));
    Assert.Equal(Context.UserId, exception.UserId);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(_language.Identifier.ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when updating a language.")]
  public async Task Given_NotAllowed_When_Update_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder(Faker).Build();

    UpdateLanguagePayload payload = new();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _languageService.UpdateAsync(_language.Id, payload));
    Assert.Equal(Context.UserId, exception.UserId);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(_language.Identifier.ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should update an existing language.")]
  public async Task Given_Exists_When_Update_Then_Updated()
  {
    Guid id = _language.Id;
    UpdateLanguagePayload payload = new()
    {
      Name = " Commun ",
      Summary = new Optional<string>("  Langue véhiculaire pragmatique et évolutive, parlée sur tout Ouespéro.  "),
      HtmlContent = new Optional<string>("   Le Rénon commun, souvent abrégé en _Commun_, est la langue véhiculaire la plus répandue sur le continent d’Ouespéro. Héritier direct de la langue populaire de l’ancien empire occidental, il s’est imposé comme langue du commerce, de la diplomatie et des échanges quotidiens, en particulier dans l’Ouest et le Sud du continent. Il est parlé sous six grands dialectes régionaux, mutuellement intelligibles à l’oral. Tous utilisent le même alphabet, mais diffèrent par leurs conventions orthographiques, leurs choix graphiques et leurs traditions scribales.\n\nLe Rénon commun est une langue fonctionnelle, pragmatique et évolutive, issue de la langue parlée plutôt que de la norme savante. Il privilégie l’efficacité communicative et l’intercompréhension entre peuples d’origines diverses. Il est parfaitement adapté aux usages quotidiens, commerciaux et diplomatiques, mais reste peu apte à exprimer des concepts abstraits complexes sans périphrases. Ses traits généraux incluent :\n\n- une grammaire simplifiée par rapport à la [langue impériale ancienne](/regles/langues/imperial),\n- une syntaxe plus stable, généralement sujet–verbe–objet,\n- un affaiblissement des flexions anciennes, compensé par l’usage accru de prépositions,\n- un vocabulaire composite mêlant héritage impérial, innovations populaires et emprunts régionaux.   "),
      ScriptId = new Optional<Guid?>(_renon.Id),
      TypicalSpeakers = new Optional<string>("   Humains   ")
    };

    LanguageModel? language = await _languageService.UpdateAsync(id, payload);
    Assert.NotNull(language);

    Assert.Equal(id, language.Id);
    Assert.Equal(2, language.Version);
    Assert.Equal(_language.CreatedBy, language.CreatedBy.Id);
    Assert.Equal(_language.CreatedOn, language.CreatedOn, TimeSpan.FromMilliseconds(1));
    Assert.Equal(Actor, language.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, language.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Equal(payload.Name.CleanTrim(), language.Name);
    Assert.Equal(payload.Summary.Value?.CleanTrim(), language.Summary);
    Assert.Equal(payload.HtmlContent.Value?.CleanTrim(), language.HtmlContent);

    Assert.NotNull(language.Script);
    Assert.Equal(_renon.Id, language.Script.Id);
    Assert.Equal(payload.TypicalSpeakers.Value?.CleanTrim(), language.TypicalSpeakers);
  }
}
