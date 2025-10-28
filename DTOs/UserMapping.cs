namespace N10.DTOs;

public static class UserMapping
{
    public static Expression<Func<ApplicationUser, UserDto>> ToDtoExpression => u => new UserDto
    {
        Id = u.Id,
        AvatarUrl = u.AvatarUrl,
        FirstName = u.FirstName,
        LastName = u.LastName,
        CompanyName = u.CompanyName,
        DateOfBirth = u.DateOfBirth,
        Email = u.Email,
        EmailConfirmed = u.EmailConfirmed,
        PhoneNumber = u.PhoneNumber,
        PhoneNumberConfirmed = u.PhoneNumberConfirmed,
        Country = u.Country,
        Zip = u.Zip,
        City = u.City,
        Street = u.Street,
        Latitude = u.Latitude,
        Longitude = u.Longitude,
        PlaceId = u.PlaceId,
        IsActive = u.IsActive,
        IsDeleted = u.IsDeleted,
        CreatedId = u.CreatedId,
        CreatedUtc = u.CreatedUtc,
        LastModifiedId = u.LastModifiedId,
        LastModifiedUtc = u.LastModifiedUtc
    };

    private static readonly Func<ApplicationUser, UserDto> _projector = ToDtoExpression.Compile();
    public static List<UserDto> ToDtoList(this IEnumerable<ApplicationUser> entities) => entities.Select(_projector).ToList();

    public static async Task<UserDto?> ToDtoAsync(this ApplicationUser? entity, ApplicationDbContext context)
    {
        if (entity is null) return null;
        if (context is null) throw new ArgumentNullException(nameof(context));

        var roles = await context.UserRoles.Where(ur => ur.UserId == entity.Id)
            .Join(context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new RoleDto
            {
                Id = r.Id,
                Name = r.Name ?? string.Empty
            }).ToListAsync();

        var dto = _projector(entity); // map scalar properties
        dto.Roles = roles;
        return dto;
    }

    public static UserDto ToDto(this ApplicationUser entity) => _projector(entity);

    public static ApplicationUser? ToEntity(this UserInput? input)
    {
        if (input is null) return null;

        return new ApplicationUser
        {
            FirstName = input.FirstName,
            LastName = input.LastName,
            CompanyName = input.CompanyName,
            DateOfBirth = input.DateOfBirth,
            Email = input.Email,
            UserName = input.Email, // Identity convention: UserName = Email
            PhoneNumber = input.PhoneNumber,
            Country = input.Country,
            Zip = input.Zip,
            City = input.City,
            Street = input.Street
            // Latitude/Longitude/PlaceId can be set here if present on input
        };
    }

    public static void UpdateFromInput(this ApplicationUser? entity, UserInput? input)
    {
        if (entity is null || input is null) return;

        entity.FirstName = input.FirstName;
        entity.LastName = input.LastName;
        entity.CompanyName = input.CompanyName;
        entity.DateOfBirth = input.DateOfBirth;
        entity.PhoneNumber = input.PhoneNumber;
        entity.Country = input.Country;
        entity.Zip = input.Zip;
        entity.City = input.City;
        entity.Street = input.Street;
        // Latitude/Longitude/PlaceId if needed
    }

    public static UserInput? ToInput(this UserDto? dto)
    {
        if (dto is null) return null;

        return new UserInput
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            CompanyName = dto.CompanyName,
            DateOfBirth = dto.DateOfBirth,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            Country = dto.Country,
            Zip = dto.Zip,
            City = dto.City,
            Street = dto.Street
            // Password omitted - handled separately
        };
    }
}