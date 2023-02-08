#pragma warning disable 8618
namespace Ching.Entities;

public class BudgetIncrease : BaseEntity
{
    public Transfer Transfer { get; set; }
    public BudgetMonth BudgetMonth { get; set; }

    public int BudgetCategoryId { get; set; }
    public BudgetCategory BudgetCategory { get; set; }

    private BudgetIncrease() { }

    public BudgetIncrease(Transfer transfer, BudgetMonth budgetMonth, BudgetCategory budgetCategory)
    {
        Transfer = transfer;
        BudgetMonth = budgetMonth;
        BudgetCategory = budgetCategory;
    }
}