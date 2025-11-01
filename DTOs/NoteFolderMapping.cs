namespace N10.DTOs;

public static class NoteFolderMapping
{
    // GetAllAsync() - Expression for EF projections
    public static Expression<Func<NoteFolder, NoteFolderDto>> ToDtoExpression => x => new()
    {
        Id = x.Id,
        ApplicationUserId = x.ApplicationUserId,
        ParentFolderId = x.ParentFolderId,
        Name = x.Name ?? string.Empty,
        Color = x.Color ?? string.Empty
    };

    // GetByIdAsync() - Map a single materialized entity to DTO (use when you already have an entity instance)
    public static NoteFolderDto ToDto(this NoteFolder entity) => new()
    {
        Id = entity.Id,
        ApplicationUserId = entity.ApplicationUserId,
        ParentFolderId = entity.ParentFolderId,
        Name = entity.Name ?? string.Empty,
        Color = entity.Color ?? string.Empty
    };

    // AddAsync() - Convert Input to Entity
    public static NoteFolder ToEntity(this NoteFolderInput input) => new()
    {
        ApplicationUserId = input.ApplicationUserId,
        ParentFolderId = input.ParentFolderId,
        Name = input.Name ?? string.Empty,
        Color = input.Color ?? string.Empty
    };
    // UpdateAsync() - Update existing entity from input
    public static void UpdateFromInput(this NoteFolder entity, NoteFolderInput input)
    {
        entity.ApplicationUserId = input.ApplicationUserId;
        entity.ParentFolderId = input.ParentFolderId;
        entity.Name = input.Name ?? string.Empty;
        entity.Color = input.Color ?? string.Empty;
    }

    // UI -> Convert Dto to Input
    public static NoteFolderInput ToInput(this NoteFolderDto dto) => new()
    {
        ApplicationUserId = dto.ApplicationUserId,
        ParentFolderId = dto.ParentFolderId,
        Name = dto.Name ?? string.Empty,
        Color = dto.Color ?? string.Empty
    };
}