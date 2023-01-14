namespace Ching.Entities;

public class Settlement : BaseEntity
{
    public DateOnly Date { get; set; }
    public ICollection<AccountTransaction> AccountTransactions { get; set; }

    public int TransferId { get; set; }
    public Transfer Transfer { get; set; }
}