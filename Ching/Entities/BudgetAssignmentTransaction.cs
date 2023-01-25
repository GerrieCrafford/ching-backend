namespace Ching.Entities;

public class BudgetAssignmentTransaction : BudgetAssignment
{
    private BudgetAssignmentTransaction() { }
    public BudgetAssignmentTransaction(BudgetCategory category, decimal amount, BudgetMonth budgetMonth)
    {
        BudgetCategory = category;
        Amount = amount;
        BudgetMonth = budgetMonth;
    }
}