namespace Ching.Entities;

public class AccountPartition : BaseEntity
{
    public string Name { get; private set; }
    public string Type { get; private set; }
    public BudgetMonth BudgetMonth { get; private set; }

    public int AccountId { get; private set; }
    public Account Account { get; private set; }

    private AccountPartition() { }
}