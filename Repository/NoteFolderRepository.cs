namespace N10.Repository;

public class NoteFolderRepository(IDbContextFactory<ApplicationDbContext> context) : INoteFolderRepository
{
    readonly string entityName = "NoteFolder";

    public async Task<Result<List<NoteFolderDto>>> GetAllAsync()
    {
        await using var db = await context.CreateDbContextAsync();

        var dtos = await db.NoteFolders.AsNoTracking().OrderBy(x => x.Name).Select(NoteFolderMapping.ToDtoExpression).ToListAsync();
        if (dtos.Count == 0) return Result<List<NoteFolderDto>>.Error($"{entityName} Not Found!");

        return Result<List<NoteFolderDto>>.Ok(dtos);
    }

    public async Task<Result<List<NoteFolderDto>>> GetSubFoldersInCurrentFolderAsync(int? userId, int? currentFolderId)
    {
        await using var db = await context.CreateDbContextAsync();

        IQueryable<NoteFolder> query = db.NoteFolders.AsNoTracking();

        if (userId.HasValue) query = query.Where(x => x.ApplicationUserId == userId.Value);
        else query = query.Where(x => x.ApplicationUserId == null);

        if (currentFolderId.HasValue) query = query.Where(x => x.ParentFolderId == currentFolderId.Value);
        else query = query.Where(x => x.ParentFolderId == null);

        var dtos = await query.OrderBy(x => x.Name).Select(NoteFolderMapping.ToDtoExpression).ToListAsync();
        if (dtos.Count == 0) return Result<List<NoteFolderDto>>.Error($"{entityName} Not Found!");

        return Result<List<NoteFolderDto>>.Ok(dtos);
    }




    public async Task<Result<NoteFolderDto>> GetByIdAsync(int id)
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

    public async Task<Result> UpdateAsync(int id, NoteFolderInput input)
    {
        await using var db = await context.CreateDbContextAsync();

        var entity = await db.NoteFolders.FindAsync(id);
        if (entity is null) return Result.Error($"{entityName} Not Found!");

        entity.UpdateFromInput(input);

        db.NoteFolders.Update(entity);
        await db.SaveChangesAsync();

        return Result.Ok($"{entityName} Updated!");
    }

    public async Task<Result> DeleteAsync(int id)
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





    public async Task<Result<List<NoteFolderDto>>> GetPathToFolderAsync(int? userId, int? folderId)
    {
        await using var db = await context.CreateDbContextAsync();

        var path = new List<NoteFolderDto>();
        var currentId = folderId;

        while (currentId != null && currentId != null)
        {
            var current = await db.NoteFolders.AsNoTracking()
                .Where(x => x.Id == currentId)
                .Where(x => userId.HasValue ? x.ApplicationUserId == userId.Value : x.ApplicationUserId == null)
                .Select(NoteFolderMapping.ToDtoExpression)
                .FirstOrDefaultAsync();

            if (current == null) break;
            path.Insert(0, current);  // Dodaj na početak za root -> ... -> current
            currentId = current.ParentFolderId;
        }
        return Result<List<NoteFolderDto>>.Ok(path);
    }






    public async Task<List<NoteFolder>> GetHierarchicalFoldersAsync()
    {
        await using var db = await context.CreateDbContextAsync();

        return await db.NoteFolders
            .Include(f => f.SubFolders)  // Rekurzivno sa ThenInclude ako duboko
            .ThenInclude(sf => sf.SubFolders)  // Dodajte više levela po potrebi
            .Where(f => f.ParentFolderId == null)  // Root
            .ToListAsync();
    }

    public async Task<List<NoteFolderDto>> GetPathToFolderAsync(int folderId)
    {
        // Rekurzivno build path od foldera do root-a (implementirajte sa loop-om ili query-em)
        var path = new List<NoteFolderDto>();

        await using var db = await context.CreateDbContextAsync();
        var current = await db.NoteFolders.FindAsync(folderId);
        while (current != null)
        {
            path.Insert(0, new NoteFolderDto { Id = current.Id, Name = current.Name });
            current = await db.NoteFolders.FindAsync(current.ParentFolderId);
        }
        return path;
    }
}