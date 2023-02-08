#pragma warning disable 8618
namespace Ching.Entities;

public class BudgetAssignment : BaseEntity
{
    public int BudgetCategoryId { get; set; }
    public decimal Amount { get; set; }
    public string? Note { get; set; }
    public BudgetCategory BudgetCategory { get; set; }
    public BudgetMonth BudgetMonth { get; set; }

    protected BudgetAssignment() { }

    public BudgetAssignment(BudgetCategory category, decimal amount, BudgetMonth budgetMonth, string? note = null)
    {
        BudgetCategory = category;
        Amount = amount;
        BudgetMonth = budgetMonth;
        Note = note;
    }
}