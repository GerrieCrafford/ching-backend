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

    /// <summary>
    /// Creates a new budget month for the next month
    /// </summary>
    /// <param name="prev">Previous month</param>
    public BudgetMonth(BudgetMonth prev)
    {
        if (prev.Month == 12)
        {
            this.Year = prev.Year + 1;
            this.Month = 1;
        }
        else
        {
            this.Year = prev.Year;
            this.Month = prev.Month + 1;
        }
    }
}