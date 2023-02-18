namespace Ching.Entities;

public class BudgetAssignmentTransfer : BaseEntity
{
    public int BudgetCategoryId { get; set; }
    public decimal Amount { get; set; }
    public string? Note { get; set; }
    public BudgetCategory BudgetCategory { get; set; }
    public BudgetMonth BudgetMonth { get; set; }
    public int TransferId { get; set; }
    public Transfer Transfer { get; set; }

    private BudgetAssignmentTransfer() { }

    public BudgetAssignmentTransfer(
        BudgetCategory category,
        decimal amount,
        BudgetMonth budgetMonth,
        string? note = null
    )
    {
        BudgetCategory = category;
        Amount = amount;
        BudgetMonth = budgetMonth;
        Note = note;
    }
}
