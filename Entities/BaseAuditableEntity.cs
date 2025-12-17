namespace N10.Entities;

public abstract class BaseAuditableEntity : BaseEntity
{
    private protected BaseAuditableEntity()
    {
        IsActive = true;
        IsDeleted = false;
        CreatedUtc = DateTime.UtcNow; // Ne treba u konstruktoru
    }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public int? CreatedId { get; set; }

    public DateTime CreatedUtc { get; set; }

    public int? LastModifiedId { get; set; }

    public DateTime LastModifiedUtc { get; set; }
}