using Microsoft.EntityFrameworkCore;
using Rezept_Hopper.Data;
using Rezept_Hopper.Data.Models;

namespace Rezept_Hopper.Services;

public class RecipeService(AppDbContext db)
{
    public async Task<List<Recipe>> GetUserRecipesAsync(int userId)
        => await db.Recipes
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

    public async Task<Recipe?> GetRecipeAsync(int id, int userId)
        => await db.Recipes
            .Include(r => r.Ingredients.OrderBy(i => i.SortOrder))
            .Include(r => r.Steps.OrderBy(s => s.StepNumber))
            .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

    public async Task<Recipe?> GetSharedRecipeAsync(int id, int userId)
    {
        var hasAccess = await db.FolderRecipes
            .AnyAsync(fr => fr.RecipeId == id &&
                db.FolderShares.Any(fs => fs.FolderId == fr.FolderId && fs.SharedWithUserId == userId));

        if (!hasAccess) return null;

        return await db.Recipes
            .Include(r => r.Ingredients.OrderBy(i => i.SortOrder))
            .Include(r => r.Steps.OrderBy(s => s.StepNumber))
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Recipe> SaveRecipeAsync(ExtractedRecipe extracted, string sourceUrl, int userId)
    {
        var recipe = new Recipe
        {
            UserId = userId,
            Title = extracted.Title,
            Description = extracted.Description,
            SourceUrl = sourceUrl,
            ImageUrl = extracted.ImageUrl,
            PrepTimeMinutes = extracted.PrepTimeMinutes,
            CookTimeMinutes = extracted.CookTimeMinutes,
            Servings = extracted.Servings,
            Cuisine = extracted.Cuisine,
            Difficulty = extracted.Difficulty,
            Ingredients = extracted.Ingredients.Select((ing, i) => new Ingredient
            {
                Name = ing.Name,
                Amount = ing.Amount,
                Unit = ing.Unit,
                SortOrder = i
            }).ToList(),
            Steps = extracted.Steps.Select((step, i) => new RecipeStep
            {
                StepNumber = i + 1,
                Instruction = step
            }).ToList()
        };

        db.Recipes.Add(recipe);
        await db.SaveChangesAsync();
        return recipe;
    }

    public async Task UpdateRecipeAsync(Recipe recipe)
    {
        recipe.UpdatedAt = DateTime.UtcNow;
        db.Recipes.Update(recipe);
        await db.SaveChangesAsync();
    }

    public async Task DeleteRecipeAsync(int id, int userId)
    {
        var recipe = await db.Recipes.FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);
        if (recipe is not null)
        {
            db.Recipes.Remove(recipe);
            await db.SaveChangesAsync();
        }
    }
}
