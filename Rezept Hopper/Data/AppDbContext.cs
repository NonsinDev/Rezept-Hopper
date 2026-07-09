using Microsoft.EntityFrameworkCore;
using Rezept_Hopper.Data.Models;

namespace Rezept_Hopper.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Recipe> Recipes => Set<Recipe>();
    public DbSet<Ingredient> Ingredients => Set<Ingredient>();
    public DbSet<RecipeStep> RecipeSteps => Set<RecipeStep>();
    public DbSet<Folder> Folders => Set<Folder>();
    public DbSet<FolderRecipe> FolderRecipes => Set<FolderRecipe>();
    public DbSet<FolderShare> FolderShares => Set<FolderShare>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FolderRecipe>()
            .HasKey(fr => new { fr.FolderId, fr.RecipeId });

        modelBuilder.Entity<FolderRecipe>()
            .HasOne(fr => fr.Folder)
            .WithMany(f => f.FolderRecipes)
            .HasForeignKey(fr => fr.FolderId);

        modelBuilder.Entity<FolderRecipe>()
            .HasOne(fr => fr.Recipe)
            .WithMany(r => r.FolderRecipes)
            .HasForeignKey(fr => fr.RecipeId);

        modelBuilder.Entity<FolderShare>()
            .HasOne(fs => fs.SharedWithUser)
            .WithMany(u => u.SharedFolders)
            .HasForeignKey(fs => fs.SharedWithUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();
    }
}
