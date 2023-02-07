namespace Ching.DTOs;

public record CreateAccountTransactionRequest(int AccountPartitionId, DateOnly Date, decimal Amount, string Recipient, string? Note);

public record CreateAccountTransactionFromAssignmentsRequest(int AccountPartitionId, DateOnly Date, string Recipient, List<CreateBudgetAssignmentRequest> BudgetAssignments);

public record EditAccountTransactionRequest(DateOnly Date, string? Note, string Recipient, List<CreateBudgetAssignmentRequest> BudgetAssignments);