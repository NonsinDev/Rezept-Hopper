using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rezept_Hopper.Services;

public class GeminiService(IConfiguration config, IHttpClientFactory httpClientFactory)
{
    private const string ApiBaseUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent";

    public async Task<ExtractedRecipe?> ExtractRecipeAsync(string url, string prompt)
    {
        var apiKey = config["Gemini:ApiKey"] ?? throw new InvalidOperationException("Gemini API Key nicht konfiguriert.");
        var client = httpClientFactory.CreateClient();

        var fullPrompt = prompt.Replace("{URL}", url);

        var requestBody = new
        {
            contents = new[]
            {
                new { parts = new[] { new { text = fullPrompt } } }
            },
            generationConfig = new
            {
                responseMimeType = "application/json"
            }
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync($"{ApiBaseUrl}?key={apiKey}", content);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(responseJson);

        var text = doc.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString();

        if (string.IsNullOrWhiteSpace(text)) return null;

        return JsonSerializer.Deserialize<ExtractedRecipe>(text, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }
}

public class ExtractedRecipe
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }

    [JsonPropertyName("prep_time_minutes")]
    public int? PrepTimeMinutes { get; set; }

    [JsonPropertyName("cook_time_minutes")]
    public int? CookTimeMinutes { get; set; }

    [JsonPropertyName("servings")]
    public int? Servings { get; set; }

    [JsonPropertyName("cuisine")]
    public string? Cuisine { get; set; }

    [JsonPropertyName("difficulty")]
    public string? Difficulty { get; set; }

    [JsonPropertyName("ingredients")]
    public List<ExtractedIngredient> Ingredients { get; set; } = [];

    [JsonPropertyName("steps")]
    public List<string> Steps { get; set; } = [];
}

public class ExtractedIngredient
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("amount")]
    public string? Amount { get; set; }

    [JsonPropertyName("unit")]
    public string? Unit { get; set; }
}
