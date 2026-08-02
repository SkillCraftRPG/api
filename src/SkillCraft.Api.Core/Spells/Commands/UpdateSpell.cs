using Logitar.CQRS;
using Logitar.EventSourcing;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Spells.Models;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Spells.Commands;

internal record UpdateSpellCommand(Guid Id, UpdateSpellPayload Payload) : ICommand<SpellModel?>;

internal class UpdateSpellCommandHandler : ICommandHandler<UpdateSpellCommand, SpellModel?>
{
  private readonly IContext _context;
  private readonly IPermissionService _permissionService;
  private readonly ISpellQuerier _spellQuerier;
  private readonly ISpellRepository _spellRepository;

  public UpdateSpellCommandHandler(
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

  public async Task<SpellModel?> HandleAsync(UpdateSpellCommand command, CancellationToken cancellationToken)
  {
    UpdateSpellPayload payload = command.Payload;
    payload.Validate();

    ActorId? actorId = _context.ActorId;
    WorldId worldId = _context.WorldId;

    SpellId spellId = new(worldId, command.Id);
    Spell? spell = await _spellRepository.LoadAsync(spellId, cancellationToken);
    if (spell is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Update, spell, cancellationToken);

    Name? name = Name.TryCreate(payload.Name);
    if (name is not null)
    {
      spell.Rename(name, actorId);
    }

    if (payload.Summary is not null || payload.Content is not null)
    {
      spell.Edit(
        payload.Summary is null ? spell.Summary : Summary.TryCreate(payload.Summary.Value),
        payload.Content is null ? spell.Content : Content.TryCreate(payload.Content.Value),
        actorId);
    }

    await _spellRepository.SaveAsync(spell, cancellationToken);

    return await _spellQuerier.ReadAsync(spell, cancellationToken);
  }
}
