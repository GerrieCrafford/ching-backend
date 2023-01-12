namespace Ching.Entities;

public class Settlement : BaseEntity
{
    public DateOnly Date { get; private set; }
    public ICollection<AccountTransaction> AccountTransactions { get; private set; }

    public int TransferId { get; private set; }
    public Transfer Transfer { get; private set; }

    private Settlement() { }
}