namespace N10.Entities;

public static class BaseAttachmentEntityConst
{
    public const int FileNameMinLength = 1;

    public const int FileNameLength = 256;

    public const int FileUrlLength = 512;

    public const int MimeTypeLength = 100;

    public const long MaxFileSizeBytes = 20 * 1024 * 1024; // 20 MB kao default max veličina

    public const int MaxFileSizeMB = 20; // Max veličina fajla u MB

    public static readonly string[] SupportedMimeTypes = ["application/octet-stream"]; // Default generic, može se proširiti

    public const string AllowedExtensions = ".pdf,.docx,.xlsx,.txt"; // Primer dozvoljenih ekstenzija za attachment-e
}