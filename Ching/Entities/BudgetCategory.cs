namespace Ching.Entities;

public class BudgetCategory : BaseEntity
{
    public string Name { get; private set; }

    private BudgetCategory() { }
}