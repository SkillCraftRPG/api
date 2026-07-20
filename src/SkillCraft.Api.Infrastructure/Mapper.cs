using Krakenar.Contracts;
using Krakenar.Contracts.Actors;
using Logitar;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Castes;
using SkillCraft.Api.Core.Castes.Models;
using SkillCraft.Api.Core.Customizations;
using SkillCraft.Api.Core.Customizations.Models;
using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Languages.Models;
using SkillCraft.Api.Core.Scripts;
using SkillCraft.Api.Core.Scripts.Models;
using SkillCraft.Api.Core.Worlds;
using SkillCraft.Api.Core.Worlds.Models;

namespace SkillCraft.Api.Infrastructure;

internal class Mapper
{
  private readonly Dictionary<Guid, Actor> _actors = [];
  private readonly Actor _system = new();

  public Mapper()
  {
  }

  public Mapper(IEnumerable<KeyValuePair<Guid, Actor>> actors)
  {
    foreach (KeyValuePair<Guid, Actor> actor in actors)
    {
      _actors[actor.Key] = actor.Value;
    }
  }

  public CasteModel ToCaste(Caste source)
  {
    CasteModel destination = new()
    {
      Id = source.Id,
      Name = source.Name,
      Summary = source.Summary,
      HtmlContent = source.HtmlContent,
      Skill = source.Skill,
      WealthRoll = source.WealthRoll
    };

    MapAggregate(source, destination);

    return destination;
  }

  public CustomizationModel ToCustomization(Customization source)
  {
    CustomizationModel destination = new()
    {
      Id = source.Id,
      Kind = source.Kind,
      Name = source.Name,
      Summary = source.Summary,
      HtmlContent = source.HtmlContent
    };

    MapAggregate(source, destination);

    return destination;
  }

  public LanguageModel ToLanguage(Language source)
  {
    LanguageModel destination = new()
    {
      Id = source.Id,
      Name = source.Name,
      Summary = source.Summary,
      HtmlContent = source.HtmlContent,
      TypicalSpeakers = source.TypicalSpeakers
    };

    if (source.ScriptId.HasValue)
    {
      if (source.Script is null)
      {
        throw new ArgumentException("The script is required.", nameof(source));
      }
      destination.Script = ToScript(source.Script);
    }

    MapAggregate(source, destination);

    return destination;
  }

  public ScriptModel ToScript(Script source)
  {
    ScriptModel destination = new()
    {
      Id = source.Id,
      Name = source.Name,
      Summary = source.Summary,
      HtmlContent = source.HtmlContent
    };

    MapAggregate(source, destination);

    return destination;
  }

  public WorldModel ToWorld(World source)
  {
    WorldModel destination = new()
    {
      Id = source.Id,
      Owner = FindActor(source.OwnerId),
      Key = source.Key,
      Name = source.Name,
      HtmlContent = source.HtmlContent
    };

    MapAggregate(source, destination);

    return destination;
  }

  private void MapAggregate(object source, Aggregate destination)
  {
    if (source is IAuditable auditable)
    {
      destination.CreatedBy = FindActor(auditable.CreatedBy);
      destination.CreatedOn = auditable.CreatedOn.AsUniversalTime();
      destination.UpdatedBy = FindActor(auditable.UpdatedBy);
      destination.UpdatedOn = auditable.UpdatedOn.AsUniversalTime();
    }

    if (source is IVersioned versioned)
    {
      destination.Version = versioned.Version;
    }
  }

  private Actor FindActor(Guid id) => _actors.TryGetValue(id, out Actor? actor) ? actor : _system;
}
