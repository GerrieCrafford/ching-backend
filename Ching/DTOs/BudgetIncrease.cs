namespace Ching.DTOs;

public record CreateBudgetIncreaseTransferRequest(DateOnly Date, decimal Amount, int SourcePartitionId, int DestinationPartitionId);
public record CreateBudgetIncreaseRequest(int BudgetCategoryId, BudgetMonthDTO BudgetMonth, CreateBudgetIncreaseTransferRequest Transfer);