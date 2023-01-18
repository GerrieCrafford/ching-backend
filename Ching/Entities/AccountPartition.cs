namespace Ching.Entities;

public class AccountPartition : BaseEntity
{
    public string Name { get; set; }
    public Boolean Archived { get; set; }
    public BudgetMonth? BudgetMonth { get; set; }

    public int AccountId { get; set; }
    public Account Account { get; set; }

    private AccountPartition() { }

    public AccountPartition(string name) : this(name, null) { }
    public AccountPartition(string name, BudgetMonth? budgetMonth)
    {
        Name = name;
        Archived = false;
        BudgetMonth = budgetMonth;
    }
}