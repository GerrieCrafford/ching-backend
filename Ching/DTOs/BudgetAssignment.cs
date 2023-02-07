namespace Ching.DTOs;

public record CreateBudgetAssignmentRequest(int BudgetCategoryId, BudgetMonthDTO BudgetMonth, decimal Amount, string? Note);