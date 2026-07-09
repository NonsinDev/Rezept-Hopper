namespace Rezept_Hopper.Data.Models;

public class Folder
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public ICollection<FolderRecipe> FolderRecipes { get; set; } = [];
    public ICollection<FolderShare> Shares { get; set; } = [];
}
