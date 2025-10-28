namespace N10.Enums;

public enum NoteSharePermission
{
    Read = 0,    // Samo čitanje (default)
    Write = 1,   // Čitanje + uređivanje
    Owner = 2    // Puni pristup, uključujući brisanje/sharing
}