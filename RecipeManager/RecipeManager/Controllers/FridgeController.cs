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
using RecipeManager.Helpers;
using RecipeManager.Models;
using RecipeManager.ServicesInterfaces;

namespace RecipeManager.Controllers
{
	[Authorize]
	[Route("api/users/{userId}/[controller]")]
	[ApiController]
	public class FridgeController : ControllerBase
	{
		private readonly IMapper _mapper;
		private readonly IFridgeRepository _fridgeRepo;
		private readonly IRecipeRepository _recipeRepo;
		private readonly IProductService _productSvc;


		public FridgeController(IMapper mapper, IFridgeRepository repo, IRecipeRepository rRepo, IProductService productService)
		{
			_mapper = mapper;
			_fridgeRepo = repo;
			_recipeRepo = rRepo;
			_productSvc = productService;
		}


		[HttpGet]
		public async Task<IActionResult> GetFridgeForUser(int userId, [FromQuery]FridgeParam recipeParams)
		{
			if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
				return Unauthorized();

			recipeParams.UserId = userId;

			var preparedFridge = await _productSvc.GetAllProductsForFridge(userId);

			var fridgeFromRepo = _fridgeRepo.GetFridgeContent(recipeParams, preparedFridge.AsQueryable());

			var fridgeToReturn = _mapper.Map<IEnumerable<ProductEditDto>>(fridgeFromRepo);

			Response.AddPagination(fridgeFromRepo.CurrentPage,
								   fridgeFromRepo.PageSize,
								   fridgeFromRepo.TotalCount,
								   fridgeFromRepo.TotalPages);

			return Ok(fridgeToReturn);
		}

		[HttpGet("{id}", Name = "GetProduct")]
		public async Task<IActionResult> GetProduct(int userId, int id)
		{
			var product = await _fridgeRepo.GetProduct(id);

			var productToReturn = _mapper.Map<ProductEditDto>(product);

			return Ok(productToReturn);
		}

		[HttpPost]
		public async Task<IActionResult> AddProduct(int userId, ProductDto productDto)
		{
			if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
				return Unauthorized();

			var productToCreate = _mapper.Map<Product>(productDto);
			productToCreate.IsInFridge = true;

			var fridge = await _fridgeRepo.GetFridge(userId);
			fridge.Products.Add(productToCreate);

			if (await _fridgeRepo.SaveAll())
			{
				await _productSvc.SetCompleteRecipe(userId, productDto.Name);

				var productToReturn = _mapper.Map<ProductEditDto>(productToCreate);

				return CreatedAtRoute("GetProduct", new { id = userId }, productToReturn);
			}

			return BadRequest("Błąd dodawania produktu");
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteProduct(int userId, int id)
		{
			if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
				return Unauthorized();

			var fridge = await _fridgeRepo.GetFridge(userId);

			var product = await _fridgeRepo.GetProduct(id);

			fridge.Products.Remove(product);
			_fridgeRepo.Delete(product);

			if (await _fridgeRepo.SaveAll())
			{
				await _productSvc.SetCompleteRecipe(userId, product.Name);
				return NoContent();
			}

			throw new Exception("Błąd podczas usuwania produktu");
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> EditProduct(int userId, int id, ProductEditDto productDto)
		{
			if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
				return Unauthorized();

			var product = await _fridgeRepo.GetProduct(id);

			_mapper.Map(productDto, product);

			if (await _fridgeRepo.SaveAll())
			{
				await _productSvc.SetCompleteRecipe(userId, product.Name);
				return NoContent();
			}

			throw new Exception($"Aktualizacja produktu o id: {id} nie powiodła się");
		}

		[HttpGet("search")]
		public async Task<IActionResult> SearchProducts(int userId, [FromQuery]string term)
		{
			List<string> products = await _fridgeRepo.SearchObjects(userId, term);

			return Ok(products);
		}

		[HttpGet("search/result")]
		public async Task<IActionResult> GetProductResult(int userId, [FromQuery]string name)
		{
			var preparedFridge = await _productSvc.GetAllProductsForFridge(userId);

			if (preparedFridge.FirstOrDefault(p => p.Name == name) == null)
				return BadRequest("Nie ma takiego produktu");

			var productToReturn = _mapper.Map<ProductEditDto>(preparedFridge.FirstOrDefault(p => p.Name == name));

			return Ok(productToReturn);
		}
	}
}