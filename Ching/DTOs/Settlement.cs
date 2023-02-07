namespace Ching.DTOs;

public record CreateSettlementRequest(DateOnly Date, List<int> AccountTransactionIds, int SourcePartitionId);