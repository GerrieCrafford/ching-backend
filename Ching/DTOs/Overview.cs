namespace Ching.DTOs;

public record PartitionBalanceDTO(int PartitionId, string PartitionName, decimal Balance);

public record TransactionWithBalanceDTO(TransactionDTO Transaction, decimal Balance);

public record BudgetOverviewItemDTO(int CategoryId, string CategoryName, decimal Spent, decimal Available);