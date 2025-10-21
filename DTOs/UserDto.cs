namespace N10.DTOs;

public class UserDto
{
    public Guid Id { get; init; } = Guid.Empty;

    // public string? UserName { get; set; } = string.Empty;

    public string? AvatarUrl { get; set; } = string.Empty;



    public string? FirstName { get; set; } = string.Empty;

    public string? LastName { get; set; } = string.Empty;

    public string? CompanyName { get; set; } = string.Empty;

    public DateOnly? DateOfBirth { get; set; }



    public string? Email { get; set; } = string.Empty;

    public bool EmailConfirmed { get; set; }

    public string? PhoneNumber { get; set; } = string.Empty;

    public bool PhoneNumberConfirmed { get; set; }



    public string? Country { get; set; } = string.Empty;

    public string? Zip { get; set; } = string.Empty;

    public string? City { get; set; } = string.Empty;

    public string? Street { get; set; } = string.Empty;



    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public string? PlaceId { get; set; }




    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public Guid? CreatedId { get; set; }

    public DateTime CreatedUtc { get; set; }

    public Guid? LastModifiedId { get; set; }

    public DateTime LastModifiedUtc { get; set; }




    public ICollection<RoleDto>? Roles { get; set; }
}