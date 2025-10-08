namespace FlexiMarket.SharedKernel;

public record WorkflowResult(bool IsSuccess, string Message)
{
    public static WorkflowResult Success() => new(true, "Workflow completed");
    public static WorkflowResult Failed(string message) => new(false, message);
}
