using RecipeManager.DataInterfaces;
using RecipeManager.Dtos;
using RecipeManager.Models;
using RecipeManager.ServicesInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeManager.Services
{
	public class ProductService : IProductService
	{
		private readonly IShoppingRepository _shoppingRepo;
		private readonly IFridgeRepository _fridgeRepo;
		private readonly IRecipeRepository _recipeRepo;

		public ProductService(IShoppingRepository sRepo, IFridgeRepository fRepo, IRecipeRepository rRepo)
		{
			_shoppingRepo = sRepo;
			_fridgeRepo = fRepo;
			_recipeRepo = rRepo;
		}

		public async Task<List<Product>> GetAllProductsForShopping(int userId)
		{
			ProductsOrderer orderer = new ProductsOrderer();

			IProductsBuilder shoppingBuilder = new ShoppingProductsBuilder(_shoppingRepo);

			await orderer.BuildProducts(shoppingBuilder, userId);

			AllProducts allProducts = shoppingBuilder.GetAllProducts();

			return allProducts.GetProducts();
		}

		public async Task<List<Product>> GetAllProductsForFridge(int userId)
		{
			ProductsOrderer orderer = new ProductsOrderer();

			IProductsBuilder fridgeBuilder = new FridgeProductsBuilder(_fridgeRepo);

			await orderer.BuildProducts(fridgeBuilder, userId);

			AllProducts allProducts = fridgeBuilder.GetAllProducts();

			return allProducts.GetProducts();
		}


		public async Task<bool> FinishShopping(int userId, ShoppingListsDto shoppingList, IEnumerable<Product> products)
		{
			await CloseShoppingList(shoppingList);
			await LoadFridge(userId, products);

			return await _shoppingRepo.SaveAll();
		}

		public ICollection<ProductShoppingFinishDto> AddFridgeProducts(int userId, ICollection<ProductShoppingFinishDto> products, List<Product> fridge)
		{
			foreach (var itemF in fridge)
			{
				foreach (var item in products)
				{
					if (itemF.Name == item.Name)
						item.FridgeCount = itemF.Count;
				}
			}

			return products;
		}

		public async Task SetCompleteRecipe(int userId, string name)
		{
			var fridge = await GetAllProductsForFridge(userId);

			var recipes = await _recipeRepo.GetCompleteRecipesForUser(userId, name);

			if (recipes.Count > 0)
			{
				foreach (var itemR in recipes)
				{
					CompleteRecipe(fridge, itemR);
				}
				await _recipeRepo.SaveAll();
			}				
		}

		public async Task<Recipe> SetCompleteAddedorEditedRecipe(int userId, Recipe recipe)
		{
			var fridge = await GetAllProductsForFridge(userId);

			CompleteRecipe(fridge, recipe);

			return recipe;
		}


		/// private

		private async Task CloseShoppingList(ShoppingListsDto shoppingList)
		{
			var shoppingListFromRepo = await _shoppingRepo.GetShoppingList(shoppingList.Id);

			if (shoppingListFromRepo.RecipeId != null)
			{
				var recipe = await _recipeRepo.GetRecipe(shoppingList.RecipeId);
				recipe.IsListAdded = false;
				recipe.IsCompleteProducts = IsCompleteProducts(shoppingList.Products);
			}

			shoppingListFromRepo.Products = null;
			_shoppingRepo.Delete(shoppingListFromRepo);
		}

		private void CompleteRecipe(List<Product> fridge, Recipe recipe)
		{		
			bool isComplete = true;
			foreach (var itemP in recipe.Products)
			{
				if (fridge.Any(p => p.Name == itemP.Name))
					foreach (var itemF in fridge)
					{
						if (itemP.Name == itemF.Name && itemP.Count > itemF.Count)
						{
							isComplete = false;
						}
					}
				else
					isComplete = false;
			}

			recipe.IsCompleteProducts = isComplete;
		}

		private bool IsCompleteProducts(IEnumerable<ProductShoppingFinishDto> products)
		{
			bool isComplete = products.All(p => p.IsInFridge && p.BoughtCount + p.FridgeCount >= p.Count);

			return isComplete;
		}

		private async Task LoadFridge(int userId, IEnumerable<Product> products)
		{
			var fridge = await _fridgeRepo.GetFridge(userId);

			foreach (var item in products)
			{
				if (item.IsInFridge)
				{
					if (fridge.Products.Any(p => p.Name == item.Name))
						fridge.Products.FirstOrDefault(p => p.Name == item.Name).Count += item.Count;
					else
						fridge.Products.Add(item);
				}
			}
		}
	}


	#region Builder
	public class AllProducts
	{
		public List<Product> Products { get; set; }

		public List<Product> GetProducts()
		{
			foreach (var item in Products)
			{
				double count = 0;
				foreach (var itemRepeat in Products.Where(i => i.Name == item.Name && i.Unit == item.Unit))
				{
					count += itemRepeat.Count;
					itemRepeat.Count = 0;
				}
				item.Count = count;
			}

			return Products.Where(p => p.Count > 0).ToList();
		}
	}

	public interface IProductsBuilder
	{
		Task SetProducts(int userId);
		AllProducts GetAllProducts();
	}

	public class ShoppingProductsBuilder : IProductsBuilder
	{
		private AllProducts _allProducts = new AllProducts();

		private readonly IShoppingRepository _repo;

		public ShoppingProductsBuilder(IShoppingRepository repo)
		{
			_repo = repo;
		}

		public async Task SetProducts(int userId)
		{
			var shoppingListsFromRepo = await _repo.GetAllShoppingLists(userId);
			_allProducts.Products = new List<Product>();
			foreach (var item in shoppingListsFromRepo)
			{
				_allProducts.Products.AddRange(item.Products);
			}			
		}

		public AllProducts GetAllProducts()
		{
			return _allProducts;
		}
	}

	public class FridgeProductsBuilder : IProductsBuilder
	{
		private AllProducts _allProducts = new AllProducts();

		private readonly IFridgeRepository _repo;

		public FridgeProductsBuilder(IFridgeRepository repo)
		{
			_repo = repo;
		}

		public async Task SetProducts(int userId)
		{
			var fridgeFromRepo = await _repo.GetFridge(userId);

			_allProducts.Products = fridgeFromRepo.Products.ToList();
		}

		public AllProducts GetAllProducts()
		{
			return _allProducts;
		}
	}

	public class ProductsOrderer
	{
		public async Task BuildProducts(IProductsBuilder productsBuilder, int userId)
		{
			await productsBuilder.SetProducts(userId);
		}
	}

	#endregion
}