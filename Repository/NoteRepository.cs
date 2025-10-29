namespace N10.Repository;

public class NoteRepository(IDbContextFactory<ApplicationDbContext> context) : INoteRepository
{
    readonly string entityName = "Note";

    public async Task<Result<List<NoteDto>>> GetAllAsync()
    {
        await using var db = await context.CreateDbContextAsync();

        var dtos = await db.Notes.AsNoTracking().OrderBy(x => x.Title).Select(NoteMapping.ToDtoExpression).ToListAsync();
        if (dtos.Count == 0) return Result<List<NoteDto>>.Error($"{entityName} Not Found!");

        return Result<List<NoteDto>>.Ok(dtos);
    }

    public async Task<Result<NoteDto>> GetByIdAsync(Guid id)
    {
        await using var db = await context.CreateDbContextAsync();

        var entity = await db.Notes.FindAsync(id);
        if (entity is null) return Result<NoteDto>.Error($"{entityName} Not Found!");

        return Result<NoteDto>.Ok(entity.ToDto());
    }

    public async Task<Result> AddAsync(NoteInput input)
    {
        if (await IsTitleAvailableAsync(input.Title)) return Result.Error($"{entityName} with this Title already exists!");

        await using var db = await context.CreateDbContextAsync();

        var entity = input.ToEntity();

        await db.Notes.AddAsync(entity);
        await db.SaveChangesAsync();

        return Result.Ok($"{entityName} Added!");
    }

    public async Task<Result> UpdateAsync(Guid id, NoteInput input)
    {
        await using var db = await context.CreateDbContextAsync();

        var entity = await db.Notes.FindAsync(id);
        if (entity is null) return Result.Error($"{entityName} Not Found!");

        entity.UpdateFromInput(input);

        db.Notes.Update(entity);
        await db.SaveChangesAsync();

        return Result.Ok($"{entityName} Updated!");
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        await using var db = await context.CreateDbContextAsync();

        var entity = await db.Notes.FindAsync(id);
        if (entity is null) return Result.Error($"{entityName} Not Found!");

        db.Notes.Remove(entity);
        await db.SaveChangesAsync();

        return Result.Ok($"{entityName} Deleted!");
    }

    async Task<bool> IsTitleAvailableAsync(string title)
    {
        await using var db = await context.CreateDbContextAsync();

        return await db.Notes.AnyAsync(x => x.Title.Equals(title));
    }
}