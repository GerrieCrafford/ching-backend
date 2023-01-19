namespace Ching.Entities;

public class BudgetAssignmentTransfer : BudgetAssignment
{
    private BudgetAssignmentTransfer() { }
    public BudgetAssignmentTransfer(BudgetCategory category, decimal amount, BudgetMonth budgetMonth)
    {
        BudgetCategory = category;
        Amount = amount;
        BudgetMonth = budgetMonth;
    }
}