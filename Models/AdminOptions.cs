namespace N10.Models;

public class AdminOptions
{
    public string Avatar { get; set; } = string.Empty;


    public string UserName { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string CompanyName { get; set; } = string.Empty;

    public string DateOfBirth { get; set; } = string.Empty;


    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;




    public string Country { get; set; } = string.Empty;

    public string Zip { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string Street { get; set; } = string.Empty;




    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public string? PlaceId { get; set; }
}