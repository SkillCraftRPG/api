using Logitar;
using Logitar.CQRS;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Spells.Events;
using SkillCraft.Api.Core.Spells.Models;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Spells.Commands;

internal record CreateOrReplaceSpellCommand(CreateOrReplaceSpellPayload Payload, Guid? Id) : ICommand<CreateOrReplaceSpellResult>;

internal class CreateOrReplaceSpellCommandHandler : ICommandHandler<CreateOrReplaceSpellCommand, CreateOrReplaceSpellResult>
{
  private readonly IContext _context;
  private readonly IPermissionService _permissionService;
  private readonly ISpellRepository _spellRepository;
  private readonly IWorldRepository _worldRepository;

  public CreateOrReplaceSpellCommandHandler(
    IContext context,
    IPermissionService permissionService,
    ISpellRepository spellRepository,
    IWorldRepository worldRepository)
  {
    _context = context;
    _permissionService = permissionService;
    _spellRepository = spellRepository;
    _worldRepository = worldRepository;
  }

  public async Task<CreateOrReplaceSpellResult> HandleAsync(CreateOrReplaceSpellCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceSpellPayload payload = command.Payload;
    payload.Validate();

    Spell? spell = null;
    if (command.Id.HasValue)
    {
      spell = await _spellRepository.LoadAsync(command.Id.Value, cancellationToken);
    }

    Guid userId = _context.UserUid;

    SpellSnapshot? snapshot = null;
    if (spell is null)
    {
      World world = await _worldRepository.LoadFromContextAsync(cancellationToken);
      await _permissionService.CheckAsync(Actions.CreateSpell, world, cancellationToken);

      spell = new Spell(world, payload.Tier, command.Id, userId);
      _spellRepository.Add(spell);
    }
    else
    {
      await _permissionService.CheckAsync(Actions.Update, spell, cancellationToken);

      if (payload.Tier != spell.Tier)
      {
        throw new ImmutablePropertyException<int>(spell, spell.Tier, payload.Tier, nameof(Spell.Tier));
      }

      snapshot = new SpellSnapshot(spell);
    }

    spell.Name = payload.Name.Trim();
    spell.Summary = payload.Summary?.CleanTrim();
    spell.Content = payload.Content?.CleanTrim();

    if (snapshot is not null)
    {
      SpellUpdated? record = snapshot.Compare(spell);
      if (record is not null)
      {
        spell.Update(userId);
        _spellRepository.Update(spell, record);
      }
    }

    await _context.SaveChangesAsync(cancellationToken);

    SpellModel model = await _spellRepository.ReadAsync(spell, cancellationToken);
    return new CreateOrReplaceSpellResult(model, Created: snapshot is null);
  }
}
