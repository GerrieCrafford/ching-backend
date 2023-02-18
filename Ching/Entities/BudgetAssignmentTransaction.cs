namespace Ching.Entities;

public class BudgetAssignmentTransaction : BaseEntity
{
    public int BudgetCategoryId { get; set; }
    public decimal Amount { get; set; }
    public string? Note { get; set; }
    public BudgetCategory BudgetCategory { get; set; }
    public BudgetMonth BudgetMonth { get; set; }
    public int AccountTransactionId { get; set; }
    public AccountTransaction AccountTransaction { get; set; }

    private BudgetAssignmentTransaction() { }

    public BudgetAssignmentTransaction(
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
