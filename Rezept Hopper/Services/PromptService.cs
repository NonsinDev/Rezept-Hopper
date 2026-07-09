namespace Rezept_Hopper.Services;

public class PromptService(IWebHostEnvironment env)
{
    private string? _cachedPrompt;

    public async Task<string> GetRecipePromptAsync()
    {
        if (_cachedPrompt is not null) return _cachedPrompt;

        var path = Path.Combine(env.ContentRootPath, "Prompts", "recipe-extraction.md");
        _cachedPrompt = await File.ReadAllTextAsync(path);
        return _cachedPrompt;
    }
}
