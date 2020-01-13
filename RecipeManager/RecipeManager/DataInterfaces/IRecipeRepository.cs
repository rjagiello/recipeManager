using RecipeManager.Helpers;
using RecipeManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeManager.DataInterfaces
{
    public interface IRecipeRepository : IGenericRepository
	{
        Task<PagedList<Recipe>> GetRecipesForUser(RecipeParam recipeParams);
        Task<Recipe> GetRecipe(int recipeId);
		Task<List<Recipe>> GetCompleteRecipesForUser(int userId, string productName);
	}
}
