using Krakenar.Contracts.Search;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Contracts;
using SkillCraft.Api.Contracts.Parties;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Parties;

namespace SkillCraft.Api.Parties;

[Trait(Traits.Category, Categories.Integration)]
public class PartyIntegrationTests : IntegrationTests
{
  private readonly IPartyRepository _partyRepository;
  private readonly IPartyService _partyService;

  private Party _party = null!;

  public PartyIntegrationTests() : base()
  {
    _partyRepository = ServiceProvider.GetRequiredService<IPartyRepository>();
    _partyService = ServiceProvider.GetRequiredService<IPartyService>();
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    _party = new Party(World, new Name("Party"), UserId);
    await _partyRepository.SaveAsync(_party);
  }

  [Theory(DisplayName = "It should create a new party.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_NotExist_When_CreateOrReplace_Then_Created(bool withId)
  {
    CreateOrReplacePartyPayload payload = new()
    {
      Name = "  Nova  ",
      Description = "  Le groupe est joueurs ex-Nmédia.  "
    };
    Guid? id = withId ? Guid.NewGuid() : null;

    CreateOrReplacePartyResult result = await _partyService.CreateOrReplaceAsync(payload, id);
    Assert.True(result.Created);
    PartyModel party = result.Party;

    if (id.HasValue)
    {
      Assert.Equal(id.Value, party.Id);
    }
    Assert.Equal(2, party.Version);
    Assert.Equal(Actor, party.CreatedBy);
    Assert.Equal(DateTime.UtcNow, party.CreatedOn, TimeSpan.FromSeconds(1));
    Assert.Equal(Actor, party.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, party.UpdatedOn, TimeSpan.FromSeconds(1));

    Assert.Equal(payload.Name.Trim(), party.Name);
    Assert.Equal(payload.Description.Trim(), party.Description);
  }

  [Fact(DisplayName = "It should delete an existing party.")]
  public async Task Given_Exists_When_Delete_Then_Deleted()
  {
    Guid id = _party.EntityId;
    PartyModel? party = await _partyService.DeleteAsync(id);
    Assert.NotNull(party);
    Assert.Equal(id, party.Id);
  }

  [Fact(DisplayName = "It should read an existing party.")]
  public async Task Given_Exists_When_Read_Then_Returned()
  {
    Guid id = _party.EntityId;
    PartyModel? party = await _partyService.ReadAsync(id);
    Assert.NotNull(party);
    Assert.Equal(id, party.Id);
  }

  [Fact(DisplayName = "It should replace an existing party.")]
  public async Task Given_Exists_When_CreateOrReplace_Then_Replaced()
  {
    CreateOrReplacePartyPayload payload = new()
    {
      Name = "  Nova  ",
      Description = "  Le groupe est joueurs ex-Nmédia.  "
    };
    Guid id = _party.EntityId;

    CreateOrReplacePartyResult result = await _partyService.CreateOrReplaceAsync(payload, id);
    Assert.False(result.Created);
    PartyModel party = result.Party;

    Assert.Equal(id, party.Id);
    Assert.Equal(2, party.Version);
    Assert.Equal(Actor, party.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, party.UpdatedOn, TimeSpan.FromSeconds(1));

    Assert.Equal(payload.Name.Trim(), party.Name);
    Assert.Equal(payload.Description.Trim(), party.Description);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task Given_Payload_When_Search_Then_Results()
  {
    Party ascension = new(World, new Name("Ascension"), UserId);
    Party brigade = new(World, new Name("Brigade"), UserId);
    Party griffeNoire = new(World, new Name("Griffe Noire"), UserId);
    await _partyRepository.SaveAsync([ascension, brigade, griffeNoire]);

    SearchPartiesPayload payload = new()
    {
      Skip = 1,
      Limit = 1
    };
    payload.Ids.AddRange([ascension.EntityId, brigade.EntityId, griffeNoire.EntityId, Guid.Empty]);
    payload.Search.Operator = SearchOperator.Or;
    payload.Search.Terms.AddRange([new SearchTerm("party"), new SearchTerm("%g%")]);
    payload.Sort.Add(new PartySortOption(PartySort.Name));

    SearchResults<PartyModel> results = await _partyService.SearchAsync(payload);
    Assert.Equal(2, results.Total);

    PartyModel result = Assert.Single(results.Items);
    Assert.Equal(griffeNoire.EntityId, result.Id);
  }

  [Fact(DisplayName = "It should update an existing party.")]
  public async Task Given_Exists_When_Update_ThenUpdated()
  {
    Guid id = _party.EntityId;
    UpdatePartyPayload payload = new()
    {
      Name = "  Nova  ",
      Description = new Update<string>("  Le groupe est joueurs ex-Nmédia.  ")
    };

    PartyModel? party = await _partyService.UpdateAsync(id, payload);
    Assert.NotNull(party);

    Assert.Equal(id, party.Id);
    Assert.Equal(2, party.Version);
    Assert.Equal(Actor, party.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, party.UpdatedOn, TimeSpan.FromSeconds(1));

    Assert.Equal(payload.Name.Trim(), party.Name);
    Assert.Equal(payload.Description.Value?.Trim(), party.Description);
  }
}
