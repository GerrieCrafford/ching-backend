namespace Ching.DTOs;

public record CreateMonthBudgetRequest(int BudgetCategoryId, decimal Amount, BudgetMonthDTO BudgetMonth);