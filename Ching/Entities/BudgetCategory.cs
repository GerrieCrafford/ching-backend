#pragma warning disable 8618
namespace Ching.Entities;

public class BudgetCategory : BaseEntity
{
    public string Name { get; set; }

    private BudgetCategory() { }

    public BudgetCategory(string name)
    {
        Name = name;
    }
}