using MediatR;

namespace FlexiMarket.SharedKernel.Interfaces;

public interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{ 
}
