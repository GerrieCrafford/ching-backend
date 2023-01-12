namespace Ching.Entities;

public class BudgetIncrease : BaseEntity
{
    public Transfer Transfer { get; private set; }
    public BudgetMonth BudgetMonth { get; private set; }

    public int BudgetCategoryId { get; private set; }
    public BudgetCategory BudgetCategory { get; private set; }

    private BudgetIncrease() { }
}