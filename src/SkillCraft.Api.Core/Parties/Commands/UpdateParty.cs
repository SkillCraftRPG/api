using FluentValidation;
using Logitar.CQRS;
using SkillCraft.Api.Contracts.Parties;
using SkillCraft.Api.Core.Parties.Validators;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Storages;

namespace SkillCraft.Api.Core.Parties.Commands;

internal record UpdatePartyCommand(Guid Id, UpdatePartyPayload Payload) : ICommand<PartyModel?>;

internal class UpdatePartyCommandHandler : ICommandHandler<UpdatePartyCommand, PartyModel?>
{
  private readonly IContext _context;
  private readonly IPartyQuerier _partyQuerier;
  private readonly IPartyRepository _partyRepository;
  private readonly IPermissionService _permissionService;
  private readonly IStorageService _storageService;

  public UpdatePartyCommandHandler(
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

  public async Task<PartyModel?> HandleAsync(UpdatePartyCommand command, CancellationToken cancellationToken)
  {
    UpdatePartyPayload payload = command.Payload;
    new UpdatePartyValidator().ValidateAndThrow(payload);

    PartyId partyId = new(command.Id, _context.WorldId);
    Party? party = await _partyRepository.LoadAsync(partyId, cancellationToken);
    if (party is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Update, party, cancellationToken);

    if (!string.IsNullOrWhiteSpace(payload.Name))
    {
      party.Name = new Name(payload.Name);
    }
    if (payload.Description is not null)
    {
      party.Description = Description.TryCreate(payload.Description.Value);
    }

    party.Update(_context.UserId);

    await _storageService.ExecuteWithQuotaAsync(
      party,
      async () => await _partyRepository.SaveAsync(party, cancellationToken),
      cancellationToken);

    return await _partyQuerier.ReadAsync(party, cancellationToken);
  }
}
