namespace N10.Services;

public class AiBatchService
{
    //private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    //private readonly GeminiEnhancerService _gemini;

    //public AiBatchService(IDbContextFactory<ApplicationDbContext> contextFactory, GeminiEnhancerService gemini)
    //{
    //    _contextFactory = contextFactory;
    //    _gemini = gemini;
    //}

    //// Event da javimo UI-u dokle smo stigli
    //public event Action<int, int, string>? OnProgress;

    //public async Task ProcessUnfinishedChroniclesAsync(CancellationToken ct = default)
    //{
    //    // 1. Dohvati sve koji NEMAJU sređen sadržaj (ili nemaju naslov)
    //    using var db = await _contextFactory.CreateDbContextAsync();

    //    var idsToProcess = await db.Chronicles
    //        .Where(c => string.IsNullOrEmpty(c.EnhancedContent) || string.IsNullOrEmpty(c.Title))
    //        .Select(c => c.Id)
    //        .ToListAsync(ct);

    //    int total = idsToProcess.Count;
    //    int current = 0;

    //    // 2. Vrti petlju (ID po ID, da ne držimo context otvoren predugo)
    //    foreach (var id in idsToProcess)
    //    {
    //        if (ct.IsCancellationRequested) break;

    //        // Otvori novi, kratki scope za svaki zapis (Best Practice)
    //        using var batchDb = await _contextFactory.CreateDbContextAsync();
    //        var chronicle = await batchDb.Chronicles.FindAsync(id);

    //        if (chronicle != null)
    //        {
    //            // Javi UI-u što radimo
    //            current++;
    //            OnProgress?.Invoke(current, total, $"Obrađujem ID {id}...");

    //            // Pripremi datum za kontekst
    //            string dateContext = chronicle.Date.HasValue
    //                ? chronicle.Date.Value.ToShortDateString()
    //                : $"{chronicle.Day}. {chronicle.Month}. {chronicle.Year}.";

    //            // POZIV AI-a
    //            var result = await _gemini.AnalyzeAndEnhanceAsync(chronicle.Content, dateContext);

    //            if (result != null)
    //            {
    //                chronicle.Title = result.Title;
    //                chronicle.EnhancedContent = result.EnhancedContent;

    //                if (Enum.IsDefined(typeof(ChronicleType), result.SuggestedTypeId))
    //                {
    //                    chronicle.Type = (ChronicleType)result.SuggestedTypeId;
    //                }

    //                // SPREMI ODMAH
    //                await batchDb.SaveChangesAsync(ct);
    //            }
    //        }

    //        // Mali odmor da ne zagušimo API (iako Gemma trpi puno)
    //        await Task.Delay(100, ct);
    //    }
    //}
}