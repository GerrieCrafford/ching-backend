namespace Ching.Entities;

public class MonthBudget : BaseEntity
{
    public Decimal Amount { get; private set; }
    public BudgetMonth BudgetMonth { get; private set; }

    public int BudgetCategoryId { get; private set; }
    public BudgetCategory BudgetCategory { get; private set; }

    private MonthBudget() { }
}