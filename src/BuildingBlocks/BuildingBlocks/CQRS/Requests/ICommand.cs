using MediatR;

namespace BuildingBlocks.CQRS.Requests;

public interface ICommand : ICommand<Unit>
{
}

public interface ICommand<out TResponse> : IRequest<TResponse>
{
}
