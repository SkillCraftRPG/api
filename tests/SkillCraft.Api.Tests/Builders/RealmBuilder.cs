using Bogus;
using Krakenar.Contracts.Realms;

namespace SkillCraft.Api.Builders;

public interface IRealmBuilder
{
  Realm Build();
}

public class RealmBuilder : IRealmBuilder
{
  private readonly Faker _faker;

  public RealmBuilder(Faker? faker = null)
  {
    _faker = faker ?? new();
  }

  public Realm Build()
  {
    Realm realm = new()
    {
      Id = Guid.NewGuid(),
      UniqueSlug = "game",
      DisplayName = "Game"
    };
    realm.CreatedOn = realm.UpdatedOn = realm.SecretChangedOn = DateTime.UtcNow;
    return realm;
  }
}
