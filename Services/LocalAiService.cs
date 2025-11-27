namespace N10.Services;

public class LocalAiService(IConfiguration configuration)
{
    readonly string modelPath = configuration["LocalAI:ModelPath"] ?? @"C:\models\tinyllama-1.1b-chat-v1.0.Q4_K_M.gguf";
    readonly string llamaPath = configuration["LocalAI:LlamaPath"] ?? @"C:\llama\llama-cli.exe";

    public async Task<string> GetResponseAsync(string prompt)
    {
        try
        {
            // Dodajemo system prompt za bolje odgovore
            var fullPrompt = $"""
            <|system|>
            Ti si korisni asistent koji pomaže korisnicima. Odgovaraj na hrvatskom jeziku ako je moguće.
            Budi prijatan, korisni i koncizan.
            </s>
            <|user|>
            {prompt}
            </s>
            <|assistant|>
            """;

            var args = $"-m \"{modelPath}\" -p \"{fullPrompt}\" -n 256 -c 2048 --temp 0.7 --repeat_penalty 1.1 --log-disable";

            var psi = new ProcessStartInfo
            {
                FileName = llamaPath,
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardOutputEncoding = System.Text.Encoding.UTF8,
                StandardErrorEncoding = System.Text.Encoding.UTF8
            };

            using var process = new Process { StartInfo = psi };

            process.Start();

            var output = await process.StandardOutput.ReadToEndAsync();
            var error = await process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            if (!string.IsNullOrEmpty(error)) Debug.WriteLine($"Llama error: {error}");

            return CleanOutput(output);
        }
        catch (Exception ex)
        {
            return $"Došlo je do greške: {ex.Message}";
        }
    }

    string CleanOutput(string text)
    {
        // Ukloni llama specifične logove i system promptove
        var lines = text.Split('\n')
            .Where(l => !l.Contains("llama_") &&
                       !l.Contains("main:") &&
                       !l.Contains("<|") &&
                       !string.IsNullOrWhiteSpace(l))
            .Select(l => l.Trim());

        var cleaned = string.Join(' ', lines);

        // Ukloni višestruke razmake
        return Regex.Replace(cleaned, @"\s+", " ").Trim();
    }

    public bool IsConfigured() => File.Exists(llamaPath) && File.Exists(modelPath);

    public string GetConfigurationInfo() => $"Model: {Path.GetFileName(modelPath)}\nLlama: {llamaPath}\nConfigured: {IsConfigured()}";
}