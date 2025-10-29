namespace N10.DTOs;

public static class NoteMapping
{
    // GetAllAsync() - Expression for EF projections
    public static Expression<Func<Note, NoteDto>> ToDtoExpression => x => new()
    {
        Id = x.Id,
        UserId = x.ApplicationUserId,
        NoteFolderId = x.NoteFolderId,
        Title = x.Title ?? string.Empty,
        Content = x.Content ?? string.Empty,
        Color = x.Color ?? string.Empty,
        ReminderAt = x.ReminderAt,
        IsEncrypted = x.IsEncrypted,
        EncryptionMetadata = x.EncryptionMetadata ?? string.Empty
    };

    // GetByIdAsync() - Map a single materialized entity to DTO (use when you already have an entity instance)
    public static NoteDto ToDto(this Note entity) => new()
    {
        Id = entity.Id,
        UserId = entity.ApplicationUserId,
        NoteFolderId = entity.NoteFolderId,
        Title = entity.Title ?? string.Empty,
        Content = entity.Content ?? string.Empty,
        Color = entity.Color ?? string.Empty,
        ReminderAt = entity.ReminderAt,
        IsEncrypted = entity.IsEncrypted,
        EncryptionMetadata = entity.EncryptionMetadata ?? string.Empty
    };

    // AddAsync() - Convert Input to Entity
    public static Note ToEntity(this NoteInput input) => new()
    {
        ApplicationUserId = input.UserId,
        NoteFolderId = input.NoteFolderId,
        Title = input.Title,
        Content = input.Content,
        Color = input.Color,
        ReminderAt = input.ReminderAt,
        IsEncrypted = input.IsEncrypted,
        EncryptionMetadata = input.EncryptionMetadata
    };
    // UpdateAsync() - Update existing entity from input
    public static void UpdateFromInput(this Note entity, NoteInput input)
    {
        entity.ApplicationUserId = input.UserId;
        entity.NoteFolderId = input.NoteFolderId;
        entity.Title = input.Title;
        entity.Content = input.Content;
        entity.Color = input.Color;
        entity.ReminderAt = input.ReminderAt;
        entity.IsEncrypted = input.IsEncrypted;
        entity.EncryptionMetadata = input.EncryptionMetadata;
    }

    // UI -> Convert Dto to Input
    public static NoteInput ToInput(this NoteDto dto) => new()
    {
        UserId = dto.UserId,
        NoteFolderId = dto.NoteFolderId,
        Title = dto.Title,
        Content = dto.Content,
        Color = dto.Color,
        ReminderAt = dto.ReminderAt,
        IsEncrypted = dto.IsEncrypted,
        EncryptionMetadata = dto.EncryptionMetadata
    };
}