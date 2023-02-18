using Ching.Entities;
using AutoMapper;

namespace Ching.DTOs;

public record AccountPartitionDTO(int Id, bool Archived, BudgetMonthDTO? BudgetMonth);

public class AccountPartitionMappingProfile : Profile
{
    public AccountPartitionMappingProfile()
    {
        CreateMap<AccountPartition, AccountPartitionDTO>();
    }
}
