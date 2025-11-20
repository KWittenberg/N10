namespace N10.Repository.Interfaces;

public interface IMovieRepository
{
    Task<Result<List<MovieDto>>> GetAllAsync();

    Task<Result<MovieDto>> GetByIdAsync(Guid id);

    Task<Result> AddAsync(MovieInput input);

    Task<Result> UpdateAsync(Guid id, MovieInput input);

    Task<Result> DeleteAsync(Guid id);




    Task<Result<PaginatedResult<MovieDto>>> GetPagedAsync(int pageNumber, int pageSize);



    Task<Result<PaginatedResult<MovieDto>>> GetFilteredPagedAsync(int pageNumber, int pageSize, MovieFilter? filter = null);

    Task<Result<FilterOptionsDto>> GetFilterOptionsAsync();





    Task<Result> PopulateFromTmdbByIdAsync(Guid id);



    Task SyncParsedMoviesAsync();

    Task PopulateFromTmdbAsync();
}