using FluentValidation;
using Logitar.CQRS;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Storages;

namespace SkillCraft.Api.Core;

internal class CommandBus : Logitar.CQRS.CommandBus
{
  public CommandBus(IServiceProvider serviceProvider) : base(serviceProvider)
  {
  }

  protected override bool ShouldRetry<TResult>(ICommand<TResult> command, Exception exception) => exception is not NotEnoughStorageException
    && exception is not NotFoundException
    && exception is not PermissionDeniedException
    && exception is not ValidationException;
}
