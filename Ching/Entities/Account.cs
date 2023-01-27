namespace Ching.Entities;

public class Account : BaseEntity
{
    public string Name { get; set; }
    public List<AccountTransaction> Transactions { get; set; }
    public List<AccountPartition> Partitions { get; set; }

    private Account() { }

    public Account(string name)
    {
        Name = name;
        Transactions = new List<AccountTransaction>();
        Partitions = new List<AccountPartition> { new AccountPartition("Remaining") };
    }

    public AccountPartition RemainingPartition
    {
        get
        {
            return Partitions.Where(p => p.Name == "Remaining").First();
        }
    }
}