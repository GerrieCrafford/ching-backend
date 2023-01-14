namespace Ching.Entities;

public class AccountPartition : BaseEntity
{
    public string Name { get; set; }
    public string Type { get; set; }
    public BudgetMonth BudgetMonth { get; set; }

    public int AccountId { get; set; }
    public Account Account { get; set; }
}