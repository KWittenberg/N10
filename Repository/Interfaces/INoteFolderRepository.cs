namespace N10.Repository.Interfaces;

public interface INoteFolderRepository
{
    Task<Result<List<NoteFolderDto>>> GetAllAsync();

    Task<Result<List<NoteFolderDto>>> GetSubFoldersInCurrentFolderAsync(int? userId, int? currentFolderId);

    Task<Result<NoteFolderDto>> GetByIdAsync(int id);

    Task<Result> AddAsync(NoteFolderInput input);

    Task<Result> UpdateAsync(int id, NoteFolderInput input);

    Task<Result> DeleteAsync(int id);




    Task<Result<List<NoteFolderDto>>> GetPathToFolderAsync(int? userId, int? folderId);



    Task<List<NoteFolder>> GetHierarchicalFoldersAsync();

    Task<List<NoteFolderDto>> GetPathToFolderAsync(int folderId);
}