using Logitar.CQRS;
using Logitar.EventSourcing;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Spells.Models;
using SkillCraft.Api.Core.Talents;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Spells.Commands;

internal record CreateOrReplaceSpellCommand(CreateOrReplaceSpellPayload Payload, Guid? Id) : ICommand<CreateOrReplaceSpellResult>;

internal class CreateOrReplaceSpellCommandHandler : ICommandHandler<CreateOrReplaceSpellCommand, CreateOrReplaceSpellResult>
{
  private readonly IContext _context;
  private readonly IPermissionService _permissionService;
  private readonly ISpellQuerier _spellQuerier;
  private readonly ISpellRepository _spellRepository;

  public CreateOrReplaceSpellCommandHandler(
    IContext context,
    IPermissionService permissionService,
    ISpellQuerier spellQuerier,
    ISpellRepository spellRepository)
  {
    _context = context;
    _permissionService = permissionService;
    _spellQuerier = spellQuerier;
    _spellRepository = spellRepository;
  }

  public async Task<CreateOrReplaceSpellResult> HandleAsync(CreateOrReplaceSpellCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceSpellPayload payload = command.Payload;
    payload.Validate();

    ActorId? actorId = _context.ActorId;
    WorldId worldId = _context.WorldId;

    Spell? spell = null;
    SpellId spellId = SpellId.NewId(worldId);
    if (command.Id.HasValue)
    {
      spellId = new SpellId(worldId, command.Id.Value);
      spell = await _spellRepository.LoadAsync(spellId, cancellationToken);
    }

    Name name = new(payload.Name);

    bool created = false;
    if (spell is null)
    {
      await _permissionService.CheckAsync(Actions.CreateSpell, cancellationToken);

      spell = new Spell(spellId, new TalentTier(payload.Tier), name, actorId);
      created = true;
    }
    else
    {
      await _permissionService.CheckAsync(Actions.Update, spell, cancellationToken);

      if (spell.Tier.Value != payload.Tier)
      {
        throw new ImmutablePropertyException<int>(spell, spell.Tier.Value, payload.Tier, nameof(payload.Tier));
      }

      spell.Rename(name, actorId);
    }

    spell.Edit(Summary.TryCreate(payload.Summary), Content.TryCreate(payload.Content), actorId);

    await _spellRepository.SaveAsync(spell, cancellationToken);

    SpellModel model = await _spellQuerier.ReadAsync(spell, cancellationToken);
    return new CreateOrReplaceSpellResult(model, created);
  }
}
