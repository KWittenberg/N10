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

    //public async Task<Result<List<NoteDto>>> GetNotesInCurrentFolderAsync(Guid? userId, Guid? noteFolderId)
    //{
    //    await using var db = await context.CreateDbContextAsync();

    //    var dtos = await db.Notes.AsNoTracking()
    //        .Where(x => x.ApplicationUserId.Equals(userId))
    //        .Where(x => x.NoteFolder!.Equals(noteFolderId)) // jel mogu ako je null? izbaciti taj red?
    //        .OrderBy(x => x.Title).Select(NoteMapping.ToDtoExpression).ToListAsync();

    //    if (dtos.Count == 0) return Result<List<NoteDto>>.Error($"{entityName} Not Found!");

    //    return Result<List<NoteDto>>.Ok(dtos);
    //}

    public async Task<Result<List<NoteDto>>> GetNotesInCurrentFolderAsync(Guid? userId, Guid? currentFolderId, bool ascending = true)
    {
        await using var db = await context.CreateDbContextAsync();

        IQueryable<Note> query = db.Notes.AsNoTracking();

        if (userId.HasValue) query = query.Where(x => x.ApplicationUserId == userId.Value);
        else query = query.Where(x => x.ApplicationUserId == null);

        if (currentFolderId.HasValue) query = query.Where(x => x.NoteFolderId == currentFolderId.Value);
        else query = query.Where(x => x.NoteFolderId == null);

        if (ascending) query = query.OrderBy(x => x.Title);
        else query = query.OrderByDescending(x => x.Title);

        var dtos = await query.Select(NoteMapping.ToDtoExpression).ToListAsync();
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