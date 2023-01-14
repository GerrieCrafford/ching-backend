namespace Ching.Entities;

public class BudgetAssignment : BaseEntity
{
    public int BudgetCategoryId { get; set; }
    public BudgetCategory BudgetCategory { get; set; }
    public BudgetMonth BudgetMonth { get; set; }
}