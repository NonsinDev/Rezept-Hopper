namespace Rezept_Hopper.Data.Models;

public class Ingredient
{
    public int Id { get; set; }
    public int RecipeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Amount { get; set; }
    public string? Unit { get; set; }
    public int SortOrder { get; set; }

    public Recipe Recipe { get; set; } = null!;
}
