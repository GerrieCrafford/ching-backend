namespace Ching.DTOs;

public record TransactionDTO(DateOnly Date, decimal Amount, string? Note);