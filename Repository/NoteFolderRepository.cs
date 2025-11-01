namespace N10.Repository;

public class NoteFolderRepository(IDbContextFactory<ApplicationDbContext> context) //: INoteRepository
{
    readonly string entityName = "NoteFolder";

    public async Task<Result<List<NoteFolderDto>>> GetAllAsync()
    {
        await using var db = await context.CreateDbContextAsync();

        var dtos = await db.NoteFolders.AsNoTracking().OrderBy(x => x.Name).Select(NoteFolderMapping.ToDtoExpression).ToListAsync();
        if (dtos.Count == 0) return Result<List<NoteFolderDto>>.Error($"{entityName} Not Found!");

        return Result<List<NoteFolderDto>>.Ok(dtos);
    }

    public async Task<Result<NoteFolderDto>> GetByIdAsync(Guid id)
    {
        await using var db = await context.CreateDbContextAsync();

        var entity = await db.NoteFolders.FindAsync(id);
        if (entity is null) return Result<NoteFolderDto>.Error($"{entityName} Not Found!");

        return Result<NoteFolderDto>.Ok(entity.ToDto());
    }

    public async Task<Result> AddAsync(NoteFolderInput input)
    {
        if (await IsTitleAvailableAsync(input.Name)) return Result.Error($"{entityName} with this Name already exists!");

        await using var db = await context.CreateDbContextAsync();

        var entity = input.ToEntity();

        await db.NoteFolders.AddAsync(entity);
        await db.SaveChangesAsync();

        return Result.Ok($"{entityName} Added!");
    }

    public async Task<Result> UpdateAsync(Guid id, NoteFolderInput input)
    {
        await using var db = await context.CreateDbContextAsync();

        var entity = await db.NoteFolders.FindAsync(id);
        if (entity is null) return Result.Error($"{entityName} Not Found!");

        entity.UpdateFromInput(input);

        db.NoteFolders.Update(entity);
        await db.SaveChangesAsync();

        return Result.Ok($"{entityName} Updated!");
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        await using var db = await context.CreateDbContextAsync();

        var entity = await db.NoteFolders.FindAsync(id);
        if (entity is null) return Result.Error($"{entityName} Not Found!");

        db.NoteFolders.Remove(entity);
        await db.SaveChangesAsync();

        return Result.Ok($"{entityName} Deleted!");
    }

    async Task<bool> IsTitleAvailableAsync(string title)
    {
        await using var db = await context.CreateDbContextAsync();

        return await db.NoteFolders.AnyAsync(x => x.Name.Equals(title));
    }
}