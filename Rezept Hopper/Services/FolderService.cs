using Microsoft.EntityFrameworkCore;
using Rezept_Hopper.Data;
using Rezept_Hopper.Data.Models;

namespace Rezept_Hopper.Services;

public class FolderService(AppDbContext db)
{
    public async Task<List<Folder>> GetUserFoldersAsync(int userId)
        => await db.Folders
            .Where(f => f.UserId == userId)
            .Include(f => f.FolderRecipes)
            .OrderBy(f => f.Name)
            .ToListAsync();

    public async Task<List<Folder>> GetSharedFoldersAsync(int userId)
        => await db.FolderShares
            .Where(fs => fs.SharedWithUserId == userId)
            .Include(fs => fs.Folder)
                .ThenInclude(f => f.FolderRecipes)
            .Include(fs => fs.Folder)
                .ThenInclude(f => f.User)
            .Select(fs => fs.Folder)
            .ToListAsync();

    public async Task<Folder?> GetFolderAsync(int id, int userId)
        => await db.Folders
            .Include(f => f.FolderRecipes)
                .ThenInclude(fr => fr.Recipe)
            .Include(f => f.Shares)
                .ThenInclude(s => s.SharedWithUser)
            .FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId);

    public async Task<(Folder? Folder, bool CanEdit)> GetSharedFolderAsync(int id, int userId)
    {
        var share = await db.FolderShares
            .Include(fs => fs.Folder)
                .ThenInclude(f => f.FolderRecipes)
                    .ThenInclude(fr => fr.Recipe)
            .Include(fs => fs.Folder)
                .ThenInclude(f => f.User)
            .FirstOrDefaultAsync(fs => fs.FolderId == id && fs.SharedWithUserId == userId);

        return share is null ? (null, false) : (share.Folder, share.CanEdit);
    }

    public async Task<Folder> CreateFolderAsync(string name, string? description, int userId)
    {
        var folder = new Folder { Name = name, Description = description, UserId = userId };
        db.Folders.Add(folder);
        await db.SaveChangesAsync();
        return folder;
    }

    public async Task UpdateFolderAsync(Folder folder)
    {
        db.Folders.Update(folder);
        await db.SaveChangesAsync();
    }

    public async Task DeleteFolderAsync(int id, int userId)
    {
        var folder = await db.Folders.FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId);
        if (folder is not null)
        {
            db.Folders.Remove(folder);
            await db.SaveChangesAsync();
        }
    }

    public async Task AddRecipeToFolderAsync(int folderId, int recipeId, int userId)
    {
        var folder = await db.Folders.FirstOrDefaultAsync(f => f.Id == folderId && f.UserId == userId);
        if (folder is null) return;

        if (!await db.FolderRecipes.AnyAsync(fr => fr.FolderId == folderId && fr.RecipeId == recipeId))
        {
            db.FolderRecipes.Add(new FolderRecipe { FolderId = folderId, RecipeId = recipeId });
            await db.SaveChangesAsync();
        }
    }

    public async Task RemoveRecipeFromFolderAsync(int folderId, int recipeId, int userId)
    {
        var folder = await db.Folders.FirstOrDefaultAsync(f => f.Id == folderId && f.UserId == userId);
        if (folder is null) return;

        var entry = await db.FolderRecipes.FirstOrDefaultAsync(fr => fr.FolderId == folderId && fr.RecipeId == recipeId);
        if (entry is not null)
        {
            db.FolderRecipes.Remove(entry);
            await db.SaveChangesAsync();
        }
    }

    public async Task<(bool Success, string? Error)> ShareFolderAsync(int folderId, string username, bool canEdit, int ownerId)
    {
        var folder = await db.Folders.FirstOrDefaultAsync(f => f.Id == folderId && f.UserId == ownerId);
        if (folder is null) return (false, "Ordner nicht gefunden.");

        var targetUser = await db.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (targetUser is null) return (false, "Benutzer nicht gefunden.");

        if (targetUser.Id == ownerId) return (false, "Du kannst den Ordner nicht mit dir selbst teilen.");

        var existing = await db.FolderShares
            .FirstOrDefaultAsync(fs => fs.FolderId == folderId && fs.SharedWithUserId == targetUser.Id);

        if (existing is not null)
        {
            existing.CanEdit = canEdit;
            db.FolderShares.Update(existing);
        }
        else
        {
            db.FolderShares.Add(new FolderShare
            {
                FolderId = folderId,
                SharedWithUserId = targetUser.Id,
                CanEdit = canEdit
            });
        }

        await db.SaveChangesAsync();
        return (true, null);
    }

    public async Task RemoveShareAsync(int folderId, int sharedUserId, int ownerId)
    {
        var share = await db.FolderShares
            .Include(fs => fs.Folder)
            .FirstOrDefaultAsync(fs => fs.FolderId == folderId && fs.SharedWithUserId == sharedUserId && fs.Folder.UserId == ownerId);

        if (share is not null)
        {
            db.FolderShares.Remove(share);
            await db.SaveChangesAsync();
        }
    }
}
