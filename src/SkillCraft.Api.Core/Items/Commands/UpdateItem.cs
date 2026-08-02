using Logitar.CQRS;
using Logitar.EventSourcing;
using SkillCraft.Api.Core.Items.Models;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Items.Commands;

internal record UpdateItemCommand(Guid Id, UpdateItemPayload Payload) : ICommand<ItemModel?>;

internal class UpdateItemCommandHandler : ICommandHandler<UpdateItemCommand, ItemModel?>
{
  private readonly IContext _context;
  private readonly IItemQuerier _itemQuerier;
  private readonly IItemRepository _itemRepository;
  private readonly IPermissionService _permissionService;

  public UpdateItemCommandHandler(
    IContext context,
    IItemQuerier itemQuerier,
    IItemRepository itemRepository,
    IPermissionService permissionService)
  {
    _context = context;
    _itemQuerier = itemQuerier;
    _itemRepository = itemRepository;
    _permissionService = permissionService;
  }

  public async Task<ItemModel?> HandleAsync(UpdateItemCommand command, CancellationToken cancellationToken)
  {
    UpdateItemPayload payload = command.Payload;
    payload.Validate();

    ActorId? actorId = _context.ActorId;
    WorldId worldId = _context.WorldId;

    ItemId itemId = new(worldId, command.Id);
    Item? item = await _itemRepository.LoadAsync(itemId, cancellationToken);
    if (item is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Update, item, cancellationToken);

    Name? name = Name.TryCreate(payload.Name);
    if (name is not null)
    {
      item.Rename(name, actorId);
    }

    if (payload.Summary is not null || payload.Content is not null)
    {
      item.Edit(
        payload.Summary is null ? item.Summary : Summary.TryCreate(payload.Summary.Value),
        payload.Content is null ? item.Content : Content.TryCreate(payload.Content.Value),
        actorId);
    }

    if (payload.Price is not null || payload.Weight is not null)
    {
      item.SetRules(
        payload.Price is null ? item.Price : Price.TryCreate(payload.Price.Value),
        payload.Weight is null ? item.Weight : Weight.TryCreate(payload.Weight.Value),
        actorId);
    }

    await _itemRepository.SaveAsync(item, cancellationToken);

    return await _itemQuerier.ReadAsync(item, cancellationToken);
  }
}
