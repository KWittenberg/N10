using Google.GenAI;
using Google.GenAI.Types;

namespace N10.Services;

public class AiService(IConfiguration config)
{
    readonly string apiKey = config["Google:AiStudioKey"] ?? throw new ArgumentNullException("AiStudio:ApiKey not set in configuration");
    readonly string modelId = "gemma-3-27b-it"; // gemma-3-27b-it   gemini-2.5-flash
    readonly string ModelGemma = "gemma-3-27b-it";
    readonly string ModelGemini = "gemini-2.5-flash";


    public async Task<Result<AiResult?>> EnhanceWithGemmaAsync(string prompt)
    {
        try
        {
            using var client = new Client(apiKey: apiKey);
            List<Content> content = [new() { Role = "user", Parts = [new Part { Text = prompt }] }];
            // Call the Gemma model
            var response = await client.Models.GenerateContentAsync(ModelGemma, content);
            if (response?.Candidates == null || response.Candidates.Count == 0) return Result<AiResult?>.Error($"AiError: Safety block??");

            // Izvlačenje teksta
            string rawJson = response.Candidates[0].Content.Parts[0].Text;
            // Čišćenje markdowna (Gemma to voli dodati)
            rawJson = rawJson.Replace("```json", "").Replace("```", "").Trim();

            // Pokušaj parsirati samo JSON dio ako ima smeća okolo
            int start = rawJson.IndexOf('{');
            int end = rawJson.LastIndexOf('}');
            if (start >= 0 && end > start) rawJson = rawJson.Substring(start, end - start + 1);

            // DESERIJALIZACIJA (S podrškom za č,ć,š...)
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping // OVO RJEŠAVA KODNU STRANICU:
            };

            var aiData = JsonSerializer.Deserialize<AiResult>(rawJson, options);
            if (aiData is null) return Result<AiResult?>.Error("Failed to deserialize JSON.");

            aiData.ModelName = ModelGemma;
            aiData.PromptTokens = response.UsageMetadata?.PromptTokenCount ?? 0;
            aiData.ResponseTokens = response.UsageMetadata?.CandidatesTokenCount ?? 0;
            aiData.TotalTokens = response.UsageMetadata?.TotalTokenCount ?? 0;

            // HACK: Ako iz nekog razloga tokeni nisu dostupni, napravi grubu procjenu
            if (aiData.ResponseTokens == 0 && !string.IsNullOrEmpty(rawJson))
            {
                aiData.ResponseTokens = rawJson.Length / 4; // Gruba procjena
                aiData.TotalTokens = (aiData.PromptTokens ?? 0) + aiData.ResponseTokens;
            }

            return Result<AiResult?>.Ok(aiData);
        }
        catch (Google.GenAI.ClientError ce) { return Result<AiResult?>.Error($"Google API Error: {ce.Message}"); }// Specifična greška ako model ne postoji ili je ključ krivi
        catch (JsonException) { return Result<AiResult?>.Error("AI je vratio neispravan JSON format."); }
        catch (Exception ex)
        {
            if (ex.InnerException != null) return Result<AiResult?>.Error($"AiError: {ex.Message} - Inner: {ex.InnerException.Message}");
            return Result<AiResult?>.Error($"AiError: {ex.Message}");
        }
    }

    public async Task<AiResult?> EnhanceWithGeminiAsync(string systemInstruction, string userMessage)
    {
        try
        {
            using var client = new Client(apiKey: apiKey);

            // 1. Priprema User Poruke
            List<Content> userContent = [new() { Role = "user", Parts = [new Part { Text = userMessage }] }];

            // 2. Konfiguracija (Tu ide System Prompt i JSON mode)
            var config = new GenerateContentConfig
            {
                SystemInstruction = new Content { Parts = [new Part { Text = systemInstruction }] }, // System Instruction se šalje ovdje kao Content objekt
                ResponseMimeType = "application/json", // Forsiranje JSON-a
                Temperature = 0.7f // Možeš dodati i temperaturu ako želiš preciznije/kreativnije
            };

            // 3. Poziv Metode: Model + Sadržaj + Konfiguracija
            var response = await client.Models.GenerateContentAsync(modelId, userContent, config);

            // 4. Obrada odgovora
            if (response?.Candidates == null || response.Candidates.Count == 0) return null;

            string rawJson = response.Candidates[0].Content.Parts[0].Text;

            // Čišćenje markdowna ako ga model ipak vrati
            rawJson = rawJson.Replace("```json", "").Replace("```", "").Trim();

            // Ponekad modeli vrate tekst prije JSON-a, pa tražimo zagrade
            int startIndex = rawJson.IndexOf('{');
            int endIndex = rawJson.LastIndexOf('}');
            if (startIndex >= 0 && endIndex > startIndex) rawJson = rawJson.Substring(startIndex, endIndex - startIndex + 1);

            var result = JsonSerializer.Deserialize<AiResult>(rawJson);
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"AI Greška: {ex.Message}");
            // Dobro je vidjeti inner exception ako postoji
            if (ex.InnerException != null) Console.WriteLine($"Inner: {ex.InnerException.Message}");
            return null;
        }
    }



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


    #region -- Debug metode za modele --
    public async Task DebugListModelsAsync()
    {
        string sep = "--------------------------------------------------------------------";
        Console.WriteLine(sep);
        Console.WriteLine("POPIS SVIH 'Base' MODELA:");

        try
        {
            using var client = new Client(apiKey: apiKey);

            // 1. Dohvati pager
            var response = await client.Models.ListAsync();

            // 2. Vrti redom bez ikakvih filtera
            await foreach (var model in response)
            {
                // Ispiši samo ime, to je jedino što nam treba za config
                Console.WriteLine($"- {model.Name}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ GREŠKA: {ex.Message}");
        }

        Console.WriteLine(sep);
    }

    public async Task DebugListModels2Async()
    {
        // assuming credentials are set up in environment variables as instructed above.
        using var client = new Client(apiKey: apiKey);

        // List base models with default settings
        Console.WriteLine("Base Models:");
        var pager = await client.Models.ListAsync();
        await foreach (var model in pager)
        {
            Console.WriteLine(model.Name);
        }

        // List tuned models with a page size of 10
        Console.WriteLine("Tuned Models:");
        var config = new ListModelsConfig { QueryBase = false, PageSize = 100 };
        var tunedModelsPager = await client.Models.ListAsync(config);
        await foreach (var model in tunedModelsPager)
        {
            Console.WriteLine(model.Name);
        }
    }

    // Helper za provjeru modela (Zovi samo na Admin Dashboardu npr., ne svaki put)
    public async Task<bool> CheckModelAvailability(string modelName)
    {
        try
        {
            using var client = new Client(apiKey: apiKey);
            await client.Models.GetAsync(modelName);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<(bool IsAvailable, string Message)> CheckModelAvailabilityAsync(string modelName)
    {
        try
        {
            using var client = new Client(apiKey: apiKey);
            // Pokušavamo dohvatiti info o modelu. Ako pukne 404, znači da ga nema ili nemamo pristup.
            await client.Models.GetAsync(modelName);
            return (true, "✅ Online");
        }
        catch (Google.GenAI.ClientError e)
        {
            return (false, $"❌ {e.Message}"); // Vjerojatno 404 Not Found
        }
        catch (Exception ex)
        {
            return (false, $"⚠️ Error: {ex.Message}");
        }
    }
    #endregion
}