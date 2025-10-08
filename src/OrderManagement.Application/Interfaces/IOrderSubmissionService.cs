namespace FlexiMarket.OrderManagement.Application.Interfaces;

/// <summary>
/// Service d'orchestration pour la soumission de commandes.
/// Responsabilité unique : Orchestrer le workflow de soumission avec gestion des compensations.
/// </summary>
public interface IOrderSubmissionService
{
    Task<OrderSubmissionResult> SubmitOrderAsync(Guid orderId, CancellationToken cancellationToken);
}

public record OrderSubmissionResult(
    bool Success,
    string Message,
    string ReservationId = null,
    string PaymentAuthId = null)
{
    public static OrderSubmissionResult SuccessResult(string reservationId, string paymentAuthId) =>
        new(true, "Order submitted successfully", reservationId, paymentAuthId);

    public static OrderSubmissionResult Failed(string message) =>
        new(false, message);
}
