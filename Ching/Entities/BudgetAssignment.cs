namespace Ching.Entities;

public class BudgetAssignment : BaseEntity
{
    public int BudgetCategoryId { get; private set; }
    public BudgetCategory BudgetCategory { get; private set; }
    public BudgetMonth BudgetMonth { get; private set; }

    protected BudgetAssignment() { }
}