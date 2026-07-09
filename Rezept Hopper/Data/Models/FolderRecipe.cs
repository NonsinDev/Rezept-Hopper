namespace Rezept_Hopper.Data.Models;

public class FolderRecipe
{
    public int FolderId { get; set; }
    public int RecipeId { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;

    public Folder Folder { get; set; } = null!;
    public Recipe Recipe { get; set; } = null!;
}
