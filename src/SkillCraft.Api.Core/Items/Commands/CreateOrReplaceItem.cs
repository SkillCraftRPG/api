using Logitar.CQRS;
using Logitar.EventSourcing;
using SkillCraft.Api.Core.Items.Models;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Items.Commands;

internal record CreateOrReplaceItemCommand(CreateOrReplaceItemPayload Payload, Guid? Id) : ICommand<CreateOrReplaceItemResult>;

internal class CreateOrReplaceItemCommandHandler : ICommandHandler<CreateOrReplaceItemCommand, CreateOrReplaceItemResult>
{
  private readonly IContext _context;
  private readonly IItemQuerier _itemQuerier;
  private readonly IItemRepository _itemRepository;
  private readonly IPermissionService _permissionService;

  public CreateOrReplaceItemCommandHandler(
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

  public async Task<CreateOrReplaceItemResult> HandleAsync(CreateOrReplaceItemCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceItemPayload payload = command.Payload;
    payload.Validate();

    ActorId? actorId = _context.ActorId;
    WorldId worldId = _context.WorldId;

    Item? item = null;
    ItemId itemId = ItemId.NewId(worldId);
    if (command.Id.HasValue)
    {
      itemId = new ItemId(worldId, command.Id.Value);
      item = await _itemRepository.LoadAsync(itemId, cancellationToken);
    }

    Name name = new(payload.Name);

    bool created = false;
    if (item is null)
    {
      await _permissionService.CheckAsync(Actions.CreateItem, cancellationToken);

      item = new Item(itemId, name, actorId);
      created = true;
    }
    else
    {
      await _permissionService.CheckAsync(Actions.Update, item, cancellationToken);

      item.Rename(name, actorId);
    }

    item.Edit(Summary.TryCreate(payload.Summary), Content.TryCreate(payload.Content), actorId);
    item.SetRules(Price.TryCreate(payload.Price), Weight.TryCreate(payload.Weight), actorId);

    await _itemRepository.SaveAsync(item, cancellationToken);

    ItemModel model = await _itemQuerier.ReadAsync(item, cancellationToken);
    return new CreateOrReplaceItemResult(model, created);
  }
}
