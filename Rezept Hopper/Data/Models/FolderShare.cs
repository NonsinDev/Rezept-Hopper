namespace Rezept_Hopper.Data.Models;

public class FolderShare
{
    public int Id { get; set; }
    public int FolderId { get; set; }
    public int SharedWithUserId { get; set; }
    public bool CanEdit { get; set; } = false;
    public DateTime SharedAt { get; set; } = DateTime.UtcNow;

    public Folder Folder { get; set; } = null!;
    public User SharedWithUser { get; set; } = null!;
}
