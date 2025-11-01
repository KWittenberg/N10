namespace N10.Entities;

public class NoteFolderDto
{
    public Guid? Id { get; set; }
    
    public Guid? ApplicationUserId { get; set; }
    
    public Guid? ParentFolderId { get; set; }
    

    public string Name { get; set; } = string.Empty;

    public string? Color { get; set; }



    public virtual ICollection<NoteFolderDto> SubFolders { get; set; } = [];
    public virtual ICollection<NoteDto> Notes { get; set; } = [];
}