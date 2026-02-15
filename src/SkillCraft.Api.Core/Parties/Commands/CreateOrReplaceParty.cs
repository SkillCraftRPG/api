using FluentValidation;
using Logitar.CQRS;
using SkillCraft.Api.Contracts.Parties;
using SkillCraft.Api.Core.Parties.Validators;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Storages;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Parties.Commands;

internal record CreateOrReplacePartyCommand(CreateOrReplacePartyPayload Payload, Guid? Id) : ICommand<CreateOrReplacePartyResult>;

internal class CreateOrReplacePartyCommandHandler : ICommandHandler<CreateOrReplacePartyCommand, CreateOrReplacePartyResult>
{
  private readonly IContext _context;
  private readonly IPartyQuerier _partyQuerier;
  private readonly IPartyRepository _partyRepository;
  private readonly IPermissionService _permissionService;
  private readonly IStorageService _storageService;

  public CreateOrReplacePartyCommandHandler(
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

  public async Task<CreateOrReplacePartyResult> HandleAsync(CreateOrReplacePartyCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplacePartyPayload payload = command.Payload;
    new CreateOrReplacePartyValidator().ValidateAndThrow(payload);

    UserId userId = _context.UserId;
    WorldId worldId = _context.WorldId;

    PartyId partyId = PartyId.NewId(worldId);
    Party? party = null;
    if (command.Id.HasValue)
    {
      partyId = new(command.Id.Value, worldId);
      party = await _partyRepository.LoadAsync(partyId, cancellationToken);
    }

    Name name = new(payload.Name);

    bool created = false;
    if (party is null)
    {
      await _permissionService.CheckAsync(Actions.CreateParty, cancellationToken);

      party = new Party(worldId, name, userId, partyId);
      created = true;
    }
    else
    {
      await _permissionService.CheckAsync(Actions.Update, party, cancellationToken);

      party.Name = name;
    }

    party.Description = Description.TryCreate(payload.Description);

    party.Update(userId);

    await _storageService.ExecuteWithQuotaAsync(
      party,
      async () => await _partyRepository.SaveAsync(party, cancellationToken),
      cancellationToken);

    PartyModel model = await _partyQuerier.ReadAsync(party, cancellationToken);
    return new CreateOrReplacePartyResult(model, created);
  }
}
