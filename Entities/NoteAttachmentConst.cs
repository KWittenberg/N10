namespace N10.Entities;

public static class NoteAttachmentConst
{
    // Nasleđuje od BaseAttachmentEntityConst, ali može imati specifične
    public const long MaxFileSizeBytes = 20 * 1024 * 1024; // 20 MB za note attachment-e
    
    public const int MaxFileSizeMB = 20;
    
    public static readonly string[] SupportedMimeTypes = ["application/pdf", "application/msword", "image/jpeg"];
}
