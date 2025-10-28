namespace N10.Entities;

public static class NoteConst
{
    public const int TitleMinLength = 1;
    
    public const int TitleLength = 200;
    
    public const int ContentLength = int.MaxValue; // Neograničeno za tekst, ali EF može handle-ati kao TEXT
    
    public const int ColorLength = 7; // #RRGGBB hex format
    
    public const int EncryptionMetadataLength = 512; // Za metadata enkripcije
}
