namespace N10.DTOs;

public static class MovieMapping
{
    // GetAllAsync() - Expression for EF projections
    public static Expression<Func<Movie, MovieDto>> ToDtoExpression => x => new()
    {
        Id = x.Id,
        FileName = x.FileName,
        Title = x.Title,
        SortTitle = x.SortTitle,
        Year = x.Year,
        Version = x.Version,
        Resolution = x.Resolution,
        Color = x.Color,
        Source = x.Source,
        Audio = x.Audio,
        Video = x.Video,
        Release = x.Release,
        TmdbId = x.TmdbId,
        TmdbTitle = x.TmdbTitle,
        TmdbImageUrl = x.TmdbImageUrl,
        ImdbId = x.ImdbId,
        ImdbRating = x.ImdbRating,
        IsMetadataFetched = x.IsMetadataFetched
    };

    // GetByIdAsync() - Map a single materialized entity to DTO (use when you already have an entity instance)
    public static MovieDto ToDto(this Movie entity) => new()
    {
        Id = entity.Id,
        FileName = entity.FileName,
        Title = entity.Title,
        SortTitle = entity.SortTitle,
        Year = entity.Year,
        Version = entity.Version,
        Resolution = entity.Resolution,
        Color = entity.Color,
        Source = entity.Source,
        Audio = entity.Audio,
        Video = entity.Video,
        Release = entity.Release,
        TmdbId = entity.TmdbId,
        TmdbTitle = entity.TmdbTitle,
        TmdbImageUrl = entity.TmdbImageUrl,
        ImdbId = entity.ImdbId,
        ImdbRating = entity.ImdbRating,
        IsMetadataFetched = entity.IsMetadataFetched,
    };

    // AddAsync() - Convert Input to Entity
    //public static Movie ToEntity(this MovieInput input) => new()
    //{
    //    ApplicationUserId = input.UserId,
    //    NoteFolderId = input.NoteFolderId,
    //    Title = input.Title,
    //    Content = input.Content,
    //    Color = input.Color,
    //    ReminderAt = input.ReminderAt,
    //    IsEncrypted = input.IsEncrypted,
    //    EncryptionMetadata = input.EncryptionMetadata
    //};
    //// UpdateAsync() - Update existing entity from input
    //public static void UpdateFromInput(this Movie entity, MovieInput input)
    //{
    //    entity.ApplicationUserId = input.UserId;
    //    entity.NoteFolderId = input.NoteFolderId;
    //    entity.Title = input.Title;
    //    entity.Content = input.Content;
    //    entity.Color = input.Color;
    //    entity.ReminderAt = input.ReminderAt;
    //    entity.IsEncrypted = input.IsEncrypted;
    //    entity.EncryptionMetadata = input.EncryptionMetadata;
    //}

    //// UI -> Convert Dto to Input
    //public static MovieInput ToInput(this MovieDto dto) => new()
    //{
    //    UserId = dto.UserId,
    //    NoteFolderId = dto.NoteFolderId,
    //    Title = dto.Title,
    //    Content = dto.Content,
    //    Color = dto.Color,
    //    ReminderAt = dto.ReminderAt,
    //    IsEncrypted = dto.IsEncrypted,
    //    EncryptionMetadata = dto.EncryptionMetadata
    //};
}