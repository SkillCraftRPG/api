using Logitar.CQRS;
using SkillCraft.Api.Contracts.Parties;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Storages;

namespace SkillCraft.Api.Core.Parties.Commands;

internal record DeletePartyCommand(Guid Id) : ICommand<PartyModel?>;

internal class DeletePartyCommandHandler : ICommandHandler<DeletePartyCommand, PartyModel?>
{
  private readonly IContext _context;
  private readonly IPartyQuerier _partyQuerier;
  private readonly IPartyRepository _partyRepository;
  private readonly IPermissionService _permissionService;
  private readonly IStorageService _storageService;

  public DeletePartyCommandHandler(
    IContext context,
    IPartyQuerier partyQuerier,
    IPartyRepository partyRepository,
    IPermissionService permissionService,
    IStorageService storageService)
  {
    _context = context;
    _partyQuerier = partyQuerier;
    _partyRepository = partyRepository;
    _permissionService = permissionService;
    _storageService = storageService;
  }

  public async Task<PartyModel?> HandleAsync(DeletePartyCommand command, CancellationToken cancellationToken)
  {
    PartyId partyId = new(command.Id, _context.WorldId);
    Party? party = await _partyRepository.LoadAsync(partyId, cancellationToken);
    if (party is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Delete, party, cancellationToken);
    PartyModel model = await _partyQuerier.ReadAsync(party, cancellationToken);

    party.Delete(_context.UserId);

    await _storageService.ExecuteWithQuotaAsync(
      party,
      async () => await _partyRepository.SaveAsync(party, cancellationToken),
      cancellationToken);

    return model;
  }
}
