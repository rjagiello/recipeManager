using Microsoft.EntityFrameworkCore;
using RecipeManager.DataInterfaces;
using RecipeManager.Helpers;
using RecipeManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeManager.Data
{
    public class RecipeRepository : GenericRepository, IRecipeRepository
    {
        private readonly DataContext _context;

        public RecipeRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Recipe> GetRecipe(int recipeId)
        {		
			return await _context.Recipes.Include(p => p.Photo).Include(p => p.Products).Include(s => s.Shopping).FirstOrDefaultAsync(r => r.Id == recipeId);
		}

		public override async Task<object> GetSearchResult(int userId, string name)
		{
			return await _context.Recipes.Include(p => p.Photo).Where(r => r.UserId == userId && r.Name == name).ToListAsync();
		}

		public async Task<PagedList<Recipe>> GetRecipesForUser(RecipeParam recipeParams)
        {
            var recipes = _context.Recipes.Include(p => p.Photo).AsQueryable();

            recipes = recipes.Where(u => u.UserId == recipeParams.UserId && u.Category == (RecipeCategory)recipeParams.Category);


            return await PagedList<Recipe>.CreateListAsync(recipes, recipeParams.PageNumber, recipeParams.PageSize);
		}

		public async Task<List<Recipe>> GetCompleteRecipesForUser(int userId, string productName)
		{
			return await (from recipes in _context.Recipes
					join products in _context.Products on recipes.Id equals products.RecipeId
					where recipes.UserId == userId && products.Name == productName
					select recipes).Include(p => p.Products).ToListAsync();
		}

		public override async Task<List<string>> SearchObjects(int userId, string term)
		{
			var recipes = await _context.Recipes.Where(r => r.UserId == userId && r.Name.Contains(term)).Select(r => r.Name).ToListAsync();
			return recipes;
		}

	}
}
