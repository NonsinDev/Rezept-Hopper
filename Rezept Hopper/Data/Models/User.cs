namespace Rezept_Hopper.Data.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Salt { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Recipe> Recipes { get; set; } = [];
    public ICollection<Folder> Folders { get; set; } = [];
    public ICollection<FolderShare> SharedFolders { get; set; } = [];
}
