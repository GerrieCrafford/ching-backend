#pragma warning disable 8618
namespace Ching.Entities;

public class MonthBudget : BaseEntity
{
    public Decimal Amount { get; set; }
    public BudgetMonth BudgetMonth { get; set; }

    public int BudgetCategoryId { get; set; }
    public BudgetCategory BudgetCategory { get; set; }

    private MonthBudget() { }

    public MonthBudget(decimal amount, BudgetMonth budgetMonth, BudgetCategory budgetCategory)
    {
        Amount = amount;
        BudgetMonth = budgetMonth;
        BudgetCategory = budgetCategory;
    }
}