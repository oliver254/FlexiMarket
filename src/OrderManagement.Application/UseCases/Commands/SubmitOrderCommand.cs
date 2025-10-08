using FlexiMarket.OrderManagement.Application.Interfaces;
using FlexiMarket.SharedKernel.Interfaces;

namespace FlexiMarket.OrderManagement.Application.UseCases.Commands;

public record SubmitOrderCommand(Guid OrderId) : ICommand<SubmitOrderResult>;

public record SubmitOrderResult(bool Success, string Message, string ReservationId = null, string PaymentAuthId = null);


// Use case métier complexe : soumission avec validations multiples
public class SubmitOrderCommandHandler : ICommandHandler<SubmitOrderCommand, SubmitOrderResult>
{
    private readonly IOrderSubmissionService _submissionService;

    // ✅ UNE SEULE dépendance - Respecte le Dependency Inversion Principle
    public SubmitOrderCommandHandler(IOrderSubmissionService submissionService)
    {
        _submissionService = submissionService;
    }

    public async Task<SubmitOrderResult> Handle(SubmitOrderCommand command, CancellationToken cancellationToken)
    {
        var result = await _submissionService.SubmitOrderAsync(command.OrderId, cancellationToken);

        return new SubmitOrderResult(
            result.Success,
            result.Message,
            result.ReservationId,
            result.PaymentAuthId);
    }
}

