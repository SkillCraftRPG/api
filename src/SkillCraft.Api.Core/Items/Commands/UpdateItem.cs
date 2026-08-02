using Logitar;
using Logitar.CQRS;
using SkillCraft.Api.Core.Items.Events;
using SkillCraft.Api.Core.Items.Models;
using SkillCraft.Api.Core.Permissions;

namespace SkillCraft.Api.Core.Items.Commands;

internal record UpdateItemCommand(Guid Id, UpdateItemPayload Payload) : ICommand<ItemModel?>;

internal class UpdateItemCommandHandler : ICommandHandler<UpdateItemCommand, ItemModel?>
{
  private readonly IContext _context;
  private readonly IPermissionService _permissionService;
  private readonly IItemRepository _itemRepository;

  public UpdateItemCommandHandler(IContext context, IPermissionService permissionService, IItemRepository itemRepository)
  {
    _context = context;
    _permissionService = permissionService;
    _itemRepository = itemRepository;
  }

  public async Task<ItemModel?> HandleAsync(UpdateItemCommand command, CancellationToken cancellationToken)
  {
    UpdateItemPayload payload = command.Payload;
    payload.Validate();

    Item? item = await _itemRepository.LoadAsync(command.Id, cancellationToken);
    if (item is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Update, item, cancellationToken);

    ItemSnapshot snapshot = new(item);

    if (!string.IsNullOrWhiteSpace(payload.Name))
    {
      item.Name = payload.Name.Trim();
    }
    if (payload.Summary is not null)
    {
      item.Summary = payload.Summary.Value?.CleanTrim();
    }
    if (payload.Content is not null)
    {
      item.Content = payload.Content.Value?.CleanTrim();
    }

    if (payload.Price is not null)
    {
      item.Price = payload.Price.Value;
    }
    if (payload.Weight is not null)
    {
      item.Weight = payload.Weight.Value;
    }

    ItemUpdated? record = snapshot.Compare(item);
    if (record is not null)
    {
      item.Update(_context.UserUid);
      _itemRepository.Update(item, record);

      await _context.SaveChangesAsync(cancellationToken);
    }

    return await _itemRepository.ReadAsync(item, cancellationToken);
  }
}
