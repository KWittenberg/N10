namespace N10.Repository.Interfaces;

public interface IMovieRepository
{
    Task<Result<List<MovieDto>>> GetAllAsync();

    Task<Result<MovieDto>> GetByIdAsync(int id);

    Task<Result> AddAsync(MovieInput input);

    Task<Result> UpdateAsync(int id, MovieInput input);

    Task<Result> DeleteAsync(int id);




    Task<Result<PaginatedResult<MovieDto>>> GetPagedAsync(int pageNumber, int pageSize);


    Task<Result<PaginatedResult<MovieDto>>> GetFilteredPagedAsync(int pageNumber, int pageSize, MovieFilter? filter = null);

    Task<Result<FilterOptionsDto>> GetFilterOptionsAsync();





    Task SyncParsedMoviesAsync(string path);

    Task PopulateFromTmdbAsync();

    Task<Result> PopulateFromTmdbByIdAsync(int id);
}