using RecipeManager.DataInterfaces;
using RecipeManager.Dtos;
using RecipeManager.Helpers;
using RecipeManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeManager.ServicesInterfaces
{
	public interface IProductService
	{
		Task<List<Product>> GetAllProductsForShopping(int userId);
		Task<List<Product>> GetAllProductsForFridge(int userId);
		Task<bool> FinishShopping(int userId, ShoppingListsDto shoppingList, IEnumerable<Product> products);
		ICollection<ProductShoppingFinishDto> AddFridgeProducts(int userId, ICollection<ProductShoppingFinishDto> products, List<Product> fridge);
		Task SetCompleteRecipe(int userId, string name);
		Task<Recipe> SetCompleteAddedorEditedRecipe(int userId, Recipe recipe);
	}
}
