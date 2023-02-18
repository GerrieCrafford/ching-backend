namespace Ching.DTOs;

public record AccountDTO(int Id, string Name, List<AccountPartitionDTO> Partitions);
