namespace N10.DTOs;

public static class UserMappingOLD
{
    //// GetAllToListAsync() - Convert IEnumerable<ApplicationUser> to List<UserDto> sa roles
    //public static async Task<List<UserDto>> ToDtoListAsync(this IEnumerable<ApplicationUser>? entities, ApplicationDbContext context)
    //{
    //    if (entities is null) return new List<UserDto>();

    //    var dtos = new List<UserDto>();
    //    foreach (var entity in entities)
    //    {
    //        var dto = await entity.ToDtoAsync(context);
    //        if (dto is not null)
    //        {
    //            dtos.Add(dto);
    //        }
    //    }
    //    return dtos;
    //}

    //// GetByIdAsync() ili GetCurrentUserAsync() - Convert ApplicationUser to UserDto sa roles (async zbog fetch roles)
    //public static async Task<UserDto?> ToDtoAsync(this ApplicationUser? entity, ApplicationDbContext context)
    //{
    //    if (entity is null) return null;

    //    // Fetch roles koristeći join (kao u tvom originalu)
    //    var roles = await context.UserRoles
    //        .Where(ur => ur.UserId == entity.Id)
    //        .Join(context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new RoleDto
    //        {
    //            Id = r.Id,
    //            Name = r.Name ?? string.Empty
    //        })
    //        .ToListAsync();

    //    return new UserDto
    //    {
    //        Id = entity.Id,
    //        AvatarUrl = entity.AvatarUrl ?? string.Empty,
    //        FirstName = entity.FirstName ?? string.Empty,
    //        LastName = entity.LastName ?? string.Empty,
    //        CompanyName = entity.CompanyName ?? string.Empty,
    //        DateOfBirth = entity.DateOfBirth,
    //        Email = entity.Email ?? string.Empty,
    //        EmailConfirmed = entity.EmailConfirmed,
    //        PhoneNumber = entity.PhoneNumber ?? string.Empty,
    //        PhoneNumberConfirmed = entity.PhoneNumberConfirmed,
    //        Country = entity.Country ?? string.Empty,
    //        Zip = entity.Zip ?? string.Empty,
    //        City = entity.City ?? string.Empty,
    //        Street = entity.Street ?? string.Empty,
    //        Roles = roles
    //    };
    //}

    //// GetByIdAsync()
    //public static UserDto ToDto(this ApplicationUser entity)
    //{
    //    return new UserDto
    //    {
    //        Id = entity.Id,
    //        AvatarUrl = entity.AvatarUrl ?? string.Empty,
    //        FirstName = entity.FirstName ?? string.Empty,
    //        LastName = entity.LastName ?? string.Empty,
    //        CompanyName = entity.CompanyName ?? string.Empty,
    //        DateOfBirth = entity.DateOfBirth,
    //        Email = entity.Email ?? string.Empty,
    //        EmailConfirmed = entity.EmailConfirmed,
    //        PhoneNumber = entity.PhoneNumber ?? string.Empty,
    //        PhoneNumberConfirmed = entity.PhoneNumberConfirmed,
    //        Country = entity.Country ?? string.Empty,
    //        Zip = entity.Zip ?? string.Empty,
    //        City = entity.City ?? string.Empty,
    //        Street = entity.Street ?? string.Empty,
    //        Latitude = entity.Latitude,
    //        Longitude = entity.Longitude,
    //        PlaceId = entity.PlaceId,
    //        IsActive = entity.IsActive,
    //        IsDeleted = entity.IsDeleted,
    //        CreatedId = entity.CreatedId,
    //        CreatedUtc = entity.CreatedUtc,
    //        LastModifiedId = entity.LastModifiedId,
    //        LastModifiedUtc = entity.LastModifiedUtc
    //    };
    //}

    //// AddAsync() - Convert UserInput to ApplicationUser
    //public static ApplicationUser? ToEntity(this UserInput? input)
    //{
    //    if (input is null) return null;

    //    return new ApplicationUser
    //    {
    //        //AvatarUrl = input.AvatarUrl,
    //        FirstName = input.FirstName,
    //        LastName = input.LastName,
    //        CompanyName = input.CompanyName,
    //        DateOfBirth = input.DateOfBirth,
    //        Email = input.Email,
    //        UserName = input.Email,  // Identity konvencija: UserName = Email
    //        PhoneNumber = input.PhoneNumber,
    //        Country = input.Country,
    //        Zip = input.Zip,
    //        City = input.City,
    //        Street = input.Street
    //        // Latitude/Longitude/PlaceId ako ih imaš u input-u
    //    };
    //}

    //// UpdateAsync() - Update ApplicationUser from UserInput
    //public static void UpdateFromInput(this ApplicationUser? entity, UserInput? input)
    //{
    //    if (entity is null || input is null) return;

