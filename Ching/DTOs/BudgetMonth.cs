using AutoMapper;

using Ching.Entities;
namespace Ching.DTOs;

public record BudgetMonthDTO(int Year, int Month);

public class BudgetMonthDTOMappingProfile : Profile
{
    public BudgetMonthDTOMappingProfile()
    {
        CreateMap<BudgetMonthDTO, BudgetMonth>();
    }
}