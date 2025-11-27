using LLama;
using LLama.Common;
using System.Text;

namespace N10.Services;

public class LocalAiService : IDisposable
{
    private readonly string _modelPath;
    private LLamaWeights? _weights;
    private LLamaContext? _context;
    private readonly object _lock = new();
    private bool _isInitialized = false;

    public LocalAiService(IConfiguration configuration)
    {
        _modelPath = configuration["LocalAI:ModelPath"]
            ?? @"C:\models\tinyllama-1.1b-chat-v1.0.Q4_K_M.gguf";

        // Eager initialization
        if (File.Exists(_modelPath))
        {
            Task.Run(() => EnsureInitialized());
        }
    }

    private void EnsureInitialized()
    {
        if (_isInitialized) return;

        lock (_lock)
        {
            if (_isInitialized) return;

            if (!File.Exists(_modelPath))
                throw new FileNotFoundException($"Model ne postoji: {_modelPath}");

            var parameters = new ModelParams(_modelPath)
            {
                ContextSize = 2048,
                GpuLayerCount = 0
            };

            _weights = LLamaWeights.LoadFromFile(parameters);
            _context = _weights.CreateContext(parameters);

            _isInitialized = true;
        }
    }

    public async Task<string> GetResponseAsync(string prompt)
    {
        try
        {
            EnsureInitialized();

            if (_context == null)
                return "❌ Model nije inicijaliziran.";

            // Jednostavan prompt bez komplikacija
            var fullPrompt = $"Question: {prompt}\nAnswer:";

            // Kreiraj executor za svaki request (stateless)
            var executor = new StatelessExecutor(_weights, _context.Params);

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

            return string.IsNullOrWhiteSpace(result)
                ? "Model nije vratio odgovor."
                : result;
        }
        catch (Exception ex)
        {
            return $"💥 Greška: {ex.Message}\n\nStack: {ex.StackTrace}";
        }
    }

    public bool IsConfigured() => File.Exists(_modelPath);

    public string GetConfigurationInfo()
    {
        var status = IsConfigured() ? "✅ Konfiguriran" : "❌ Model ne postoji";
        var initialized = _isInitialized ? "✅ Učitan" : "⏳ Čeka";
        return $"Model: {Path.GetFileName(_modelPath)}\nStatus: {status}\nInit: {initialized}";
    }

    public void Dispose()
    {
        _context?.Dispose();
        _weights?.Dispose();
    }
}