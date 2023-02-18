namespace Ching.DTOs;

public record CreateBudgetAssignmentRequest(
    int BudgetCategoryId,
    BudgetMonthDTO BudgetMonth,
    decimal Amount,
    string? Note
);

public enum BudgetAssignmentType
{
    Transaction,
    Transfer
}

public record BudgetAssignmentDTO(
    int Id,
    DateOnly Date,
    BudgetAssignmentType Type,
    int BudgetCategoryId,
    string BudgetCategoryName,
    BudgetMonthDTO BudgetMonth,
    decimal Amount,
    string Recipient,
    string? Note = null
);
