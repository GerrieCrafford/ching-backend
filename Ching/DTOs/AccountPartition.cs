namespace Ching.DTOs;

public record AccountPartitionDTO(int Id, bool Archived, BudgetMonthDTO? BudgetMonth);