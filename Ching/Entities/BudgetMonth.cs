namespace Ching.Entities;

public class BudgetMonth
{
    public int Year { get; private set; }
    public int Month { get; private set; }

    public BudgetMonth(int year, int month)
    {
        this.Year = year;
        this.Month = month;
    }
}