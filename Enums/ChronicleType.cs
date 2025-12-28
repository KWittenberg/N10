namespace N10.Enums;

public enum ChronicleType
{
    Unspecified = 0,

    // --- OSOBE ---
    Birth = 10,          // Rođenja (Trenk, Kučera, Thaller)
    Death = 11,          // Smrti (uključuje i pogibije u ratu ako je fokus na osobi)
    Biography = 12,      // Imenovanja, selidbe, vjenčanja

    // --- DRUŠTVO ---
    Religious = 20,      // Župe, svećenici, misije, kapele
    Education = 21,      // Škole, učitelji, tečajevi
    Culture = 22,        // Kazalište, glazba, knjižnice, izložbe, KUD
    Sports = 23,         // Šahovski klub, planinari, ribiči
    Politics = 24,       // Župani, općine, izbori, sabor, dekreti

    // --- DOGAĐAJI ---
    War = 30,            // Bitke, vojska, napadi, logori (Partizani, Ustaše, Turci)
    Crime = 31,          // Ubojstva, pljačke (razbojnici), suđenja
    Disaster = 32,       // Požari, poplave, vremenske nepogode ("Smrdi po dimu")

    // --- INFRASTRUKTURA ---
    Economy = 40,        // Tvornice (Zvečevo, Orljava), obrtnici, zadruge
    Infrastructure = 41, // Ceste, pruge, struja, vodovod, građevine

    Other = 99
}


public static class ChronicleTypeExtensions
{
    // --- 1. BOJE (HEX) ---
    public static string GetColor(this ChronicleType type) => type switch
    {
        // OSOBE
        ChronicleType.Birth => "#10b981",       // Emerald Green
        ChronicleType.Death => "#94a3b8",       // Slate Grey
        ChronicleType.Biography => "#f59e0b",   // Amber (Gold)

        // DRUŠTVO
        ChronicleType.Religious => "#8b5cf6",   // Violet
        ChronicleType.Education => "#3b82f6",   // Blue
        ChronicleType.Culture => "#d946ef",     // Fuchsia
        ChronicleType.Sports => "#06b6d4",      // Cyan
        ChronicleType.Politics => "#64748b",    // Blue Grey

        // DOGAĐAJI
        ChronicleType.War => "#ef4444",         // Red
        ChronicleType.Crime => "#be123c",       // Rose/Crimson
        ChronicleType.Disaster => "#f97316",    // Orange

        // INFRASTRUKTURA
        ChronicleType.Economy => "#14b8a6",     // Teal
        ChronicleType.Infrastructure => "#78716c", // Stone Grey

        _ => "#d4af37" // Default Gold (Unspecified/Other)
    };

    // --- 2. BOJE S PROZIRNOŠĆU (RGBA) ---
    public static string GetColorAlpha(this ChronicleType type, double opacity)
    {
        // Pomoćna lokalna funkcija za formatiranje
        string Rgba(int r, int g, int b) => $"rgba({r}, {g}, {b}, {opacity.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture)})";

        return type switch
        {
            ChronicleType.Birth => Rgba(16, 185, 129),
            ChronicleType.Death => Rgba(148, 163, 184),
            ChronicleType.Biography => Rgba(245, 158, 11),

            ChronicleType.Religious => Rgba(139, 92, 246),
            ChronicleType.Education => Rgba(59, 130, 246),
            ChronicleType.Culture => Rgba(217, 70, 239),
            ChronicleType.Sports => Rgba(6, 182, 212),
            ChronicleType.Politics => Rgba(100, 116, 139),

            ChronicleType.War => Rgba(239, 68, 68),
            ChronicleType.Crime => Rgba(190, 18, 60),
            ChronicleType.Disaster => Rgba(249, 115, 22),

            ChronicleType.Economy => Rgba(20, 184, 166),
            ChronicleType.Infrastructure => Rgba(120, 113, 108),

            _ => Rgba(212, 175, 55) // Default
        };
    }

    // --- 3. IKONE (Material Symbols) ---
    public static string GetIcon(this ChronicleType type) => type switch
    {
        ChronicleType.Birth => "child_care",
        ChronicleType.Death => "sentiment_dissatisfied",
        ChronicleType.Biography => "history_edu",

        ChronicleType.Religious => "church",
        ChronicleType.Education => "school",
        ChronicleType.Culture => "theater_comedy",
        ChronicleType.Sports => "trophy",
        ChronicleType.Politics => "account_balance",

        ChronicleType.War => "swords", // Pazi: provjeri jel font podržava 'swords', ako ne onda 'shield'
        ChronicleType.Crime => "gavel",
        ChronicleType.Disaster => "thunderstorm",

        ChronicleType.Economy => "factory",
        ChronicleType.Infrastructure => "construction",

        _ => "article"
    };
}