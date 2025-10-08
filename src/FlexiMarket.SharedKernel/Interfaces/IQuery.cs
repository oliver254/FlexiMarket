using MediatR;

namespace FlexiMarket.SharedKernel.Interfaces;

public interface IQuery<TResponse> : IRequest<TResponse> { }