    //    //entity.AvatarUrl = input.AvatarUrl;
    //    entity.FirstName = input.FirstName;
    //    entity.LastName = input.LastName;
    //    entity.CompanyName = input.CompanyName;
    //    entity.DateOfBirth = input.DateOfBirth;
    //    entity.PhoneNumber = input.PhoneNumber;
    //    entity.Country = input.Country;
    //    entity.Zip = input.Zip;
    //    entity.City = input.City;
    //    entity.Street = input.Street;
    //    // Ako mijenjaš Email/UserName, koristi _userManager.SetEmailAsync/SetUserNameAsync za Identity validaciju
    //}

    //// UI -> Convert UserDto to UserInput
    //public static UserInput? ToInput(this UserDto? dto)
    //{
    //    if (dto is null) return null;

    //    return new UserInput
    //    {
    //        //AvatarUrl = dto.AvatarUrl,
    //        FirstName = dto.FirstName,
    //        LastName = dto.LastName,
    //        CompanyName = dto.CompanyName,
    //        DateOfBirth = dto.DateOfBirth,
    //        Email = dto.Email,
    //        PhoneNumber = dto.PhoneNumber,
    //        Country = dto.Country,
    //        Zip = dto.Zip,
    //        City = dto.City,
    //        Street = dto.Street
    //        // Password ako treba za reset, ali obično ne u edit formi
    //    };
    //}









    // GetAllQueryableAsync()
    //public static Expression<Func<ApplicationUser, UserDto>> ToDtoExpression => x => new UserDto
    //{
    //    Id = x.Id,
    //    AvatarUrl = x.AvatarUrl,
    //    FirstName = x.FirstName,
    //    LastName = x.LastName,
    //    CompanyName = x.CompanyName,
    //    DateOfBirth = x.DateOfBirth,
    //    Email = x.Email,
    //    EmailConfirmed = x.EmailConfirmed,
    //    PhoneNumber = x.PhoneNumber,
    //    PhoneNumberConfirmed = x.PhoneNumberConfirmed,
    //    Country = x.Country,
    //    Zip = x.Zip,
    //    City = x.City,
    //    Street = x.Street
    //};

    // GetAllListAsync()
    //public static List<UserDto> ToDtoList(this IEnumerable<ApplicationUser> entities)
    //{
    //    return entities.Select(e => e.ToDto()).ToList();
    //}

    // GetByIdAsync() - Convert ApplicationUser to UserDto
    //public static async Task<UserDto> ToDtoAsync(this ApplicationUser entity, IUserRepository userRepository)
    //{
    //    var roles = await userRepository.GetRolesByUserIdAsync(entity.Id);

    //    //var roles = await context.UserRoles
    //    //    .Where(ur => ur.UserId == entity.Id)
    //    //    .Join(context.Roles,
    //    //        ur => ur.RoleId,
    //    //        r => r.Id,
    //    //        (ur, r) => new RoleDto(r.Id, r.Name ?? string.Empty))
    //    //    .ToListAsync();

    //    return new UserDto
    //    {
    //        Id = entity.Id,
    //        AvatarUrl = entity.AvatarUrl ?? string.Empty,
    //        FirstName = entity.FirstName ?? string.Empty,
    //        LastName = entity.LastName ?? string.Empty,
    //        CompanyName = entity.CompanyName ?? string.Empty,
    //        DateOfBirth = entity.DateOfBirth,
    //        Email = entity.Email ?? string.Empty,
    //        EmailConfirmed = entity.EmailConfirmed,
    //        PhoneNumber = entity.PhoneNumber ?? string.Empty,
    //        PhoneNumberConfirmed = entity.PhoneNumberConfirmed,
    //        Country = entity.Country ?? string.Empty,
    //        Zip = entity.Zip ?? string.Empty,
    //        City = entity.City ?? string.Empty,
    //        Street = entity.Street ?? string.Empty,
    //        Roles = roles.Data
    //    };
    //}









    // GetAllAsync() - Convert IEnumerable<ApplicationUser> to List<UserDto>
    //public static async Task<List<UserDto>> ToDtoListAsync(this IEnumerable<ApplicationUser>? entities, ApplicationDbContext context)
    //{
    //    if (entities is null) return new List<UserDto>();

    //    var dtos = new List<UserDto>();
    //    foreach (var entity in entities)
    //    {
    //        var dto = await entity.ToDtoAsync(context);
    //        if (dto is not null)
    //        {
    //            dtos.Add(dto);
    //        }
    //    }
    //    return dtos;
    //}

    // GetByIdAsync() - Convert ApplicationUser to UserDto
    //public static async Task<UserDto> ToDtoAsync(this ApplicationUser? entity, ApplicationDbContext context)
    //{
    //    if (entity is null) return null;

    //    // Fetch roles for the user
    //    var roles = await context.UserRoles
    //        .Where(ur => ur.UserId == entity.Id)
    //        .Join(context.Roles,
    //            ur => ur.RoleId,
    //            r => r.Id,
    //            (ur, r) => new RoleDto(r.Id, r.Name ?? string.Empty))
    //        .ToListAsync();

