using LLama;
using LLama.Common;
using System.Text;

namespace N10.Services;

public class LocalAiService : IDisposable
{
    readonly string modelPath;
    LLamaWeights? weights;
    LLamaContext? context;
    readonly object _lock = new();
    bool isInitialized = false;

    public LocalAiService(IConfiguration configuration)
    {
        // modelPath = configuration["LocalAI:ModelPath"] ?? @"C:\models\tinyllama-1.1b-chat-v1.0.Q4_K_M.gguf";
        modelPath = configuration["LocalAI:TinyPath"];

        // Eager initialization
        if (File.Exists(modelPath)) Task.Run(() => EnsureInitialized());
    }

    void EnsureInitialized()
    {
        if (isInitialized) return;

        lock (_lock)
        {
            if (isInitialized) return;

            if (!File.Exists(modelPath)) throw new FileNotFoundException($"Model Not Found: {modelPath}");

            var parameters = new ModelParams(modelPath)
            {
                ContextSize = 2048, // The longest length of chat as memory.
                GpuLayerCount = 0   // Use CPU only
            };

            weights = LLamaWeights.LoadFromFile(parameters);
            context = weights.CreateContext(parameters);

            isInitialized = true;
        }
    }

    public async Task<string> GetResponseAsync(string prompt)
    {
        try
        {
            EnsureInitialized();

            if (context == null) return "❌ Model is not initialized.";

            var fullPrompt = $"Question: {prompt}\nAnswer:";
            var executor = new StatelessExecutor(weights, context.Params);

            var inferenceParams = new InferenceParams
            {
                MaxTokens = 256
                // NE koristimo AntiPrompts - to uzrokuje problem!
            };

            var response = new StringBuilder();

            await foreach (var token in executor.InferAsync(fullPrompt, inferenceParams))
            {
                response.Append(token);
            }

            var result = response.ToString().Trim();

            return string.IsNullOrWhiteSpace(result) ? "Model nije vratio odgovor." : result;
        }
        catch (Exception ex)
        {
            return $"💥 Greška: {ex.Message}\n\nStack: {ex.StackTrace}";
        }
    }

    public bool IsConfigured() => File.Exists(modelPath);

    public string GetConfigurationInfo()
    {
        var status = IsConfigured() ? "✅ Konfiguriran" : "❌ Model ne postoji";
        var initialized = isInitialized ? "✅ Učitan" : "⏳ Čeka";
        return $"Model: {Path.GetFileName(modelPath)}\nStatus: {status}\nInit: {initialized}";
    }

    public void Dispose()
    {
        context?.Dispose();
        weights?.Dispose();
    }
}