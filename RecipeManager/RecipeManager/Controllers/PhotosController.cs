using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.Data;
using RecipeManager.DataInterfaces;
using RecipeManager.Dtos;
using RecipeManager.ServicesInterfaces;

namespace RecipeManager.Controllers
{
    [Authorize]
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
		private readonly IRecipeRepository _recipeRepo;
        private readonly IMapper _mapper;
		private readonly IPhotoService _photoSvc;
        private readonly DataContext _context;

		public PhotosController(IUserRepository repository,
								IMapper mapper,
								IPhotoService photoSvc,
								IRecipeRepository recipeRepository,
								DataContext context)
        {
			_userRepo = repository;
            _mapper = mapper;
			_photoSvc = photoSvc;
			_recipeRepo = recipeRepository;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId, [FromForm]PhotoForCreationDto photoForCreationDto)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var userFromRepo = await _userRepo.GetUser(userId);

			if (userFromRepo.Photo != null)
				_photoSvc.DeletePhoto(userFromRepo.Photo.public_id);

			var photo = _photoSvc.AddPhoto(photoForCreationDto, 500, 500);

            photo.UserId = userId;
            userFromRepo.Photo = photo;

			if (await _userRepo.SaveAll())
            {
                var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo);
                return CreatedAtRoute("GetPhoto", new { id = photo.Id }, photoToReturn);
            }

            return BadRequest("Nie można dodać zdjęcia");
        }

		[HttpPost("{recipeId}")]
		public async Task<IActionResult> AddPhotoForRecipe(int userId, int recipeId, [FromForm]PhotoForCreationDto photoForCreationDto)
		{
			if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
				return Unauthorized();

			var recipeFromRepo = await _recipeRepo.GetRecipe(recipeId);

			if (recipeFromRepo.Photo != null)
				_photoSvc.DeletePhoto(recipeFromRepo.Photo.public_id);

			var photo = _photoSvc.AddPhoto(photoForCreationDto, 700, 400);

			photo.RecipeId = recipeId;
			recipeFromRepo.Photo = photo;

			if (await _userRepo.SaveAll())
			{
				var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo);
				return CreatedAtRoute("GetPhoto", new { id = photo.Id }, photoToReturn);
			}

			return BadRequest("Nie można dodać zdjęcia");
		}

		[HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photoFromRepo = await _userRepo.GetPhoto(id);

            var photoForReturn = _mapper.Map<PhotoForReturnDto>(photoFromRepo);

            return Ok(photoForReturn);
        }
    }
}