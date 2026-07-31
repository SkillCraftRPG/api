using Logitar;
using Logitar.CQRS;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Spells.Events;
using SkillCraft.Api.Core.Spells.Models;

namespace SkillCraft.Api.Core.Spells.Commands;

internal record UpdateSpellCommand(Guid Id, UpdateSpellPayload Payload) : ICommand<SpellModel?>;

internal class UpdateSpellCommandHandler : ICommandHandler<UpdateSpellCommand, SpellModel?>
{
  private readonly IContext _context;
  private readonly IPermissionService _permissionService;
  private readonly ISpellRepository _spellRepository;

  public UpdateSpellCommandHandler(IContext context, IPermissionService permissionService, ISpellRepository spellRepository)
  {
    _context = context;
    _permissionService = permissionService;
    _spellRepository = spellRepository;
  }

  public async Task<SpellModel?> HandleAsync(UpdateSpellCommand command, CancellationToken cancellationToken)
  {
    UpdateSpellPayload payload = command.Payload;
    payload.Validate();

    Spell? spell = await _spellRepository.LoadAsync(command.Id, cancellationToken);
    if (spell is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Update, spell, cancellationToken);

    SpellSnapshot snapshot = new(spell);

    if (!string.IsNullOrWhiteSpace(payload.Name))
    {
      spell.Name = payload.Name.Trim();
    }
    if (payload.Summary is not null)
    {
      spell.Summary = payload.Summary.Value?.CleanTrim();
    }
    if (payload.Content is not null)
    {
      spell.Content = payload.Content.Value?.CleanTrim();
    }

    SpellUpdated? record = snapshot.Compare(spell);
    if (record is not null)
    {
      spell.Update(_context.UserId);
      _spellRepository.Update(spell, record);

      await _context.SaveChangesAsync(cancellationToken);
    }

    return await _spellRepository.ReadAsync(spell, cancellationToken);
  }
}
