namespace Ching.DTOs;

public record AccountPartitionDTO(int Id, bool Archived, BudgetMonthDTO? BudgetMonth);

public record CreateAccountPartitionRequest(int AccountId, string Name, BudgetMonthDTO? BudgetMonth);