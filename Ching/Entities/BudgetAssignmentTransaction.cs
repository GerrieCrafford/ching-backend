namespace Ching.Entities;

public class BudgetAssignmentTransaction : BudgetAssignment
{
    private BudgetAssignmentTransaction() { }
    public BudgetAssignmentTransaction(BudgetCategory category, decimal amount, BudgetMonth budgetMonth, string? note = null)
    {
        BudgetCategory = category;
        Amount = amount;
        BudgetMonth = budgetMonth;
        Note = note;
    }
}