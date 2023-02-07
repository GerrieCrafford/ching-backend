namespace Ching.DTOs;

public record CreateTransferRequest(DateOnly Date, decimal Amount, int SourcePartitionId, int DestinationPartitionId);
public record CreateSavingsPaymentRequest(DateOnly Date, decimal Amount, int SourcePartitionId, int DestinationPartitionId, CreateBudgetAssignmentRequest? BudgetAssignmentRequest);