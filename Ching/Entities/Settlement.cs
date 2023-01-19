namespace Ching.Entities;

public class Settlement : BaseEntity
{
    public DateOnly Date { get; set; }
    public List<AccountTransaction> AccountTransactions { get; set; }

    public int TransferId { get; set; }
    public Transfer Transfer { get; set; }

    private Settlement() { }

    public Settlement(DateOnly date, Transfer transfer, List<AccountTransaction>? accountTransactions)
    {
        Date = date;
        Transfer = transfer;
        AccountTransactions = accountTransactions ?? new List<AccountTransaction>();
    }

    public Settlement(DateOnly date, Transfer transfer)
    : this(date, transfer, null) { }
}