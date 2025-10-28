namespace N10.Repository.Interfaces;

public interface INoteRepository
{
    Task<Result<List<NoteDto>>> GetAllAsync();

    Task<Result<NoteDto>> GetByIdAsync(Guid id);

    Task<Result> AddAsync(NoteInput input);

    Task<Result> UpdateAsync(Guid id, NoteInput input);

    Task<Result> DeleteAsync(Guid id);
}