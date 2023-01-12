namespace Ching.Entities;

public class Account : BaseEntity
{
    public string Name { get; set; }
    public ICollection<AccountTransaction> Transactions { get; set; }
    public ICollection<AccountPartition> Partitions { get; set; }

    private Account() { }
}