namespace N10.Entities;

public static class BaseImageEntityConst
{
    public const int FileNameMinLength = 1;

    public const int FileNameLength = 256;

    public const int TypeLength = 20;

    public const int FileUrlLength = 512;

    public const int MimeTypeLength = 100;

    public const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10 MB za slike

    public const int MaxFileSizeMB = 10;

    public const int MaxImageWidth = 3840; // 4K širina

    public const int MaxImageHeight = 2160; // 4K visina

    public static readonly string[] SupportedMimeTypes = ["image/jpeg", "image/png", "image/webp"];

    public const string AllowedExtensions = ".jpg,.jpeg,.png,.webp";
}
