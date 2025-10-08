using MediatR;

namespace FlexiMarket.SharedKernel.Interfaces;

public interface ICommand<TResponse> : IRequest<TResponse> { }