    //    return new UserDto
    //    {
    //        Id = entity.Id,
    //        AvatarUrl = entity.AvatarUrl ?? string.Empty,
    //        FirstName = entity.FirstName ?? string.Empty,
    //        LastName = entity.LastName ?? string.Empty,
    //        CompanyName = entity.CompanyName ?? string.Empty,
    //        DateOfBirth = entity.DateOfBirth,
    //        Email = entity.Email ?? string.Empty,
    //        EmailConfirmed = entity.EmailConfirmed,
    //        PhoneNumber = entity.PhoneNumber ?? string.Empty,
    //        PhoneNumberConfirmed = entity.PhoneNumberConfirmed,
    //        Country = entity.Country ?? string.Empty,
    //        Zip = entity.Zip ?? string.Empty,
    //        City = entity.City ?? string.Empty,
    //        Street = entity.Street ?? string.Empty,
    //        Roles = roles
    //    };
    //}

    // AddAsync() - Convert UserInput to ApplicationUser
    //public static ApplicationUser ToEntity(this UserInput? input)
    //{
    //    if (input is null) return null;

    //    return new ApplicationUser
    //    {
    //        UserName = input.Email, // Identity typically uses Email as UserName
    //        Email = input.Email,
    //        PhoneNumber = input.PhoneNumber,
    //        AvatarUrl = input.AvatarUrl,
    //        FirstName = input.FirstName,
    //        LastName = input.LastName,
    //        CompanyName = input.CompanyName,
    //        DateOfBirth = input.DateOfBirth,
    //        Country = input.Country,
    //        Zip = input.Zip,
    //        City = input.City,
    //        Street = input.Street
    //    };
    //}

    //// UpdateAsync() - Update ApplicationUser from UserInput
    //public static void UpdateFromInput(this ApplicationUser? entity, UserInput? input)
    //{
    //    if (entity is null || input is null) return;

    //    entity.UserName = input.Email;
    //    entity.Email = input.Email;
    //    entity.PhoneNumber = input.PhoneNumber;
    //    entity.AvatarUrl = input.AvatarUrl;
    //    entity.FirstName = input.FirstName;
    //    entity.LastName = input.LastName;
    //    entity.CompanyName = input.CompanyName;
    //    entity.DateOfBirth = input.DateOfBirth;
    //    entity.Country = input.Country;
    //    entity.Zip = input.Zip;
    //    entity.City = input.City;
    //    entity.Street = input.Street;
    //}

    //// UI -> Convert UserDto to UserInput
    //public static UserInput ToInput(this UserDto? dto)
    //{
    //    if (dto is null) return null;

    //    return new UserInput(
    //        FirstName: dto.FirstName,
    //        LastName: dto.LastName,
    //        CompanyName: dto.CompanyName,
    //        DateOfBirth: dto.DateOfBirth,
    //        Email: dto.Email,
    //        PhoneNumber: dto.PhoneNumber,
    //        Country: dto.Country,
    //        Zip: dto.Zip,
    //        City: dto.City,
    //        Street: dto.Street,
    //        AvatarUrl: dto.AvatarUrl);
    //}
}



//public static class UserMapping
//{
//    // GetAllAsync()
//    public static List<UserDto> ToDtoList(this IEnumerable<ApplicationUser>? entities)
//    {
//        if (entities is null) return new List<UserDto>();

//        return entities.Select(e => e.ToDto()).Where(dto => dto is not null).ToList();
//    }

//    // GetByIdAsync()
//    public static UserDto ToDto(this User? entity)
//    {
//        if (entity is null) return null;

//        return new UserDto
//        {
//            Id = entity.Id,
//            Name = entity.Name,
//            FirstName = entity.FirstName,
//            LastName = entity.LastName,
//            Email = entity.Email,
//            ImageUrl = entity.ImageUrl,
//            Role = entity.Role,
//            // Rsvps = entity.Rsvps?.ToDtoList() ?? new List<RsvpDto>()
//        };
//    }

//    // AddAsync()
//    public static User ToEntity(this UserInput? input)
//    {
//        if (input == null) return null;

//        return new User
//        {
//            Name = input.Name,
//            FirstName = input.FirstName,
//            LastName = input.LastName,
//            Email = input.Email,
//            ImageUrl = input.ImageUrl,
//            Role = input.Role
//        };
//    }

//    // UpdateAsync()
//    public static void UpdateFromInput(this User? entity, UserInput? input)
//    {
//        if (entity == null || input == null) return;

//        entity.Name = input.Name;
//        entity.FirstName = input.FirstName;
//        entity.LastName = input.LastName;
//        entity.Email = input.Email;
//        entity.ImageUrl = input.ImageUrl;
//        entity.Role = input.Role;
//    }

//    // UI ->
//    public static UserInput ToInput(this UserDto? dto)
//    {
//        if (dto is null) return null;

//        return new UserInput
//        {
//            Name = dto.Name,
//            FirstName = dto.FirstName,
//            LastName = dto.LastName,
//            Email = dto.Email,
//            ImageUrl = dto.ImageUrl,
//            Role = dto.Role
//        };
//    }
//}