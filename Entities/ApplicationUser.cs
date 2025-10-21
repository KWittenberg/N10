namespace N10.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public string? AvatarUrl { get; set; }


    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? CompanyName { get; set; }

    public DateOnly? DateOfBirth { get; set; }


    public string? Country { get; set; }

    public string? Zip { get; set; }

    public string? City { get; set; }

    public string? Street { get; set; }


    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public string? PlaceId { get; set; }




    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public Guid? CreatedId { get; set; }

    public DateTime CreatedUtc { get; set; }

    public Guid? LastModifiedId { get; set; }

    public DateTime LastModifiedUtc { get; set; }
}