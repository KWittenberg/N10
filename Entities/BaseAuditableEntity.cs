namespace N10.Entities;

public abstract class BaseAuditableEntity<TKey> : BaseEntity<TKey>
{
    private protected BaseAuditableEntity()
    {
        IsActive = true;
        IsDeleted = false;
        CreatedUtc = DateTime.UtcNow; // Ne treba u konstruktoru
    }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public TKey? CreatedId { get; set; }

    public DateTime CreatedUtc { get; set; }

    public TKey? LastModifiedId { get; set; }

    public DateTime LastModifiedUtc { get; set; }
}