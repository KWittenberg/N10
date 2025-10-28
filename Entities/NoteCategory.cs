namespace N10.Entities;

public class NoteCategory : BaseEntity<Guid>
{
    public Guid? ApplicationUserId { get; set; }
    public virtual ApplicationUser? ApplicationUser { get; set; }


    public string Name { get; set; } = string.Empty;

    public string? Color { get; set; } = "#6f42c1";


    public virtual ICollection<Note> Notes { get; set; } = [];
}