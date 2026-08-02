using Krakenar.Contracts.Search;
using Logitar;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Builders;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Actors;
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
    await _scriptRepository.SaveAsync(_renon);

    _language = new LanguageBuilder(Faker).WithWorld(Context.World).Build();
    await _languageRepository.SaveAsync(_language);
  }

  [Theory(DisplayName = "It should create a new language.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_NotExist_When_CreateOrReplace_Then_Created(bool withId)
  {
    CreateOrReplaceLanguagePayload payload = new()
    {
      Name = " Commun ",
      Summary = "  Langue v�hiculaire pragmatique et �volutive, parl�e sur tout Ouesp�ro.  ",
      Content = "   Le R�non commun, souvent abr�g� en _Commun_, est la langue v�hiculaire la plus r�pandue sur le continent d�Ouesp�ro. H�ritier direct de la langue populaire de l�ancien empire occidental, il s�est impos� comme langue du commerce, de la diplomatie et des �changes quotidiens, en particulier dans l�Ouest et le Sud du continent. Il est parl� sous six grands dialectes r�gionaux, mutuellement intelligibles � l�oral. Tous utilisent le m�me alphabet, mais diff�rent par leurs conventions orthographiques, leurs choix graphiques et leurs traditions scribales.\n\nLe R�non commun est une langue fonctionnelle, pragmatique et �volutive, issue de la langue parl�e plut�t que de la norme savante. Il privil�gie l�efficacit� communicative et l�intercompr�hension entre peuples d�origines diverses. Il est parfaitement adapt� aux usages quotidiens, commerciaux et diplomatiques, mais reste peu apte � exprimer des concepts abstraits complexes sans p�riphrases. Ses traits g�n�raux incluent :\n\n- une grammaire simplifi�e par rapport � la [langue imp�riale ancienne](/regles/langues/imperial),\n- une syntaxe plus stable, g�n�ralement sujet�verbe�objet,\n- un affaiblissement des flexions anciennes, compens� par l�usage accru de pr�positions,\n- un vocabulaire composite m�lant h�ritage imp�rial, innovations populaires et emprunts r�gionaux.   ",
      ScriptId = _renon.ResourceId,
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
    Assert.Equal(3, language.Version);
    Assert.Equal(Actor, language.CreatedBy);
    Assert.Equal(DateTime.UtcNow, language.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(language.CreatedBy, language.UpdatedBy);
    Assert.True(language.CreatedOn < language.UpdatedOn);

    Assert.Equal(payload.Name.CleanTrim(), language.Name);
    Assert.Equal(payload.Summary?.CleanTrim(), language.Summary);
    Assert.Equal(payload.Content?.CleanTrim(), language.Content);

    Assert.NotNull(language.Script);
    Assert.Equal(_renon.ResourceId, language.Script.Id);
    Assert.Equal(payload.TypicalSpeakers?.CleanTrim(), language.TypicalSpeakers);
  }

  [Fact(DisplayName = "It should filter search results by script ID.")]
  public async Task Given_ScriptId_When_Search_Then_Results()
  {
    Language commun = LanguageBuilder.Common(Faker, Context.World, _renon);
    await _languageRepository.SaveAsync(commun);

    SearchLanguagesPayload payload = new()
    {
      ScriptId = _renon.ResourceId
    };

    SearchResults<LanguageModel> results = await _languageService.SearchAsync(payload);
    Assert.Equal(1, results.Total);

    LanguageModel language = Assert.Single(results.Items);
    Assert.Equal(commun.ResourceId, language.Id);
  }

  [Fact(DisplayName = "It should read a language by ID.")]
  public async Task Given_Id_When_Read_Then_Read()
  {
    LanguageModel? language = await _languageService.ReadAsync(_language.ResourceId);
    Assert.NotNull(language);
    Assert.Equal(_language.ResourceId, language.Id);
  }

  [Fact(DisplayName = "It should replace an existing language.")]
  public async Task Given_Exists_When_CreateOrReplace_Then_Replaced()
  {
    CreateOrReplaceLanguagePayload payload = new()
    {
      Name = " Commun ",
      Summary = "  Langue v�hiculaire pragmatique et �volutive, parl�e sur tout Ouesp�ro.  ",
      Content = "   Le R�non commun, souvent abr�g� en _Commun_, est la langue v�hiculaire la plus r�pandue sur le continent d�Ouesp�ro. H�ritier direct de la langue populaire de l�ancien empire occidental, il s�est impos� comme langue du commerce, de la diplomatie et des �changes quotidiens, en particulier dans l�Ouest et le Sud du continent. Il est parl� sous six grands dialectes r�gionaux, mutuellement intelligibles � l�oral. Tous utilisent le m�me alphabet, mais diff�rent par leurs conventions orthographiques, leurs choix graphiques et leurs traditions scribales.\n\nLe R�non commun est une langue fonctionnelle, pragmatique et �volutive, issue de la langue parl�e plut�t que de la norme savante. Il privil�gie l�efficacit� communicative et l�intercompr�hension entre peuples d�origines diverses. Il est parfaitement adapt� aux usages quotidiens, commerciaux et diplomatiques, mais reste peu apte � exprimer des concepts abstraits complexes sans p�riphrases. Ses traits g�n�raux incluent :\n\n- une grammaire simplifi�e par rapport � la [langue imp�riale ancienne](/regles/langues/imperial),\n- une syntaxe plus stable, g�n�ralement sujet�verbe�objet,\n- un affaiblissement des flexions anciennes, compens� par l�usage accru de pr�positions,\n- un vocabulaire composite m�lant h�ritage imp�rial, innovations populaires et emprunts r�gionaux.   ",
      ScriptId = _renon.ResourceId,
      TypicalSpeakers = "   Humains   "
    };
    Guid id = _language.ResourceId;

    CreateOrReplaceLanguageResult result = await _languageService.CreateOrReplaceAsync(payload, id);
    Assert.False(result.Created);
    LanguageModel language = result.Language;
    Assert.NotNull(language);

    Assert.Equal(id, language.Id);
    Assert.Equal(4, language.Version);
    Assert.Equal(_language.CreatedBy, language.CreatedBy.GetActorId());
    Assert.Equal(_language.CreatedOn.AsUniversalTime(), language.CreatedOn, TimeSpan.FromMilliseconds(1));
    Assert.Equal(Actor, language.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, language.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Equal(payload.Name.CleanTrim(), language.Name);
    Assert.Equal(payload.Summary?.CleanTrim(), language.Summary);
    Assert.Equal(payload.Content?.CleanTrim(), language.Content);

    Assert.NotNull(language.Script);
    Assert.Equal(_renon.ResourceId, language.Script.Id);
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

    Assert.Null(await _languageService.ReadAsync(_language.ResourceId));
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
    Language imperial = new LanguageBuilder(Faker).WithWorld(Context.World).WithName("Imp�rial").Build();
    Language wisgorne = new LanguageBuilder(Faker).WithWorld(Context.World).WithName("Wisgorne").Build();
    await _languageRepository.SaveAsync([commun, imperial, wisgorne]);

    SearchLanguagesPayload payload = new()
    {
      Skip = 1,
      Limit = 1
    };
    payload.Search.Operator = SearchOperator.Or;
    payload.Search.Terms.Add(new SearchTerm("%g%"));
    payload.Search.Terms.Add(new SearchTerm("%i%"));
    payload.Ids.AddRange([Guid.Empty, commun.ResourceId, imperial.ResourceId, wisgorne.ResourceId]);
    payload.Sort.Add(new LanguageSortOption(LanguageSort.Name, isDescending: true));

    SearchResults<LanguageModel> results = await _languageService.SearchAsync(payload);
    Assert.Equal(2, results.Total);

    LanguageModel language = Assert.Single(results.Items);
    Assert.Equal(imperial.ResourceId, language.Id);
  }

  [Fact(DisplayName = "It should throw ScriptNotFoundException when creating a language.")]
  public async Task Given_ScriptNotFound_When_Create_Then_ScriptNotFoundException()
  {
    CreateOrReplaceLanguagePayload payload = new()
    {
      Name = " Commun ",
      Summary = "  Langue v�hiculaire pragmatique et �volutive, parl�e sur tout Ouesp�ro.  ",
      Content = "   Le R�non commun, souvent abr�g� en _Commun_, est la langue v�hiculaire la plus r�pandue sur le continent d�Ouesp�ro. H�ritier direct de la langue populaire de l�ancien empire occidental, il s�est impos� comme langue du commerce, de la diplomatie et des �changes quotidiens, en particulier dans l�Ouest et le Sud du continent. Il est parl� sous six grands dialectes r�gionaux, mutuellement intelligibles � l�oral. Tous utilisent le m�me alphabet, mais diff�rent par leurs conventions orthographiques, leurs choix graphiques et leurs traditions scribales.\n\nLe R�non commun est une langue fonctionnelle, pragmatique et �volutive, issue de la langue parl�e plut�t que de la norme savante. Il privil�gie l�efficacit� communicative et l�intercompr�hension entre peuples d�origines diverses. Il est parfaitement adapt� aux usages quotidiens, commerciaux et diplomatiques, mais reste peu apte � exprimer des concepts abstraits complexes sans p�riphrases. Ses traits g�n�raux incluent :\n\n- une grammaire simplifi�e par rapport � la [langue imp�riale ancienne](/regles/langues/imperial),\n- une syntaxe plus stable, g�n�ralement sujet�verbe�objet,\n- un affaiblissement des flexions anciennes, compens� par l�usage accru de pr�positions,\n- un vocabulaire composite m�lant h�ritage imp�rial, innovations populaires et emprunts r�gionaux.   ",
      ScriptId = Guid.Empty,
      TypicalSpeakers = "   Humains   "
    };

    var exception = await Assert.ThrowsAsync<ScriptNotFoundException>(async () => await _languageService.CreateOrReplaceAsync(payload));
    Assert.Equal(Context.WorldUid, exception.WorldId);
    Assert.Equal(payload.ScriptId.Value, exception.ScriptId);
    Assert.Equal("ScriptId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ScriptNotFoundException when replacing a language.")]
  public async Task Given_ScriptNotFound_When_Replace_Then_ScriptNotFoundException()
  {
    CreateOrReplaceLanguagePayload payload = new()
    {
      Name = " Commun ",
      Summary = "  Langue v�hiculaire pragmatique et �volutive, parl�e sur tout Ouesp�ro.  ",
      Content = "   Le R�non commun, souvent abr�g� en _Commun_, est la langue v�hiculaire la plus r�pandue sur le continent d�Ouesp�ro. H�ritier direct de la langue populaire de l�ancien empire occidental, il s�est impos� comme langue du commerce, de la diplomatie et des �changes quotidiens, en particulier dans l�Ouest et le Sud du continent. Il est parl� sous six grands dialectes r�gionaux, mutuellement intelligibles � l�oral. Tous utilisent le m�me alphabet, mais diff�rent par leurs conventions orthographiques, leurs choix graphiques et leurs traditions scribales.\n\nLe R�non commun est une langue fonctionnelle, pragmatique et �volutive, issue de la langue parl�e plut�t que de la norme savante. Il privil�gie l�efficacit� communicative et l�intercompr�hension entre peuples d�origines diverses. Il est parfaitement adapt� aux usages quotidiens, commerciaux et diplomatiques, mais reste peu apte � exprimer des concepts abstraits complexes sans p�riphrases. Ses traits g�n�raux incluent :\n\n- une grammaire simplifi�e par rapport � la [langue imp�riale ancienne](/regles/langues/imperial),\n- une syntaxe plus stable, g�n�ralement sujet�verbe�objet,\n- un affaiblissement des flexions anciennes, compens� par l�usage accru de pr�positions,\n- un vocabulaire composite m�lant h�ritage imp�rial, innovations populaires et emprunts r�gionaux.   ",
      ScriptId = Guid.Empty,
      TypicalSpeakers = "   Humains   "
    };

    var exception = await Assert.ThrowsAsync<ScriptNotFoundException>(async () => await _languageService.CreateOrReplaceAsync(payload, _language.ResourceId));
    Assert.Equal(Context.WorldUid, exception.WorldId);
    Assert.Equal(payload.ScriptId.Value, exception.ScriptId);
    Assert.Equal("ScriptId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ScriptNotFoundException when updating a language.")]
  public async Task Given_ScriptNotFound_When_Update_Then_ScriptNotFoundException()
  {
    UpdateLanguagePayload payload = new()
    {
      ScriptId = new Optional<Guid?>(Guid.Empty)
    };

    var exception = await Assert.ThrowsAsync<ScriptNotFoundException>(async () => await _languageService.UpdateAsync(_language.ResourceId, payload));
    Assert.Equal(Context.WorldUid, exception.WorldId);
    Assert.Equal(payload.ScriptId.Value, exception.ScriptId);
    Assert.Equal("ScriptId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when creating a language.")]
  public async Task Given_NotAllowed_When_Create_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder(Faker).Build();

    CreateOrReplaceLanguagePayload payload = new()
    {
      Name = " Commun ",
      Summary = "  Langue v�hiculaire pragmatique et �volutive, parl�e sur tout Ouesp�ro.  ",
      Content = "   Le R�non commun, souvent abr�g� en _Commun_, est la langue v�hiculaire la plus r�pandue sur le continent d�Ouesp�ro. H�ritier direct de la langue populaire de l�ancien empire occidental, il s�est impos� comme langue du commerce, de la diplomatie et des �changes quotidiens, en particulier dans l�Ouest et le Sud du continent. Il est parl� sous six grands dialectes r�gionaux, mutuellement intelligibles � l�oral. Tous utilisent le m�me alphabet, mais diff�rent par leurs conventions orthographiques, leurs choix graphiques et leurs traditions scribales.\n\nLe R�non commun est une langue fonctionnelle, pragmatique et �volutive, issue de la langue parl�e plut�t que de la norme savante. Il privil�gie l�efficacit� communicative et l�intercompr�hension entre peuples d�origines diverses. Il est parfaitement adapt� aux usages quotidiens, commerciaux et diplomatiques, mais reste peu apte � exprimer des concepts abstraits complexes sans p�riphrases. Ses traits g�n�raux incluent :\n\n- une grammaire simplifi�e par rapport � la [langue imp�riale ancienne](/regles/langues/imperial),\n- une syntaxe plus stable, g�n�ralement sujet�verbe�objet,\n- un affaiblissement des flexions anciennes, compens� par l�usage accru de pr�positions,\n- un vocabulaire composite m�lant h�ritage imp�rial, innovations populaires et emprunts r�gionaux.   ",
      ScriptId = _renon.ResourceId,
      TypicalSpeakers = "   Humains   "
    };

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _languageService.CreateOrReplaceAsync(payload));
    Assert.Equal(Context.ActorId?.Value, exception.Principal);
    Assert.Equal(Actions.CreateLanguage, exception.Action);
    Assert.Null(exception.Resource);
    Assert.Equal(Context.WorldUid, exception.WorldId);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when replacing a language.")]
  public async Task Given_NotAllowed_When_Replace_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder(Faker).Build();

    CreateOrReplaceLanguagePayload payload = new()
    {
      Name = " Commun ",
      Summary = "  Langue v�hiculaire pragmatique et �volutive, parl�e sur tout Ouesp�ro.  ",
      Content = "   Le R�non commun, souvent abr�g� en _Commun_, est la langue v�hiculaire la plus r�pandue sur le continent d�Ouesp�ro. H�ritier direct de la langue populaire de l�ancien empire occidental, il s�est impos� comme langue du commerce, de la diplomatie et des �changes quotidiens, en particulier dans l�Ouest et le Sud du continent. Il est parl� sous six grands dialectes r�gionaux, mutuellement intelligibles � l�oral. Tous utilisent le m�me alphabet, mais diff�rent par leurs conventions orthographiques, leurs choix graphiques et leurs traditions scribales.\n\nLe R�non commun est une langue fonctionnelle, pragmatique et �volutive, issue de la langue parl�e plut�t que de la norme savante. Il privil�gie l�efficacit� communicative et l�intercompr�hension entre peuples d�origines diverses. Il est parfaitement adapt� aux usages quotidiens, commerciaux et diplomatiques, mais reste peu apte � exprimer des concepts abstraits complexes sans p�riphrases. Ses traits g�n�raux incluent :\n\n- une grammaire simplifi�e par rapport � la [langue imp�riale ancienne](/regles/langues/imperial),\n- une syntaxe plus stable, g�n�ralement sujet�verbe�objet,\n- un affaiblissement des flexions anciennes, compens� par l�usage accru de pr�positions,\n- un vocabulaire composite m�lant h�ritage imp�rial, innovations populaires et emprunts r�gionaux.   ",
      ScriptId = _renon.ResourceId,
      TypicalSpeakers = "   Humains   "
    };

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _languageService.CreateOrReplaceAsync(payload, _language.ResourceId));
    Assert.Equal(Context.ActorId?.Value, exception.Principal);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(_language.Identifier.ToString(), exception.Resource);
    Assert.Equal(Context.WorldUid, exception.WorldId);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when updating a language.")]
  public async Task Given_NotAllowed_When_Update_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder(Faker).Build();

    UpdateLanguagePayload payload = new();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _languageService.UpdateAsync(_language.ResourceId, payload));
    Assert.Equal(Context.ActorId?.Value, exception.Principal);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(_language.Identifier.ToString(), exception.Resource);
    Assert.Equal(Context.WorldUid, exception.WorldId);
  }

  [Fact(DisplayName = "It should update an existing language.")]
  public async Task Given_Exists_When_Update_Then_Updated()
  {
    Guid id = _language.ResourceId;
    UpdateLanguagePayload payload = new()
    {
      Name = " Commun ",
      Summary = new Optional<string>("  Langue v�hiculaire pragmatique et �volutive, parl�e sur tout Ouesp�ro.  "),
      Content = new Optional<string>("   Le R�non commun, souvent abr�g� en _Commun_, est la langue v�hiculaire la plus r�pandue sur le continent d�Ouesp�ro. H�ritier direct de la langue populaire de l�ancien empire occidental, il s�est impos� comme langue du commerce, de la diplomatie et des �changes quotidiens, en particulier dans l�Ouest et le Sud du continent. Il est parl� sous six grands dialectes r�gionaux, mutuellement intelligibles � l�oral. Tous utilisent le m�me alphabet, mais diff�rent par leurs conventions orthographiques, leurs choix graphiques et leurs traditions scribales.\n\nLe R�non commun est une langue fonctionnelle, pragmatique et �volutive, issue de la langue parl�e plut�t que de la norme savante. Il privil�gie l�efficacit� communicative et l�intercompr�hension entre peuples d�origines diverses. Il est parfaitement adapt� aux usages quotidiens, commerciaux et diplomatiques, mais reste peu apte � exprimer des concepts abstraits complexes sans p�riphrases. Ses traits g�n�raux incluent :\n\n- une grammaire simplifi�e par rapport � la [langue imp�riale ancienne](/regles/langues/imperial),\n- une syntaxe plus stable, g�n�ralement sujet�verbe�objet,\n- un affaiblissement des flexions anciennes, compens� par l�usage accru de pr�positions,\n- un vocabulaire composite m�lant h�ritage imp�rial, innovations populaires et emprunts r�gionaux.   "),
      ScriptId = new Optional<Guid?>(_renon.ResourceId),
      TypicalSpeakers = new Optional<string>("   Humains   ")
    };

    LanguageModel? language = await _languageService.UpdateAsync(id, payload);
    Assert.NotNull(language);

    Assert.Equal(id, language.Id);
    Assert.Equal(4, language.Version);
    Assert.Equal(_language.CreatedBy, language.CreatedBy.GetActorId());
    Assert.Equal(_language.CreatedOn.AsUniversalTime(), language.CreatedOn, TimeSpan.FromMilliseconds(1));
    Assert.Equal(Actor, language.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, language.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Equal(payload.Name.CleanTrim(), language.Name);
    Assert.Equal(payload.Summary.Value?.CleanTrim(), language.Summary);
    Assert.Equal(payload.Content.Value?.CleanTrim(), language.Content);

    Assert.NotNull(language.Script);
    Assert.Equal(_renon.ResourceId, language.Script.Id);
    Assert.Equal(payload.TypicalSpeakers.Value?.CleanTrim(), language.TypicalSpeakers);
  }
}
