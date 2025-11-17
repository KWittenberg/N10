namespace N10.Repository.Interfaces;

public interface IMovieRepository
{
    Task<Result<List<MovieDto>>> GetAllAsync();
}