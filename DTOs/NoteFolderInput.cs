namespace N10.Entities;

public class NoteFolderInput
{
    public Guid? ApplicationUserId { get; set; }

    public Guid? ParentFolderId { get; set; }


    public string Name { get; set; } = string.Empty;

    public string? Color { get; set; } = "#fd7e14";
}