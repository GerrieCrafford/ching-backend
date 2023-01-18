namespace Ching.Entities;

public abstract class BaseEntity : IEntity
{
    public virtual int Id { get; set; }
}