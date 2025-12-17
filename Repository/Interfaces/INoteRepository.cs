namespace N10.Repository.Interfaces;

public interface INoteRepository
{
    Task<Result<List<NoteDto>>> GetAllAsync();

    Task<Result<List<NoteDto>>> GetNotesInCurrentFolderAsync(int? userId, int? currentFolderId, bool ascending = true);


    Task<Result<NoteDto>> GetByIdAsync(int id);

    Task<Result> AddAsync(NoteInput input);

    Task<Result> UpdateAsync(int id, NoteInput input);

    Task<Result> DeleteAsync(int id);
}