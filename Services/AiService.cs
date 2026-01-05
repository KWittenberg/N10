using Google.GenAI;

namespace N10.Services;

public class AiService(IConfiguration config)
{
    readonly string apiKey = config["Google:AiStudioKey"] ?? throw new ArgumentNullException("AiStudio:ApiKey not set in configuration");
    readonly string modelId = "gemma-3-27b-it"; // "gemini-2.5-flash" gemini-flash-lite-latest gemini-2.0-flash-lite


    public async Task<AiResult?> EnhanceAsync(string prompt)
    {
        try
        {
            using var client = new Client(apiKey: apiKey);
            var response = await client.Models.GenerateContentAsync(model: modelId, contents: prompt);

            string rawJson = response.Candidates[0].Content.Parts[0].Text;

            rawJson = rawJson.Replace("```json", "").Replace("```", "").Trim();

            var result = JsonSerializer.Deserialize<AiResult>(rawJson);
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"AI Greška: {ex.Message}");
            return null;
        }
    }

    public async Task<string> EnhanceChronicleTextAsync(string originalText)
    {
        // 1. Inicijalizacija klijenta
        // Pazi: U .NET 10 možda se klijent kreira malo drugačije, prati taj Quickstart
        // Ovo je bazirano na tvom snippetu:
        using var client = new Client(apiKey: apiKey);

        // 2. PROMPT (Ovo je najbitniji dio - tu govorimo AI-u što da radi)
        var prompt = $@"
                        Ti si stručni urednik i lektor za povijesnu arhivu grada Požege. 
                        Tvoj zadatak je uzeti sirovi zapis i pretvoriti ga u gramatički ispravan, čitljiv tekst spreman za objavu na Facebooku.
                        
                        PRAVILA:
                        1. Ispravi gramatičke greške i tipfelere (npr. 'gidine' -> 'godine').
                        2. NE IZMIŠLJAJ činjenice. Koristi samo informacije iz ulaznog teksta.
                        3. Ako tekst počinje s datumom ili brojem, ukloni ga (to imamo u drugom polju).
                        4. Stil treba biti informativan, blago novinarski, ali poštujući povijesni kontekst.
                        5. Ako tekst spominje osobu, napiši puno ime i prezime ako je poznato iz konteksta.
                        6. Odgovor mora biti SAMO pročišćeni tekst, bez uvoda tipa 'Evo popravljenog teksta'.
                        
                        ULAZNI TEKST:
                        {originalText}
                        ";

        try
        {
            // 3. Poziv API-ja
            var response = await client.Models.GenerateContentAsync(model: modelId, contents: prompt);

            // 4. Izvlačenje teksta
            // Pazi na strukturu responsa, ovo je iz tvog snippeta:
            string result = response.Candidates[0].Content.Parts[0].Text;

            return result.Trim();
        }
        catch (Exception ex)
        {
            // Ako pukne, vrati original ili grešku
            Console.WriteLine($"Gemini greška: {ex.Message}");
            return originalText; // Vraćamo original da ne izgubimo podatke
        }
    }

    public async Task<AiResult?> AnalyzeAndEnhanceAsync(string originalContent, string dateContext)
    {
        var prompt = $@"
                        Ti si urednik povijesnog portala 'Požeški Vremeplov'. Analiziraj ulazni tekst i vrati JSON.

                        ULAZNI TEKST: ""{originalContent}""
                        DATUM: {dateContext}
                        
                        UPUTE ZA TEKST (EnhancedContent):
                        1. Tvoj cilj je napisati zanimljivu vijest za današnji dan, na temelju ulaznog teksta.
                        2. Ako je tekst o ROĐENJU: Započni s ""Na današnji dan rođen je..."" i istakni tko je ta osoba.
                        3. Ako je tekst o SMRTI: Započni s ""Na današnji dan napustio nas je..."" ili ""Preminuo je..."".
                        4. Ako je RATNI događaj (1941-1945): Budi objektivan, koristi izraze ""zabilježeno je"", ""dogodio se sukob"".
                        5. Obavezno makni datum s početka (npr. ""0001.-1. siječnja..."").
                        6. Stil: Informativan, blago arhaičan ali čitljiv.
                        
                        ODABIR KATEGORIJE (SuggestedTypeId):
                        Odaberi NAJPRECIZNIJI broj:
                        - 10=Birth (Rođenja), 11=Death (Smrti/Pogibije), 12=Biography (Ostalo o osobi)
                        - 20=Religious (Crkva), 21=Education (Škola), 22=Culture (Kultura/Glazba), 23=Sports, 24=Politics (Uprava)
                        - 30=War (Rat/Vojska/Logori), 31=Crime (Kriminal), 32=Disaster (Požari/Vrijeme)
                        - 40=Economy (Tvornice/Zadruge), 41=Infrastructure (Gradnja/Struja)
                        - 99=Other
                        
                        TITLE:
                        Kratak naslov (max 5 riječi). Npr. ""Rođenje Baruna Trenka"" ili ""Borbe kod Čaglina"".
                        
                        JSON FORMAT (Vrati SAMO ovo):
                        {{
                          ""Title"": ""..."",
                          ""EnhancedContent"": ""..."",
                          ""SuggestedTypeId"": 30
                        }}
                        ";

        try
        {
            using var client = new Client(apiKey: apiKey);
            var response = await client.Models.GenerateContentAsync(model: modelId, contents: prompt);

            string rawJson = response.Candidates[0].Content.Parts[0].Text;

            rawJson = rawJson.Replace("```json", "").Replace("```", "").Trim();

            var result = JsonSerializer.Deserialize<AiResult>(rawJson);
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"AI Greška: {ex.Message}");
            return null;
        }
    }


    // Pomoćna metoda za ispis svih dostupnih modela (za debug)
    public async Task DebugListModelsAsync()
    {
        Console.WriteLine("--------------------------------------------------");
        Console.WriteLine("POPIS SVIH MODELA (SIROVI ISPIS):");

        try
        {
            using var client = new Client(apiKey: apiKey);

            // 1. Dohvati pager
            var response = await client.Models.ListAsync();

            // 2. Vrti redom bez ikakvih filtera
            await foreach (var model in response)
            {
                // Ispiši samo ime, to je jedino što nam treba za config
                Console.WriteLine($"👉 {model.Name}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ GREŠKA: {ex.Message}");
        }
    }
}