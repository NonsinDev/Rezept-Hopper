namespace Rezept_Hopper.Services;

public class PromptService
{
    private string? _cachedPrompt;

    public async Task<string> GetRecipePromptAsync()
    {
        if (_cachedPrompt is not null) return _cachedPrompt;

        var assembly = typeof(PromptService).Assembly;
        var resourceName = "Rezept_Hopper.Prompts.recipe-extraction.md";
        
        using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new FileNotFoundException($"Embedded resource '{resourceName}' not found.");
        
        using var reader = new StreamReader(stream);
        _cachedPrompt = await reader.ReadToEndAsync();
        return _cachedPrompt;
    }
}