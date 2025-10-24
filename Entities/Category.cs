namespace N10.Entities;

public class Category : BaseEntity<Guid>
{
    public string Name { get; set; } = string.Empty;

    //public string? Description { get; set; }



    public Guid? ApplicationUserId { get; set; }

    public virtual ApplicationUser? ApplicationUser { get; set; }



    public virtual ICollection<Contact> Contacts { get; set; } = [];
}
