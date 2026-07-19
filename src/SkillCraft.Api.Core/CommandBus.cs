using FluentValidation;
using Logitar.CQRS;

namespace SkillCraft.Api.Core;

internal class CommandBus : Logitar.CQRS.CommandBus
{
  public CommandBus(IServiceProvider serviceProvider) : base(serviceProvider)
  {
  }

  protected override bool ShouldRetry<TResult>(ICommand<TResult> command, Exception exception) => exception is not ValidationException;
}
