namespace N10.Repository.Interfaces;

public interface INoteFolderRepository
{
    Task<Result<List<NoteFolderDto>>> GetAllAsync();

    Task<Result<List<NoteFolderDto>>> GetSubFoldersInCurrentFolderAsync(Guid? userId, Guid? currentFolderId);

    Task<Result<NoteFolderDto>> GetByIdAsync(Guid id);

    Task<Result> AddAsync(NoteFolderInput input);

    Task<Result> UpdateAsync(Guid id, NoteFolderInput input);

    Task<Result> DeleteAsync(Guid id);




    Task<Result<List<NoteFolderDto>>> GetPathToFolderAsync(Guid? userId, Guid? folderId);



    Task<List<NoteFolder>> GetHierarchicalFoldersAsync();

    Task<List<NoteFolderDto>> GetPathToFolderAsync(Guid folderId);
}