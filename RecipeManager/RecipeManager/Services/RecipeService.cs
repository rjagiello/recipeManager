using RecipeManager.DataInterfaces;
using RecipeManager.Models;
using RecipeManager.ServicesInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeManager.Services
{
	public class RecipeService : IRecipeService
	{
		private readonly IRecipeRepository _recipeRepo;
		private readonly IUserRepository _userRepo;
		public RecipeService(IRecipeRepository repo, IUserRepository repoU)
		{
			_recipeRepo = repo;
			_userRepo = repoU;
		}

		public async Task<bool> AddRecipe(int userId, string photoUrl, Recipe recipe)
		{
			recipe.UserId = userId;

			if (!string.IsNullOrEmpty(photoUrl))
			{
				var photo = await _userRepo.GetPhotoByUrl(photoUrl);

				recipe.Photo = new Photo()
				{
					public_id = photo.public_id,
					Url = photo.Url,
					Description = photo.Description
				};			
			}
			_recipeRepo.Add<Recipe>(recipe);

			return await _recipeRepo.SaveAll();
		}
	}
}
