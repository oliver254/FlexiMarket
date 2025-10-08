using FlexiMarket.OrderManagement.Application.DTOs;
using FlexiMarket.OrderManagement.Domain.ValueObjects;

namespace FlexiMarket.OrderManagement.Application.Interfaces;

public interface IPaymentService
{
    Task<PaymentAuthorizationResult> PreAuthorizeAsync(CustomerId customerId, Money amount, CancellationToken cancellationToken);
    Task CancelPreAuthorizationAsync(string authorizationId, CancellationToken cancellationToken);
}