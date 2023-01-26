namespace Ching.Entities;

public class BudgetAssignmentTransfer : BudgetAssignment
{
    private BudgetAssignmentTransfer() { }
    public BudgetAssignmentTransfer(BudgetCategory category, decimal amount, BudgetMonth budgetMonth, string? note = null)
    {
        BudgetCategory = category;
        Amount = amount;
        BudgetMonth = budgetMonth;
        Note = note;
    }
}