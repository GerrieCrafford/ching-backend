#pragma warning disable 8618
namespace Ching.Entities;

public class AccountTransaction : BaseEntity
{
    public DateOnly Date { get; set; }
    public Decimal Amount { get; set; }
    public string Recipient { get; set; }
    public string? Note { get; set; }
    public List<BudgetAssignmentTransaction> BudgetAssignments { get; set; }

    public int AccountPartitionId { get; set; }
    public AccountPartition AccountPartition { get; set; }

    public int AccountId { get; set; }
    public Account Account { get; set; }

    private AccountTransaction() { }

    public AccountTransaction(DateOnly date, decimal amount, Account account, AccountPartition partition, string recipient, string? note = null)
    {
        Date = date;
        Amount = amount;
        Account = account;
        AccountPartition = partition;
        Recipient = recipient;
        Note = note;
        BudgetAssignments = new List<BudgetAssignmentTransaction>();
    }
}