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
    public class RecipesController : ControllerBase
    {
        private readonly IRecipeRepository _repo;
        private readonly IShoppingRepository _repoShopping;
		private readonly IMapper _mapper;
		private readonly IRecipeService _recipeSvc;
		private readonly IProductService _productSvc;

		public RecipesController(IRecipeRepository repo,
								 IShoppingRepository repoS,
								 IMapper mapper,
								 IRecipeService recipeSvc,
								 IProductService productSvc,
								 IEmailService emailSvc)
        {
            _repo = repo;
			_repoShopping = repoS;
            _mapper = mapper;
			_recipeSvc = recipeSvc;
			_productSvc = productSvc;
		}

        [HttpGet]
        public async Task<IActionResult> GetRecipesForUser(int userId, [FromQuery]RecipeParam recipeParams)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

			var recipesFromrepo = await _repo.GetRecipesForUser(recipeParams);

            var recipesToReturn = _mapper.Map<IEnumerable<RecipeForReturnDto>>(recipesFromrepo);

			Response.AddPagination(recipesFromrepo.CurrentPage,
								   recipesFromrepo.PageSize,
								   recipesFromrepo.TotalCount,
								   recipesFromrepo.TotalPages);

			foreach (var item in recipesToReturn)
			{
				item.Category = (RecipeCategory)recipeParams.Category;
			}

			return Ok(recipesToReturn);
        }

		[HttpGet("{id}", Name ="GetRecipe")]
		public async Task<IActionResult> GetRecipe(int userId, int id)
		{
			var recipe = await _repo.GetRecipe(id);

			var recipeToReturn = _mapper.Map<RecipeDto>(recipe);

			return Ok(recipeToReturn);
		}

		[HttpPost("Add")]
		public async Task<IActionResult> AddRecipe(int userId, RecipeDto addRecipeDto)
		{
			if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
				return Unauthorized();

			var myRecipe = await _repo.SearchObjects(userId, addRecipeDto.Name);

			if (myRecipe.Count > 0)
				return BadRequest($"Posiadasz już przepis na {addRecipeDto.Name}");

			var recipeToCreate = _mapper.Map<Recipe>(addRecipeDto);

			recipeToCreate = await _productSvc.SetCompleteAddedorEditedRecipe(userId, recipeToCreate);

			if (await _recipeSvc.AddRecipe(userId, addRecipeDto.PhotoUrl, recipeToCreate))
			{
				var recipeToReturn = _mapper.Map<RecipeDto>(recipeToCreate);

				return CreatedAtRoute("GetRecipe", new { id = userId }, recipeToReturn);
			}

			return BadRequest("Błąd dodawania przepisu");
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> EditRecipe(int userId, int id, RecipeUpdateDto recipeUpdateDto)
		{
			if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
				return Forbid();

			var recipeToUpdate = await _repo.GetRecipe(id);

			_mapper.Map(recipeUpdateDto, recipeToUpdate);

			recipeToUpdate = await _productSvc.SetCompleteAddedorEditedRecipe(userId, recipeToUpdate);

			if (recipeToUpdate.Shopping != null)
				_repoShopping.Delete(recipeToUpdate.Shopping);

			if (await _repo.SaveAll())
				return NoContent();

			throw new Exception($"Aktualizacja przepisu o id: {id} nie powiodła się");
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteRecipe(int userId, int id)
		{
			if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
				return Unauthorized();

			var recipeToDelete = await _repo.GetRecipe(id);

			if(recipeToDelete.Shopping != null)
				_repoShopping.Delete(recipeToDelete.Shopping);

			_repo.Delete(recipeToDelete);

			if (await _repo.SaveAll())
				return NoContent();

			throw new Exception("Błąd podczas usuwania przepisu");
		}

		[HttpGet("search")]
		public async Task<IActionResult> SearchRecipes(int userId, [FromQuery]string term)
		{
			List<string> recipes = await _repo.SearchObjects(userId, term);

			return Ok(recipes);
		}

		[HttpGet("search/result")]
		public async Task<IActionResult> GetRecipeResult(int userId, [FromQuery]string name)
		{
			var recipes = await _repo.GetSearchResult(userId, name) as IEnumerable<Recipe>;

			if (recipes.Count() == 0)
				return BadRequest("Nie ma takiego przepisu");

			var recipesToReturn = _mapper.Map<IEnumerable<RecipeForReturnDto>>(recipes);

			return Ok(recipesToReturn);
		}
	}
}