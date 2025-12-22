namespace N10.Services;

public class ChronicleService(IDbContextFactory<ApplicationDbContext> contextFactory)
{





    #region Chronicle -> Loading and Parsing - Import to db
    // Metoda koja učitava i parsira datoteku, vraća listu Chronicle objekata za Preview
    public async Task<List<Chronicle>?> LoadAndParseAsync(string filePath)
    {
        if (!File.Exists(filePath)) throw new FileNotFoundException($"Nema datoteke na lokaciji: {filePath}");

        var lines = await File.ReadAllLinesAsync(filePath, Encoding.UTF8);
        if (lines.Length == 0) return null;

        List<Chronicle> parsedChronicles = [];

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            var c = ParseLine(line);
            parsedChronicles.Add(c);
        }

        return parsedChronicles;
    }

    public async Task<int> ImportToDatabaseAsync(string filePath)
    {
        // 1. Provjere
        if (!File.Exists(filePath)) throw new FileNotFoundException($"Nema datoteke na lokaciji: {filePath}");

        // Sigurnosna provjera: Jel baza već puna? 
        // Ako želiš da samo nadoda, makni ovaj dio. 
        // Ako želiš čisti start, ovo ti javlja da prvo pobrišeš.

        await using var db = await contextFactory.CreateDbContextAsync();
        if (await db.Chronicles.AnyAsync())
        {
            // Odluka: Ili throwaj grešku ili samo return 0.
            // Za sada recimo da ne želimo duplikate.
            throw new Exception("Baza već sadrži podatke! Prvo je isprazni ako želiš novi import.");
        }

        // 2. Čitanje fajla
        var lines = await File.ReadAllLinesAsync(filePath, Encoding.UTF8);
        if (lines.Length == 0) return 0;

        List<Chronicle> chroniclesBatch = new();

        // 3. Parsiranje
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            // Pozivamo tvoju ParseLine metodu (onu koju smo složili prije)
            var c = ParseLine(line);

            // Ovdje su Day, Month, Year već popunjeni u ParseLine metodi
            chroniclesBatch.Add(c);
        }

        // 4. SPREMANJE U BAZU (Bulk Insert)
        if (chroniclesBatch.Count > 0)
        {
            // Ovo dodaje sve u memoriju EF-a
            await db.Chronicles.AddRangeAsync(chroniclesBatch);

            // Ovo šalje SQL INSERT naredbu za sve odjednom
            await db.SaveChangesAsync();
        }

        return chroniclesBatch.Count;
    }




    // Regex koji "hvata" tvoj format
    // Grupe: 1=Id, 2=Dan, 3=Mjesec(tekst), 4=Godina, 5=Ostatak teksta
    static readonly Regex _formatRegex = new Regex(@"^(\d+)\.-\s*(\d+)\.\s*([a-zA-ZčćžšđČĆŽŠĐ]+)\s*(\d+)\.\s*godine\s*(.*)$", RegexOptions.Compiled);

    Chronicle ParseLine(string rawLine)
    {
        var chronicle = new Chronicle
        {
            Content = rawLine.Trim(), // Za početak stavimo sve, ako parsiranje ne uspije
            Type = ChronicleType.Unspecified
        };

        if (string.IsNullOrWhiteSpace(rawLine)) return chronicle;

        var match = _formatRegex.Match(rawLine.Trim());
        if (match.Success)
        {
            // 1. Parsiranje ID-a (Onaj 0001)
            // Ne koristimo ga za ID baze, ali možemo ga spremiti u Title ili logirati
            // int id = int.Parse(match.Groups[1].Value);
            // chronicle.Id = id;

            // 2. Parsiranje Datuma
            byte day = byte.Parse(match.Groups[2].Value);
            string monthStr = match.Groups[3].Value;
            int year = int.Parse(match.Groups[4].Value);

            chronicle.Day = day;
            chronicle.Month = DateParsingHelper.MjeseciGenitiv.TryGetValue(monthStr, out int monthVal) ? (byte?)monthVal : null;
            chronicle.Year = year;

            // Pokušaj naći broj mjeseca iz teksta
            if (DateParsingHelper.MjeseciGenitiv.TryGetValue(monthStr, out int month))
            {
                try
                {
                    // Kreiramo DateOnly
                    chronicle.Date = new DateOnly(year, month, day);
                }
                catch
                {
                    // Ako je datum nemoguć (npr. 30. veljače), ostaje null
                    Console.WriteLine($"Greška u datumu: {rawLine}");
                }
            }

            // 3. Čišćenje Sadržaja
            // Uzimamo samo onaj dio teksta nakon "godine"
            // Trimamo tabove i razmake
            chronicle.Content = match.Groups[5].Value.Trim();
        }
        else
        {
            // Ako Regex ne prolazi (format je čudan), cijela linija ide u Content
            // Ovdje ćeš kasnije ručno popravljati
            chronicle.Content = rawLine;
        }

        return chronicle;
    }
    #endregion

}

public static class DateParsingHelper
{
    public static readonly Dictionary<string, int> MjeseciGenitiv = new(StringComparer.OrdinalIgnoreCase)
    {
        { "siječnja", 1 }, { "siječanj", 1 },
        { "veljače", 2 },  { "veljača", 2 },
        { "ožujka", 3 },   { "ožujak", 3 },
        { "travnja", 4 },  { "travanj", 4 },
        { "svibnja", 5 },  { "svibanj", 5 },
        { "lipnja", 6 },   { "lipanj", 6 },
        { "srpnja", 7 },   { "srpanj", 7 },
        { "kolovoza", 8 }, { "kolovoz", 8 },
        { "rujna", 9 },    { "rujan", 9 },
        { "listopada", 10 }, { "listopad", 10 },
        { "studenoga", 11 }, { "studeni", 11 }, { "studenog", 11 }, {"studena", 11 },
        { "prosinca", 12 }, { "prosinac", 12 }
    };
}