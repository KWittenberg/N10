﻿namespace N10.Repository;

public class RoleRepository(RoleManager<IdentityRole<Guid>> roleManager, IValidator<RoleInput> validator) : IRoleRepository
{
    public async Task<Result<IQueryable<RoleDto>>> GetAllQueryableAsync()
    {
        try
        {
            var query = roleManager.Roles.AsNoTracking().OrderBy(x => x.Name).Select(RoleMapping.ToDtoExpression).AsQueryable();
            if (!await query.AnyAsync()) return Result<IQueryable<RoleDto>>.Error("Roles not found!");

            return Result<IQueryable<RoleDto>>.Ok(query);
        }
        catch (Exception ex)
        {
            return Result<IQueryable<RoleDto>>.Error($"Error: {ex.Message}");
        }
    }

    public async Task<Result<List<RoleDto>>> GetAllListAsync()
    {
        try
        {
            var entities = await roleManager.Roles.OrderBy(x => x.Name).ToListAsync();
            if (!entities.Any()) return Result<List<RoleDto>>.Error("Roles not found!");

            return Result<List<RoleDto>>.Ok(entities.ToDtoList());

            // Alternative without RoleMapping
            // return Result<List<RoleDto>>.Ok(entities.Select(e => e.ToDto()).ToList());
        }
        catch (Exception ex)
        {
            return Result<List<RoleDto>>.Error($"Error: {ex.Message}");
        }
    }

    public async Task<Result<RoleDto>> GetByIdAsync(Guid id)
    {
        try
        {
            var entity = await roleManager.FindByIdAsync(id.ToString());
            if (entity is null) return Result<RoleDto>.Error("Role not found!");

            return Result<RoleDto>.Ok(entity.ToDto());
        }
        catch (Exception ex)
        {
            return Result<RoleDto>.Error($"Error: {ex.Message}");
        }
    }

    public async Task<Result> AddAsync(RoleInput input)
    {
        try
        {
            var validateInput = await validator.ValidateAsync(input);
            if (!validateInput.IsValid) return Result.Error(string.Join(Environment.NewLine, validateInput.Errors.Select(e => e.ErrorMessage)));

            var role = new IdentityRole<Guid>(input.Name);
            var result = await roleManager.CreateAsync(role);
            if (!result.Succeeded) return Result.Error(string.Join(Environment.NewLine, result.Errors.Select(e => e.Description)));

            return Result.Ok("Role added!");
        }
        catch (Exception ex)
        {
            return Result.Error($"Unexpected error: {ex.Message}");
        }
    }

    public async Task<Result> UpdateAsync(Guid id, RoleInput input)
    {
        try
        {
            var validateInput = await validator.ValidateAsync(input);
            if (!validateInput.IsValid) return Result.Error(string.Join(Environment.NewLine, validateInput.Errors.Select(e => e.ErrorMessage)));

            var role = await roleManager.FindByIdAsync(id.ToString());
            if (role is null) return Result.Error("Role not found!");

            var existingRole = await roleManager.FindByNameAsync(input.Name);
            if (existingRole != null && existingRole.Id != id) return Result.Error("Role with this name already exists!");

            role.UpdateFromInput(input);
            var result = await roleManager.UpdateAsync(role);

            if (!result.Succeeded) return Result.Error(string.Join(Environment.NewLine, result.Errors.Select(e => e.Description)));

            return Result.Ok("Role updated!");
        }
        catch (Exception ex)
        {
            return Result.Error($"Error: {ex.Message}");
        }
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        try
        {
            var role = await roleManager.FindByIdAsync(id.ToString());
            if (role is null) return Result.Error("Role not found!");

            var result = await roleManager.DeleteAsync(role);
            if (!result.Succeeded) return Result.Error(string.Join(Environment.NewLine, result.Errors.Select(e => e.Description)));

            return Result.Ok("Role deleted!");
        }
        catch (Exception ex)
        {
            return Result.Error($"Error: {ex.Message}");
        }
    }




