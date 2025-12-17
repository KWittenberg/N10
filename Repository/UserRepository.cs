namespace N10.Repository;

public class UserRepository(
        IDbContextFactory<ApplicationDbContext> dbFactory,
        IHttpContextAccessor contextAccessor,
        UserManager<ApplicationUser> userManager,
        IWebHostEnvironment webHostEnvironment,
        AuthenticationStateProvider authStateProvider,
        IValidator<UserInput> validator
    ) : IUserRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory;
    private readonly IHttpContextAccessor _contextAccessor = contextAccessor;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;
    private readonly AuthenticationStateProvider _authStateProvider = authStateProvider;
    private readonly IValidator<UserInput> _validator = validator;

    private static readonly string AvatarFolder = Path.Combine("img", "avatar");



    #region Current User
    public int? GetCurrentUserId()
    {
        var userIdClaim = _contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        return userIdClaim != null && int.TryParse(userIdClaim, out var gid) ? gid : null;
    }

    public async Task<Result<UserDto>> GetCurrentUserAsync()
    {
        var authState = await _authStateProvider.GetAuthenticationStateAsync();
        var principal = authState.User;
        if (principal?.Identity is null || !principal.Identity.IsAuthenticated) return Result<UserDto>.Error("User context unavailable");

        var user = await _userManager.GetUserAsync(principal);
        if (user is null) return Result<UserDto>.Error("Current user not found");

        await using var db = await _dbFactory.CreateDbContextAsync();
        var dto = await user.ToDtoAsync(db);
        if (dto is null) return Result<UserDto>.Error("Current user mapping failed");

        return Result<UserDto>.Ok(dto);
    }

    private UserDto? CurrentUser { get; set; }
    private Task? InitTask;

    public Task InitializeAsync()
    {
        InitTask ??= LoadCurrentUserAsync();
        return InitTask;
    }

    private async Task LoadCurrentUserAsync()
    {
        var authState = await _authStateProvider.GetAuthenticationStateAsync();
        var principal = authState.User;
        if (principal?.Identity is null || !principal.Identity.IsAuthenticated) return;

        var user = await _userManager.GetUserAsync(principal);
        if (user is null) return;

        await using var db = await _dbFactory.CreateDbContextAsync();
        CurrentUser = await user.ToDtoAsync(db);
    }
    #endregion

    #region CRUD (Users)
    public async Task<ResultQPagination<UserDto>> GetPagedAsync(int page, int pageSize, string? search = null, string? sortBy = null, bool sortDescending = false, CancellationToken cancellationToken = default)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(cancellationToken);

        var baseQuery = db.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            baseQuery = baseQuery.Where(u =>
                (u.FirstName ?? string.Empty).ToLower().Contains(s) ||
                (u.LastName ?? string.Empty).ToLower().Contains(s) ||
                (u.Email ?? string.Empty).ToLower().Contains(s)
            );
        }

        var total = await baseQuery.CountAsync(cancellationToken);

        // Sorting
        baseQuery = (sortBy?.ToLowerInvariant()) switch
        {
            "firstname" => sortDescending ? baseQuery.OrderByDescending(u => u.FirstName) : baseQuery.OrderBy(u => u.FirstName),
            "lastname" => sortDescending ? baseQuery.OrderByDescending(u => u.LastName) : baseQuery.OrderBy(u => u.LastName),
            "email" => sortDescending ? baseQuery.OrderByDescending(u => u.Email) : baseQuery.OrderBy(u => u.Email),
            _ => baseQuery.OrderBy(u => u.LastName)
        };

        // Projection including roles subquery (EF Core translates this to SQL)
        var projected = baseQuery.Skip(page * pageSize).Take(pageSize).Select(u => new UserDto
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
            LastModifiedUtc = u.LastModifiedUtc,
            Roles = db.UserRoles.Where(ur => ur.UserId == u.Id).Join(db.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new RoleDto
            {
                Id = r.Id,
                Name = r.Name ?? string.Empty
            }).ToList()
        });

        var items = await projected.ToListAsync(cancellationToken);
        var pagination = new QPaginationDto(total, page, pageSize, sortBy, sortDescending);

        return ResultQPagination<UserDto>.Ok(items, pagination);
    }

    public async Task<Result<UserDto>> GetByIdAsync(int id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null) return Result<UserDto>.Error("User not found");

        await using var db = await _dbFactory.CreateDbContextAsync();
        var dto = await user.ToDtoAsync(db);
        if (dto is null) return Result<UserDto>.Error("User mapping failed");

        return Result<UserDto>.Ok(dto);
    }

    public async Task<Result> AddAsync(UserInput input)
    {
        var validateInput = await _validator.ValidateAsync(input);
        if (!validateInput.IsValid) return Result.Error(string.Join(Environment.NewLine, validateInput.Errors.Select(e => e.ErrorMessage)));

        var user = input.ToEntity();
        var result = await _userManager.CreateAsync(user!, input.Password);
        if (!result.Succeeded) return Result.Error(string.Join(Environment.NewLine, result.Errors.Select(e => e.Description)));

        return Result.Ok("User added!");
    }

    public async Task<Result> UpdateAsync(int id, UserInput input)
    {
        var validateInput = await _validator.ValidateAsync(input);
        if (!validateInput.IsValid) return Result.Error(string.Join(Environment.NewLine, validateInput.Errors.Select(e => e.ErrorMessage)));

        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null) return Result.Error("User not found");

        user.UpdateFromInput(input);
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded) return Result.Error(string.Join(Environment.NewLine, result.Errors.Select(e => e.Description)));

        return Result.Ok("User updated!");
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null) return Result.Error("User not found");

        if (!string.IsNullOrEmpty(user.AvatarUrl))
        {
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath ?? string.Empty, user.AvatarUrl.Replace("/", "\\"));
            if (File.Exists(filePath)) File.Delete(filePath);
        }

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded) return Result.Error(string.Join(Environment.NewLine, result.Errors.Select(e => e.Description)));

        return Result.Ok("User deleted!");
    }
    #endregion

    #region Avatar
    public async Task<Result> UpdateAvatarAsync(int id, AvatarInput input, bool isOriginal)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();

        var entity = await db.Users.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null) return Result.Error("User not found!");

        if (!string.IsNullOrEmpty(entity.AvatarUrl))
        {
            var imagePath = Path.Combine(_webHostEnvironment.WebRootPath ?? string.Empty, entity.AvatarUrl.Replace("/", "\\"));
            if (File.Exists(imagePath)) File.Delete(imagePath);
        }

        var extension = Path.GetExtension(input.File!.Name);
        var fileName = $"{id}{extension}";
        var saveFolder = Path.Combine(_webHostEnvironment.WebRootPath ?? string.Empty, AvatarFolder);
        var filePath = Path.Combine(saveFolder, fileName);
        if (!Directory.Exists(saveFolder)) Directory.CreateDirectory(saveFolder);

        if (isOriginal)
        {
            await using FileStream fs = new(filePath, FileMode.Create);
            await input.File.OpenReadStream().CopyToAsync(fs);
        }
        else
        {
            // optional image processing...
        }

        var relativePath = Path.Combine(AvatarFolder, fileName).Replace("\\", "/");
        entity.AvatarUrl = relativePath;

        db.Users.Update(entity);
        await db.SaveChangesAsync();

        return Result.Ok("Avatar updated!");
    }

    public async Task<Result> DeleteAvatarAsync(int id)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();

        var entity = await db.Users.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null) return Result.Error("User not found");
        if (string.IsNullOrEmpty(entity.AvatarUrl)) return Result.Error("User Avatar not found");

        var filePath = Path.Combine(_webHostEnvironment.WebRootPath ?? string.Empty, entity.AvatarUrl.Replace("/", "\\"));
        if (File.Exists(filePath)) File.Delete(filePath);

        entity.AvatarUrl = string.Empty;
        db.Users.Update(entity);
        await db.SaveChangesAsync();

        return Result.Ok("User Avatar deleted!");
    }
    #endregion

    #region Roles (for user)
    public async Task<Result<List<RoleDto>>> GetRolesByUserIdAsync(int userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null) return Result<List<RoleDto>>.Error("User not found");

        var roleNames = await _userManager.GetRolesAsync(user);
        if (!roleNames.Any()) return Result<List<RoleDto>>.Ok(new List<RoleDto>());

        await using var db = await _dbFactory.CreateDbContextAsync();
        var roles = await db.Roles.Where(r => roleNames.Contains(r.Name!))
            .Select(RoleMapping.ToDtoExpression)
            .ToListAsync();

        return Result<List<RoleDto>>.Ok(roles);
    }

    public async Task<Result<List<RoleDto>>> GetRolesNotAssignedToUserAsync(int userId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var userRoles = db.UserRoles.Where(ur => ur.UserId == userId).Select(ur => ur.RoleId);
        var roles = await db.Roles.Where(r => !userRoles.Contains(r.Id)).Select(RoleMapping.ToDtoExpression).ToListAsync();
        return Result<List<RoleDto>>.Ok(roles);
    }

    public async Task<Result<RoleDto>> GetRoleByIdAsync(int userId, int roleId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null) return Result<RoleDto>.Error("User not found");

        await using var db = await _dbFactory.CreateDbContextAsync();
        var role = await db.Roles.Where(r => r.Id == roleId).Select(RoleMapping.ToDtoExpression).FirstOrDefaultAsync();
        if (role is null) return Result<RoleDto>.Error("Role not found");

        var userRoles = await _userManager.GetRolesAsync(user);
        if (!userRoles.Contains(role.Name)) return Result<RoleDto>.Error("Role is not assigned to this user");

        return Result<RoleDto>.Ok(role);
    }

    public async Task<Result> AddRoleAsync(int userId, int roleId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null) return Result.Error("User not found");

        await using var db = await _dbFactory.CreateDbContextAsync();
        var role = await db.Roles.FindAsync(roleId);
        if (role is null) return Result.Error("Role not found");

        var userRoles = await _userManager.GetRolesAsync(user);
        if (userRoles.Contains(role.Name!)) return Result.Error("Role already assigned to user");

        var result = await _userManager.AddToRoleAsync(user, role.Name!);
        if (!result.Succeeded) return Result.Error(string.Join(", ", result.Errors.Select(e => e.Description)));

        return Result.Ok("Role added successfully!");
    }

    public async Task<Result> DeleteRoleAsync(int userId, int roleId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null) return Result.Error("User not found");

        await using var db = await _dbFactory.CreateDbContextAsync();
        var role = await db.Roles.FindAsync(roleId);
        if (role is null) return Result.Error("Role not found");

        var userRoles = await _userManager.GetRolesAsync(user);
        if (!userRoles.Contains(role.Name!)) return Result.Error("Role is not assigned to this user");

        var result = await _userManager.RemoveFromRoleAsync(user, role.Name!);
        if (!result.Succeeded) return Result.Error(string.Join(", ", result.Errors.Select(e => e.Description)));

        return Result.Ok("Role removed successfully!");
    }

    #endregion

    #region Utility / Existence
    public async Task<bool> IsUserAvailableAsync(string email)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.Users.AsNoTracking().AnyAsync(x => x.Email!.ToLower() == email.ToLower());
    }
    #endregion
}