using SkillCraft.Api.Core.Scripts;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Languages.Commands;

internal abstract class SaveLanguage
{
  protected virtual IScriptRepository ScriptRepository { get; }

  protected SaveLanguage(IScriptRepository scriptRepository)
  {
    ScriptRepository = scriptRepository;
  }

  protected virtual async Task SetScriptAsync(Language language, Guid? scriptEntityId, WorldId worldId, CancellationToken cancellationToken)
  {
    Script? script = null;
    if (scriptEntityId.HasValue)
    {
      ScriptId scriptId = new(scriptEntityId.Value, worldId);
      script = await ScriptRepository.LoadAsync(scriptId, cancellationToken)
        ?? throw new EntityNotFoundException(new Entity(Script.EntityKind, scriptEntityId.Value), nameof(Language.ScriptId));
    }
    language.SetScript(script);
  }
}
