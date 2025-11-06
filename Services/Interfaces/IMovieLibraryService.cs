namespace N10.Services.Interfaces;

public interface IMovieLibraryService
{
    Task<List<MovieInfo>> GetAllMoviesAsync(CancellationToken cancellationToken = default);




    // MovieInfo ParseFilename(string filename);

    // Task<MovieInfo> ParseFilenameAsync(string filename);


    // Task<List<string>> ReadFolderAsync(bool sortByYear = false);
}