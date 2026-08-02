using Logitar;
using Logitar.CQRS;
using SkillCraft.Api.Core.Items.Events;
using SkillCraft.Api.Core.Items.Models;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Items.Commands;

internal record CreateOrReplaceItemCommand(CreateOrReplaceItemPayload Payload, Guid? Id) : ICommand<CreateOrReplaceItemResult>;

internal class CreateOrReplaceItemCommandHandler : ICommandHandler<CreateOrReplaceItemCommand, CreateOrReplaceItemResult>
{
  private readonly IContext _context;
  private readonly IItemRepository _itemRepository;
  private readonly IPermissionService _permissionService;
  private readonly IWorldRepository _worldRepository;

  public CreateOrReplaceItemCommandHandler(
    IContext context,
    IItemRepository itemRepository,
    IPermissionService permissionService,
    IWorldRepository worldRepository)
  {
    _context = context;
    _itemRepository = itemRepository;
    _permissionService = permissionService;
    _worldRepository = worldRepository;
  }

  public async Task<CreateOrReplaceItemResult> HandleAsync(CreateOrReplaceItemCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceItemPayload payload = command.Payload;
    payload.Validate();

    Item? item = null;
    if (command.Id.HasValue)
    {
      item = await _itemRepository.LoadAsync(command.Id.Value, cancellationToken);
    }

    Guid userId = _context.UserId;
    Guid worldId = _context.WorldUid;

    ItemSnapshot? snapshot = null;
    if (item is null)
    {
      World world = await _worldRepository.LoadAsync(worldId, cancellationToken)
        ?? throw new InvalidOperationException($"The world 'Id={worldId}' was not found.");
      await _permissionService.CheckAsync(Actions.CreateItem, world, cancellationToken);

      item = new Item(world, command.Id, userId);
      _itemRepository.Add(item);
    }
    else
    {
      await _permissionService.CheckAsync(Actions.Update, item, cancellationToken);

      snapshot = new ItemSnapshot(item);
    }

    item.Name = payload.Name.Trim();
    item.Summary = payload.Summary?.CleanTrim();
    item.Content = payload.Content?.CleanTrim();

    item.Price = payload.Price;
    item.Weight = payload.Weight;

    if (snapshot is not null)
    {
      ItemUpdated? record = snapshot.Compare(item);
      if (record is not null)
      {
        item.Update(userId);
        _itemRepository.Update(item, record);
      }
    }

    await _context.SaveChangesAsync(cancellationToken);

    ItemModel model = await _itemRepository.ReadAsync(item, cancellationToken);
    return new CreateOrReplaceItemResult(model, Created: snapshot is null);
  }
}
