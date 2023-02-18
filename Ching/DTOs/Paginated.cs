namespace Ching.DTOs;

public record PaginatedDTO<T>(List<T> Items, int TotalItems);
