namespace N10.Services.Interfaces;

public interface IMovieService
{
    Task<List<Movie>> GetAllMoviesAsync(CancellationToken cancellationToken = default);




    // MovieInfo ParseFilename(string filename);

    // Task<MovieInfo> ParseFilenameAsync(string filename);


    // Task<List<string>> ReadFolderAsync(bool sortByYear = false);
}