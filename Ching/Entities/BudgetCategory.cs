#pragma warning disable 8618
namespace Ching.Entities;

public class BudgetCategory : BaseEntity
{
    public string Name { get; set; }

    public int? ParentId { get; set; }
    public BudgetCategory? Parent { get; set; }

    private BudgetCategory() { }

    public BudgetCategory(string name, BudgetCategory? parent = null)
    {
        Name = name;
        Parent = parent;
    }
}