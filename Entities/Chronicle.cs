namespace N10.Entities;

public class Chronicle : BaseEntity
{
    public byte? Day { get; set; }
    public byte? Month { get; set; }
    public int? Year { get; set; }


    public DateOnly? Date { get; set; }

    public string? Title { get; set; }

    public required string Content { get; set; } = string.Empty;

    public string? EnhancedContent { get; set; }

    public string? Note { get; set; }

    public ChronicleType Type { get; set; } = ChronicleType.Unspecified;

    public bool IsApproved { get; set; } = false;
}


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



public class SocialPost : BaseAuditableEntity
{
    public int ChronicleId { get; set; }

    public virtual Chronicle? Chronicle { get; set; }

    public string TextForPublish { get; set; } = string.Empty;

    public DateTime PublishedAt { get; set; }
}