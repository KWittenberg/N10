namespace N10.Entities;

public abstract class BaseEntity<TKey>
{
    public TKey Id { get; set; }
}