using RecipeManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeManager.ServicesInterfaces
{
	public interface IRecipeService
	{
		Task<bool> AddRecipe(int userId, string photoUrl, Recipe recipe);
	}
}
