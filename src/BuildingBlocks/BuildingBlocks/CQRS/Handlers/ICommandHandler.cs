using BuildingBlocks.CQRS.Requests;

using MediatR;

namespace BuildingBlocks.CQRS.Handlers;

public interface ICommandHandler<in TCommand> : ICommandHandler<TCommand , Unit>
    where TCommand : ICommand
{
}
public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand , TResponse>
    where TCommand : ICommand<TResponse>
    where TResponse : notnull
{
}