    #region OLD CRUD using direct db
    // Mane Direktnog Pristupa:
    // Gubiš Identity-specific feature-e(npr.automatska normalizacija).
    // Teže testirati(manager je lakše mockati sa Moq ili sličnim).
    // U budućnosti, ako dodaješ role claims ili permissions, manager je neophodan.

    // Inject:
    // IDbContextFactory<ApplicationDbContext> dbFactory,
    // ApplicationDbContext context,

    //public async Task<Result<IQueryable<RoleDto>>> GetAllQueryableAsync()
    //{
    //    try
    //    {
    //        // Alternative without RoleMapping
    //        //var query = context.Roles.AsNoTracking().OrderBy(x => x.Name).Select(ir => new RoleDto
    //        //{
    //        //    Id = ir.Id,
    //        //    Name = ir.Name ?? string.Empty
    //        //}).AsQueryable();

    //        var query = context.Roles.AsNoTracking().OrderBy(x => x.Name).Select(RoleMapping.ToDtoExpression).AsQueryable();

    //        if (!await query.AnyAsync()) return Result<IQueryable<RoleDto>>.Error("Roles not found!");

    //        return Result<IQueryable<RoleDto>>.Ok(query);
    //    }
    //    catch (Exception ex)
    //    {
    //        return Result<IQueryable<RoleDto>>.Error($"Error: {ex.Message}");
    //    }
    //}

    //public async Task<Result<List<RoleDto>>> GetAllListAsync()
    //{
    //    try
    //    {
    //        await using var db = await dbFactory.CreateDbContextAsync();

    //        var entities = await db.Roles.OrderBy(x => x.Name).ToListAsync();
    //        if (!entities.Any()) return Result<List<RoleDto>>.Error("Roles not found!");

    //        return Result<List<RoleDto>>.Ok(entities.ToDtoList());

    //        // Alternative without RoleMapping
    //        // return Result<List<RoleDto>>.Ok(entities.Select(e => e.ToDto()).ToList());
    //    }
    //    catch (Exception ex)
    //    {
    //        return Result<List<RoleDto>>.Error($"Error: {ex.Message}");
    //    }
    //}

    //public async Task<Result<RoleDto>> GetByIdAsync(Guid id)
    //{
    //    try
    //    {
    //        await using var db = await dbFactory.CreateDbContextAsync();

    //        var entity = await db.Roles.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
    //        if (entity is null) return Result<RoleDto>.Error("Role not found!");

    //        return Result<RoleDto>.Ok(entity.ToDto());
    //    }
    //    catch (Exception ex)
    //    {
    //        return Result<RoleDto>.Error($"Error: {ex.Message}");
    //    }
    //}

    //public async Task<Result> AddAsync(RoleInput input)
    //{
    //    await using var db = await dbFactory.CreateDbContextAsync();

    //    var validateInput = await validator.ValidateAsync(input);
    //    if (!validateInput.IsValid) return Result.Error(validateInput.ToString());
    //    if (await IsNameAvailableAsync(input.Name)) return Result.Error("Role with this Name already exists!");

    //    await db.Roles.AddAsync(new IdentityRole<Guid>(input.Name));
    //    await db.SaveChangesAsync();

    //    return Result.Ok("Role Added!");
    //}

    //public async Task<Result> DeleteAsync(Guid id)
    //{
    //    await using var db = await dbFactory.CreateDbContextAsync();

    //    var entity = await db.Roles.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
    //    if (entity is null) return Result.Error("Role Not found");

    //    db.Roles.Remove(entity);
    //    await db.SaveChangesAsync();

    //    return Result.Ok("Role Deleted!");
    //}

    //async Task<bool> IsNameAvailableAsync(string name) => await context.Roles.AsNoTracking().AnyAsync(x => x.Name!.ToLower().Equals(name.ToLower()));
    #endregion
}