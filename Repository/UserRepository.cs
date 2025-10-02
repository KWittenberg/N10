namespace N10.Repository;

public class UserRepository(IDbContextFactory<ApplicationDbContext> dbFactory,
                            ApplicationDbContext context,
                            IHttpContextAccessor contextAccessor,
                            UserManager<ApplicationUser> userManager,
                            IWebHostEnvironment webHostEnvironment
                            //UserInputValidator validator
                            ) : IUserRepository
{


    static readonly string AvatarFolder = Path.Combine("img", "avatar");


    //public Guid GetCurrentUserId() => Guid.Parse(contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());
    //public Guid UserId => contextAccessor.HttpContext?.User.GetUserId() ?? throw new ApplicationException("User context is unavailable");
    //public Guid GetCurrentUserId() => Guid.TryParse(contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid userId) ? userId : Guid.Empty;

    public Guid GetCurrentUserId()
    {
        var userIdClaim = contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        return userIdClaim != null ? Guid.Parse(userIdClaim) : Guid.Empty;
    }

    //public async Task<UserDto> GetCurrentUser()
    //{
    //    // Guid userId = contextAccessor.HttpContext?.User.GetUserId() ?? Guid.Empty; //pozivanje iz extension metode

    //    Guid userId = GetCurrentUserId(); // lokalna metoda

    //    // with UserManager
    //    //var user = await userManager.FindByIdAsync(userId);
    //    //if (user is null) return new UserDto();

    //    await using var db = await dbFactory.CreateDbContextAsync();

    //    var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId);
    //    if (user is null) return new UserDto();

    //    var userRoleIds = db.UserRoles.Where(ur => ur.UserId == user.Id).Select(ur => ur.RoleId).ToList();
    //    var userRoles = db.Roles.Where(r => userRoleIds.Contains(r.Id)).ToList();

    //    var output = user.Adapt<UserDto>();
    //    output.Roles = userRoles.Select(r => r.Adapt<RoleDto>()).ToList();

    //    return output;
    //}

    //public async Task<ServiceResponse<UserDto>> GetCurrentUserAsync()
    //{
    //    await using var db = await dbFactory.CreateDbContextAsync();

    //    Guid userId = GetCurrentUserId();

    //    //var user = await userManager.FindByIdAsync(userId);
    //    //var user = await db.Users.FindAsync(userId); //Error - Slow loading

    //    var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId);
    //    if (user is null) return new ServiceResponse<UserDto>(HttpStatusCode.NotFound, "CurrentUser - Not found!");

    //    var userRoleIds = db.UserRoles.Where(ur => ur.UserId == user.Id).Select(ur => ur.RoleId).ToList();
    //    var userRoles = db.Roles.Where(r => userRoleIds.Contains(r.Id)).ToList();

    //    var output = user.Adapt<UserDto>();
    //    output.Roles = userRoles.Select(r => r.Adapt<RoleDto>()).ToList();

    //    return new ServiceResponse<UserDto>(HttpStatusCode.OK, "OK", output);
    //}






    //public async Task<Result<List<UserDto>>> GetAllToListAsync()
    //{
    //    await using var db = await dbFactory.CreateDbContextAsync();

    //    var entities = await db.Users.OrderBy(x => x.LastName).ToListAsync();
    //    if (!entities.Any()) return Result<List<UserDto>>.Error("Users not found!");
    //    var outputs = entities.Adapt<List<UserDto>>();

    //    return Result<List<UserDto>>.Ok(outputs);
    //}

    public async Task<Result<IQueryable<UserDto>>> GetAllAsync()
    {
        try
        {
            var query = context.Users.AsNoTracking().Select(x => new UserDto
            {
                Id = x.Id,
                AvatarUrl = x.AvatarUrl,
                FirstName = x.FirstName,
                LastName = x.LastName,
                CompanyName = x.CompanyName,
                DateOfBirth = x.DateOfBirth,
                Email = x.Email,
                EmailConfirmed = x.EmailConfirmed,
                PhoneNumber = x.PhoneNumber,
                PhoneNumberConfirmed = x.PhoneNumberConfirmed,
                Country = x.Country,
                Zip = x.Zip,
                City = x.City,
                Street = x.Street,
                //Roles = context.UserRoles.Where(ur => ur.UserId == x.Id)
                //                                .Join(context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new RoleDto(r.Id, r.Name!))
                //                                .ToList()
                Roles = context.UserRoles.Where(ur => ur.UserId == x.Id)
                                                .Join(context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new RoleDto
                                                {
                                                    Id = r.Id,
                                                    Name = r.Name ?? string.Empty
                                                }).ToList()
            }).AsQueryable();

            if (!await query.AnyAsync()) return Result<IQueryable<UserDto>>.Error("No users found!");

            return Result<IQueryable<UserDto>>.Ok(query);
        }
        catch (Exception ex)
        {
            return Result<IQueryable<UserDto>>.Error($"Database error: {ex.Message}");
        }
    }






    //public async Task<Result<UserDto>> GetByIdNoMapsterAsync(Guid id)
    //{
    //    try
    //    {
    //        await using var db = await dbFactory.CreateDbContextAsync();

    //        var user = await userManager.FindByIdAsync(id.ToString());
    //        if (user == null) return Result<UserDto>.Error("User not found!");

    //        var roles = await db.UserRoles
    //            .Where(ur => ur.UserId == user.Id)
    //            .Join(db.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new RoleDto(r.Id, r.Name))
    //            .ToListAsync();

    //        var userDto = new UserDto
    //        {
    //            Id = user.Id,
    //            AvatarUrl = user.AvatarUrl,
    //            FirstName = user.FirstName,
    //            LastName = user.LastName,
    //            CompanyName = user.CompanyName,
    //            Email = user.Email,
    //            PhoneNumber = user.PhoneNumber,
    //            DateOfBirth = user.DateOfBirth,
    //            Country = user.Country,
    //            Zip = user.Zip,
    //            City = user.City,
    //            Street = user.Street,
    //            EmailConfirmed = user.EmailConfirmed,
    //            Roles = roles
    //        };

    //        return Result<UserDto>.Ok(userDto);
    //    }
    //    catch (Exception ex)
    //    {
    //        return Result<UserDto>.Error($"Database error: {ex.Message}");
    //    }
    //}

    //public async Task<Result<UserDto>> GetByIdAsync(Guid id)
    //{
    //    try
    //    {
    //        await using var db = await dbFactory.CreateDbContextAsync();

    //        var user = await userManager.FindByIdAsync(id.ToString());
    //        if (user == null) return Result<UserDto>.Error("User not found!");

    //        var roles = await db.UserRoles.Where(ur => ur.UserId == user.Id)
    //                                        .Join(db.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Adapt<RoleDto>())
    //                                        .ToListAsync();

    //        var userDto = user.Adapt<UserDto>();
    //        userDto.Roles = roles;

    //        return Result<UserDto>.Ok(userDto);
    //    }
    //    catch (Exception ex)
    //    {
    //        return Result<UserDto>.Error($"Database error: {ex.Message}");
    //    }
    //}



    public async Task<Result> AddAsync(UserInput input)
    {
        //var validateInput = await validator.ValidateAsync(input);
        //if (!validateInput.IsValid) return Result.Error(validateInput.ToString());

        var user = new ApplicationUser();
        user.FirstName = input.FirstName;
        user.LastName = input.LastName;
        user.CompanyName = input.CompanyName;
        user.Email = input.Email;
        user.DateOfBirth = input.DateOfBirth;
        user.PhoneNumber = input.PhoneNumber;
        user.Country = input.Country;
        user.Zip = input.Zip;
        user.City = input.City;
        user.Street = input.Address;
        user.UserName = input.Email;

        var result = await userManager.CreateAsync(user, input.Password);
        if (!result.Succeeded) throw new Exception($"Error in creating User {Environment.NewLine}{string.Join(Environment.NewLine, result.Errors.Select(e => e.Description))}");

        // await userManager.AddToRoleAsync(user, ApplicationRole.User);

        return Result.Ok("User - Added!");
    }

    public async Task<Result> UpdateAsync(Guid id, UserInput input)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var entity = await db.Users.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null) return Result.Error("User - Not found!");

        //var validateInput = await _validator.ValidateAsync(input);
        //if (!validateInput.IsValid) return new ServiceResponse(HttpStatusCode.BadRequest, validateInput.ToString());

        entity.FirstName = input.FirstName;
        entity.LastName = input.LastName;
        entity.CompanyName = input.CompanyName;
        entity.DateOfBirth = input.DateOfBirth;
        entity.PhoneNumber = input.PhoneNumber;
        entity.Country = input.Country;
        entity.Zip = input.Zip;
        entity.City = input.City;
        entity.Street = input.Address;

        db.Users.Update(entity);
        await db.SaveChangesAsync();

        return Result.Ok("User - Updated!");
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var user = await db.Users.FirstOrDefaultAsync(x => x.Id == id);
        if (user is null) return Result.Error("User - Not found!");
        if (!string.IsNullOrEmpty(user.AvatarUrl))
        {
            var filePath = Path.Combine(webHostEnvironment.WebRootPath, user.AvatarUrl.Replace("/", "\\"));
            if (File.Exists(filePath)) File.Delete(filePath);
        }

        db.Users.Remove(user);
        await db.SaveChangesAsync();

        return Result.Ok("User - Deleted!");
    }



    #region AVATAR
    public async Task<Result> UpdateAvatarAsync(Guid id, AvatarInput input, bool isOriginal)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var entity = await db.Users.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null) return Result.Error("User not found!");

        if (!string.IsNullOrEmpty(entity.AvatarUrl))
        {
            var imagePath = Path.Combine(webHostEnvironment.WebRootPath, entity.AvatarUrl.Replace("/", "\\"));
            if (File.Exists(imagePath)) File.Delete(imagePath);
        }

        var extension = Path.GetExtension(input.File!.Name);
        var fileName = $"{id}{extension}";
        var saveFolder = Path.Combine(webHostEnvironment.WebRootPath, AvatarFolder);
        var filePath = Path.Combine(saveFolder, fileName);
        if (!Directory.Exists(saveFolder)) Directory.CreateDirectory(saveFolder);

        if (isOriginal)
        {
            await using FileStream fs = new(filePath, FileMode.Create);
            await input.File.OpenReadStream().CopyToAsync(fs);
        }
        else
        {
            //await imageService.OptimizeAndSaveImage(input.File.OpenReadStream(), filePath, 500, 500);
        }

        var relativePath = Path.Combine(AvatarFolder, fileName).Replace("\\", "/");
        entity.AvatarUrl = relativePath;

        db.Users.Update(entity);
        await db.SaveChangesAsync();

        return Result.Ok("Avatar Updated!");
    }

    //public async Task<ServiceResponse> UpdateAvatarAsync(Guid id, AvatarInput input)
    //{
    //    await using var db = await dbFactory.CreateDbContextAsync();

    //    var entity = await db.Users.FirstOrDefaultAsync(x => x.Id == id);
    //    if (entity is null) return new ServiceResponse(HttpStatusCode.NotFound, "User - Not found!");

    //    //var validateInput = await _validatorImage.ValidateAsync(input);
    //    //if (!validateInput.IsValid) return new ServiceResponse(HttpStatusCode.BadRequest, validateInput.ToString());

    //    //if (input.File is null) return new ServiceResponse(HttpStatusCode.NotFound, "Author Image Not Found!");

    //    if (!string.IsNullOrEmpty(entity.AvatarUrl))
    //    {
    //        var imagePath = Path.Combine(webHostEnvironment.WebRootPath, entity.AvatarUrl.Replace("/", "\\"));
    //        if (File.Exists(imagePath)) File.Delete(imagePath);
    //    }

    //    var extension = Path.GetExtension(input.File!.Name);
    //    var fileName = $"{id}{extension}";
    //    var saveFolder = Path.Combine(webHostEnvironment.WebRootPath, AvatarFolder);
    //    var filePath = Path.Combine(saveFolder, fileName);
    //    if (!Directory.Exists(saveFolder)) Directory.CreateDirectory(saveFolder);

    //    await using FileStream fs = new(filePath, FileMode.Create);
    //    await input.File.OpenReadStream().CopyToAsync(fs);

    //    var relativePath = Path.Combine(AvatarFolder, fileName).Replace("\\", "/");
    //    entity.AvatarUrl = relativePath;

    //    db.Users.Update(entity);
    //    await db.SaveChangesAsync();

    //    return new ServiceResponse(HttpStatusCode.OK, "User - Avatar Updated!");
    //}

    public async Task<Result> DeleteAvatarAsync(Guid id)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var entity = await db.Users.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null) return Result.Error("User - Not found!");
        if (string.IsNullOrEmpty(entity.AvatarUrl)) return Result.Error("User Avatar - Not found!");

        var filePath = Path.Combine(webHostEnvironment.WebRootPath, entity.AvatarUrl.Replace("/", "\\"));
        if (File.Exists(filePath)) File.Delete(filePath);

        entity.AvatarUrl = string.Empty;

        db.Users.Update(entity);
        await db.SaveChangesAsync();

        return Result.Ok("User Avatar - Deleted!");
    }
    #endregion

    #region ROLE
    public async Task<Result<List<RoleDto>>> GetRolesByUserIdAsync(Guid userId)
    {
        try
        {
            await using var db = await dbFactory.CreateDbContextAsync();

            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user is null) return Result<List<RoleDto>>.Error("User not found!");

            var userRoles = await userManager.GetRolesAsync(user);
            if (!userRoles.Any()) return Result<List<RoleDto>>.Ok(new List<RoleDto>());

            //var roles = await db.Roles.Where(r => userRoles.Contains(r.Name!)).Select(r => new RoleDto(r.Id, r.Name!)).ToListAsync();
            var roles = await db.Roles.Where(r => userRoles.Contains(r.Name!)).Select(r => r.ToDto()).ToListAsync();

            return Result<List<RoleDto>>.Ok(roles);
        }
        catch (Exception ex)
        {
            return Result<List<RoleDto>>.Error($"Error retrieving roles: {ex.Message}");
        }
    }

    public async Task<Result<List<RoleDto>>> GetRolesNotAssignedToUserAsync(Guid userId)
    {
        try
        {
            await using var db = await dbFactory.CreateDbContextAsync();
            var userRoles = db.UserRoles.Where(ur => ur.UserId == userId).Select(ur => ur.RoleId);
            //var roles = await db.Roles.Where(r => !userRoles.Contains(r.Id)).Select(r => new RoleDto(r.Id, r.Name!)).ToListAsync();
            var roles = await db.Roles.Where(r => !userRoles.Contains(r.Id)).Select(r => r.ToDto()).ToListAsync();

            return Result<List<RoleDto>>.Ok(roles);
        }
        catch (Exception ex)
        {
            return Result<List<RoleDto>>.Error($"Error retrieving roles: {ex.Message}");
        }
    }

    public async Task<Result<RoleDto>> GetRoleByIdAsync(Guid userId, Guid roleId)
    {
        try
        {
            await using var db = await dbFactory.CreateDbContextAsync();

            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user is null) return Result<RoleDto>.Error("User not found!");

            //var role = await db.Roles.Where(r => r.Id == roleId).Select(r => new RoleDto(r.Id, r.Name!)).FirstOrDefaultAsync();
            var role = await db.Roles.Where(r => r.Id == roleId).Select(r => r.ToDto()).FirstOrDefaultAsync();
            if (role is null) return Result<RoleDto>.Error("Role not found!");

            var userRoles = await userManager.GetRolesAsync(user);
            if (!userRoles.Contains(role.Name)) return Result<RoleDto>.Error("Role is not assigned to this user!");

            return Result<RoleDto>.Ok(role);
        }
        catch (Exception ex)
        {
            return Result<RoleDto>.Error($"Error retrieving role: {ex.Message}");
        }
    }

    public async Task<Result> AddRoleAsync(Guid userId, Guid roleId)
    {
        try
        {
            await using var db = await dbFactory.CreateDbContextAsync();

            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user is null) return Result.Error("User not found!");

            var role = await db.Roles.FindAsync(roleId);
            if (role is null) return Result.Error("Role not found!");

            var userRoles = await userManager.GetRolesAsync(user);
            if (userRoles.Contains(role.Name!)) return Result.Error("Role already assigned to user!");

            var result = await userManager.AddToRoleAsync(user, role.Name!);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return Result.Error($"Failed to add role: {errors}");
            }

            return Result.Ok("Role added successfully!");
        }
        catch (Exception ex)
        {
            return Result.Error($"Error adding role: {ex.Message}");
        }
    }

    public async Task<Result> DeleteRoleAsync(Guid userId, Guid roleId)
    {
        try
        {
            await using var db = await dbFactory.CreateDbContextAsync();

            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user is null) return Result.Error("User not found!");

            var role = await db.Roles.FindAsync(roleId);
            if (role is null) return Result.Error("Role not found!");

            var userRoles = await userManager.GetRolesAsync(user);
            if (!userRoles.Contains(role.Name!)) return Result.Error("Role is not assigned to this user!");

            var result = await userManager.RemoveFromRoleAsync(user, role.Name!);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return Result.Error($"Failed to remove role: {errors}");
            }

            return Result.Ok("Role removed successfully!");
        }
        catch (Exception ex)
        {
            return Result.Error($"Error removing role: {ex.Message}");
        }
    }
    #endregion

    public async Task<bool> IsUserAvailableAsync(string email)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        return await db.Users.AsNoTracking().AnyAsync(x => x.Email!.ToLower().Equals(email.ToLower()));
    }
}

public static class IdentityExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var userIdClaim = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        return userIdClaim != null ? Guid.Parse(userIdClaim) : Guid.Empty;
    }
    // Korištenje:
    // Guid userId = contextAccessor.HttpContext?.User.GetUserId() ?? Guid.Empty;
}