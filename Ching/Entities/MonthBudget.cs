namespace Ching.Entities;

public class MonthBudget : BaseEntity
{
    public Decimal Amount { get; set; }
    public BudgetMonth BudgetMonth { get; set; }

    public int BudgetCategoryId { get; set; }
    public BudgetCategory BudgetCategory { get; set; }
}