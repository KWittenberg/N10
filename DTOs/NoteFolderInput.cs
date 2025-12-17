namespace N10.DTOs;

public class NoteFolderInput
{
    public int? ApplicationUserId { get; set; }

    public int? ParentFolderId { get; set; }


    public string Name { get; set; } = string.Empty;

    public string? Color { get; set; } = "#fd7e14";
}