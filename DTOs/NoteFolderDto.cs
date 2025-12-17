namespace N10.DTOs;

public class NoteFolderDto
{
    public int? Id { get; set; }

    public int? ApplicationUserId { get; set; }

    public int? ParentFolderId { get; set; }


    public string Name { get; set; } = string.Empty;

    public string? Color { get; set; }


    public bool IsExpanded { get; set; } = false;



    public virtual ICollection<NoteFolderDto> SubFolders { get; set; } = [];
    public virtual ICollection<NoteDto> Notes { get; set; } = [];
}