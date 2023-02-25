using Ching.Entities;
using AutoMapper;

namespace Ching.DTOs;

public record AccountPartitionDTO(
    int Id,
    string Name,
    bool Archived,
    BudgetMonthDTO? BudgetMonth = null
);

public class AccountPartitionMappingProfile : Profile
{
    public AccountPartitionMappingProfile()
    {
        CreateMap<AccountPartition, AccountPartitionDTO>();
    }
}
