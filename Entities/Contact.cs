namespace N10.Entities;

public class Contact : BaseAuditableEntity
{
    public int? ApplicationUserId { get; set; }
    public virtual ApplicationUser? ApplicationUser { get; set; }



    public string? ImageUrl { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string? LastName { get; set; }

    public string? CompanyName { get; set; }

    public DateOnly? DateOfBirth { get; set; }



    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }


    public int? AddressId { get; set; }
    public virtual Address? Address { get; set; }


    public virtual ICollection<ContactCategory> Categories { get; set; } = [];
}
