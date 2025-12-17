namespace N10.Entities;

public class TaskerItem : BaseAuditableEntity
{
    public int? UserId { get; set; }


    [Required(ErrorMessage = "Every task must have a name")]
    public string Name { get; set; } = string.Empty;

    public bool IsComplete { get; set; } = false;



    public virtual ApplicationUser? User { get; set; }
}