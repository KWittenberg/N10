namespace N10.Services.Interfaces;

public interface IMovieLibraryService
{
    MovieInfo ParseFilename(string filename);

    Task<MovieInfo> ParseFilenameAsync(string filename);
}