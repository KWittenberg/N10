namespace N10.DTOs;

public class UserDto
{
    public int? Id { get; init; }

    //public Guid Id { get; init; } = Guid.Empty;

    // public string? UserName { get; set; } = string.Empty;

    public string? AvatarUrl { get; set; } = string.Empty;



    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string? CompanyName { get; set; }

    public DateOnly? DateOfBirth { get; set; }



    public string? Password { get; set; }


    public string Email { get; set; } = string.Empty;

    public bool EmailConfirmed { get; set; }

    public string? PhoneNumber { get; set; }

    public bool PhoneNumberConfirmed { get; set; }



    public string? Country { get; set; }

    public string? Zip { get; set; }

    public string? City { get; set; }

    public string? Street { get; set; }



    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public string? PlaceId { get; set; }




    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public int? CreatedId { get; set; }

    public DateTime CreatedUtc { get; set; }

    public int? LastModifiedId { get; set; }

    public DateTime LastModifiedUtc { get; set; }




    public ICollection<RoleDto>? Roles { get; set; }
}