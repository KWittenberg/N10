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
        IsMetadataFetched = x.IsMetadataFetched,
        CreatedUtc = x.CreatedUtc,
        LastModifiedUtc = x.LastModifiedUtc
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
        CreatedUtc = entity.CreatedUtc,
        LastModifiedUtc = entity.LastModifiedUtc
    };

    // AddAsync() - Convert Input to Entity
    public static Movie ToEntity(this MovieInput input) => new()
    {
        FileName = input.FileName,
        Title = input.Title,
        SortTitle = input.SortTitle,
        Year = input.Year,
        Version = input.Version,
        Resolution = input.Resolution,
        Color = input.Color,
        Source = input.Source,
        Audio = input.Audio,
        Video = input.Video,
        Release = input.Release,
        TmdbId = input.TmdbId,
        TmdbTitle = input.TmdbTitle,
        TmdbImageUrl = input.TmdbImageUrl,
        ImdbId = input.ImdbId,
        ImdbRating = input.ImdbRating,
        IsMetadataFetched = input.IsMetadataFetched,
    };

    // UpdateAsync() - Update existing entity from input
    public static void UpdateFromInput(this Movie entity, MovieInput input)
    {
        entity.FileName = input.FileName;
        entity.Title = input.Title;
        entity.SortTitle = input.SortTitle;
        entity.Year = input.Year;
        entity.Version = input.Version;
        entity.Resolution = input.Resolution;
        entity.Color = input.Color;
        entity.Source = input.Source;
        entity.Audio = input.Audio;
        entity.Video = input.Video;
        entity.Release = input.Release;
        entity.TmdbId = input.TmdbId;
        entity.TmdbTitle = input.TmdbTitle;
        entity.TmdbImageUrl = input.TmdbImageUrl;
        entity.ImdbId = input.ImdbId;
        entity.ImdbRating = input.ImdbRating;
        entity.IsMetadataFetched = input.IsMetadataFetched;
    }

    // UI -> Convert Dto to Input
    public static MovieInput ToInput(this MovieDto dto) => new()
    {
        FileName = dto.FileName,
        Title = dto.Title,
        SortTitle = dto.SortTitle,
        Year = dto.Year,
        Version = dto.Version,
        Resolution = dto.Resolution,
        Color = dto.Color,
        Source = dto.Source,
        Audio = dto.Audio,
        Video = dto.Video,
        Release = dto.Release,
        TmdbId = dto.TmdbId,
        TmdbTitle = dto.TmdbTitle,
        TmdbImageUrl = dto.TmdbImageUrl,
        ImdbId = dto.ImdbId,
        ImdbRating = dto.ImdbRating,
        IsMetadataFetched = dto.IsMetadataFetched
    };
}