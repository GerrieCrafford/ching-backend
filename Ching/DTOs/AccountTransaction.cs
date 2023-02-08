namespace Ching.DTOs;

public record EditAccountTransactionRequest(DateOnly Date, string? Note, string Recipient, List<CreateBudgetAssignmentRequest> BudgetAssignments);