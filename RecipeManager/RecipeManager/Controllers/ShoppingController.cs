using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.DataInterfaces;
using RecipeManager.Dtos;
using RecipeManager.Models;
using RecipeManager.ServicesInterfaces;

namespace RecipeManager.Controllers
{
	[Authorize]
	[Route("api/users/{userId}/[controller]")]
	[ApiController]
	public class ShoppingController : ControllerBase
	{
		private readonly IShoppingRepository _shoppingRepo;
		private readonly IRecipeRepository _recipeRepo;
		private readonly IFridgeRepository _fridgeRepo;
		private readonly IMapper _mapper;
		private readonly IProductService _productSvc;

		public ShoppingController(IShoppingRepository repo,
								  IRecipeRepository repoR,
								  IFridgeRepository repoF,
								  IMapper mapper,
								  IProductService prodSvc)
		{
			_shoppingRepo = repo;
			_recipeRepo = repoR;
			_fridgeRepo = repoF;
			_mapper = mapper;
			_productSvc = prodSvc;
		}

		[HttpGet("{id}", Name = "GetShoppingList")]
		public async Task<IActionResult> GetShoppingList(int id)
		{
			var shoppingList = await _shoppingRepo.GetShoppingList(id);

			var shoppingListToReturn = _mapper.Map<ShoppingListsDto>(shoppingList);


			return Ok(shoppingListToReturn);
		}

		[HttpGet]
		public async Task<IActionResult> GetShoppingLists(int userId)
		{
			if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
				return Unauthorized();

			var shoppingListsFromRepo = await _shoppingRepo.GetAllShoppingLists(userId);

			var shoppingListsToReturn = _mapper.Map<IEnumerable<ShoppingListsDto>>(shoppingListsFromRepo);

			var fridge = await _productSvc.GetAllProductsForFridge(userId);

			foreach (var item in shoppingListsToReturn)
			{
				item.Products = _productSvc.AddFridgeProducts(userId, item.Products, fridge);
			}

			if (shoppingListsToReturn != null)
				return Ok(shoppingListsToReturn);

			return NoContent();
		}

		[HttpPost]
		public async Task<IActionResult> AddShoppingList(int userId, ShoppingListToAddDto shoppingList)
		{
			if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
				return Unauthorized();

			var shoppingListToCreate = _mapper.Map<Shopping>(shoppingList);
			shoppingListToCreate.UserId = userId;

			_shoppingRepo.Add<Shopping>(shoppingListToCreate);

			if (await _shoppingRepo.SaveAll())
			{
				var shoppingListToReturn = _mapper.Map<ShoppingListsDto>(shoppingListToCreate);
				var fridge = await _productSvc.GetAllProductsForFridge(userId);
				shoppingListToReturn.Products = _productSvc.AddFridgeProducts(userId, shoppingListToReturn.Products, fridge);

				return CreatedAtRoute("GetShoppingList", new { id = shoppingListToCreate.Id }, shoppingListToReturn);
			}

			return BadRequest("Błąd dodawania listy zakupów");
		}

		[HttpPost("{id}")]
		public async Task<IActionResult> AddRecipeToShoppingList(int userId, int id)
		{
			if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
				return Unauthorized();

			var recipe = await _recipeRepo.GetRecipe(id);

			if (recipe.IsListAdded)
				return BadRequest("Już dodałeś przepis do listy zakupów");


			Shopping listToAdd = new Shopping()
			{
				UserId = userId,
				Name = recipe.Name,
				RecipeId = recipe.Id,
				Products = recipe.Products.ToList()
			};

			recipe.IsListAdded = true;

			_recipeRepo.Add(listToAdd);

			if (await _shoppingRepo.SaveAll())
				return NoContent();

			return BadRequest("Błąd dodawania przepisu do listy");
		}

		[HttpPut]
		public async Task<IActionResult> FinishShopping(int userId, ShoppingListsDto shoppingList)
		{
			if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
				return Unauthorized();

			if (await _productSvc.FinishShopping(userId, shoppingList, _mapper.Map<IEnumerable<Product>>(shoppingList.Products)))
				return NoContent();

			throw new Exception($"Błąd podczas kończenia zakupów");
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteShoppingList(int userId, int id)
		{
			if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
				return Unauthorized();

			var listToDelete = await _shoppingRepo.GetShoppingList(id);

			if (listToDelete.RecipeId != null)
			{
				var recipe = await _recipeRepo.GetRecipe(listToDelete.RecipeId ?? default);
				recipe.IsListAdded = false;
				listToDelete.Products = null;
			}
			else
				_fridgeRepo.DeleteRange(listToDelete.Products);
			
			_shoppingRepo.Delete(listToDelete);

			if (await _shoppingRepo.SaveAll())
				return NoContent();

			throw new Exception("Błąd podczas usuwania listy zakupów");
		}

		[HttpDelete]
		public async Task<IActionResult> DeleteAllShoppingLists(int userId)
		{
			if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
				return Unauthorized();

			var lists = await _shoppingRepo.GetAllShoppingLists(userId);

			foreach (var item in lists)
			{
				if (item.RecipeId != null)
				{
					var recipe = await _recipeRepo.GetRecipe(item.RecipeId ?? default);
					recipe.IsListAdded = false;
					item.Products = null;
				}
				else
					_shoppingRepo.DeleteRange(item.Products);				
			}

			_shoppingRepo.DeleteRange(lists);

			if (await _shoppingRepo.SaveAll())
				return NoContent();

			throw new Exception("Błąd podczas usuwania list zakupów");
		}

		[HttpGet("products")]
		public async Task<IActionResult> GetAllProducts(int userId)
		{
			if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
				return Unauthorized();

			var productsToReturn = _mapper.Map<ICollection<ProductShoppingFinishDto>>(await _productSvc.GetAllProductsForShopping(userId));

			var fridge = await _productSvc.GetAllProductsForFridge(userId);

			productsToReturn = _productSvc.AddFridgeProducts(userId, productsToReturn, fridge);

			if (productsToReturn != null)
				return Ok(productsToReturn);

			return NoContent();
		}
	}
}