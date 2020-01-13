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
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly IUserRepository _repo;
		private readonly IAuthRepository _authRepo;
		private readonly IMapper _mapper;
		private readonly INotificationService _notifyService;

		public UsersController(IUserRepository repo, IMapper mapper, INotificationService notify, IAuthRepository authRepository)
		{
			_repo = repo;
			_mapper = mapper;
			_notifyService = notify;
			_authRepo = authRepository;
		}

		[HttpGet("{id}", Name = "GetUser")]
		public async Task<IActionResult> GetUser(int id)
		{
			var user = await _repo.GetUser(id);

			var userToReturn = _mapper.Map<UserForDetailedDto>(user);

			return Ok(userToReturn);
		}

		[HttpGet("search/result")]
		public async Task<IActionResult> GetUserResult([FromQuery]string userName)
		{
			var user = await _repo.GetSearchResult(int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value), userName) as User;

			if (user == null)
				return BadRequest("Nie ma takiego użytkownika");

			var userToReturn = _mapper.Map<UserForDetailedDto>(user);

			return Ok(userToReturn);
		}

		[HttpGet]
		public async Task<IActionResult> GetUsers([FromQuery]UserParam userParams)
		{
			var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

			userParams.UserId = currentUserId;

			var users = await _repo.Getusers(userParams);

			var usersToReturn = _mapper.Map<IEnumerable<UserForDetailedDto>>(users);

			Response.AddPagination(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);		

			return Ok(usersToReturn);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateUser(int id, UserForUpdateDto userForUpdateDto)
		{
			if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
				return Unauthorized();

			var userFromRepo = await _repo.GetUser(id);

			_mapper.Map(userForUpdateDto, userFromRepo);

			_authRepo.SetHashPassword(userFromRepo, userForUpdateDto.Password);

			if (await _repo.SaveAll())
				return NoContent();

			throw new Exception($"Aktualizacja użytkownika o id: {id} nie powiodła się przy zapisywaniu do bazy");
		}

		[HttpPost("{id}/friends/{recipientId}")]
		public async Task<IActionResult> FollowUser(int id, int recipientId)
		{
			if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
				return Unauthorized();

			var friendship = await _repo.GetFriensShip(id, recipientId);
			var friendshipMe = await _repo.GetFriensShip(recipientId, id);

			if (friendship != null || friendshipMe != null)
				return BadRequest("Juz obserwujesz tego uzytkownika");

			if (await _repo.GetUser(recipientId) == null)
				return NotFound();

			friendship = new Friendship
			{
				UserFollowId = id,
				UserIsFollowedId = recipientId,
				IsAccepted = false
			};

			_repo.Add<Friendship>(friendship);

			if (await _repo.SaveAll())
			{
				if (await _notifyService.SendInvitationNotify(recipientId, User.FindFirst(ClaimTypes.Name).Value) == "Success")
					return Ok();
				else
					return BadRequest("Nie udało się wysłać powiadomienia");
			}

			return BadRequest("Nie można obserwować tego użytkownika");
		}

		[HttpPut("{id}/friends/{recipientId}")]
		public async Task<IActionResult> AcceptInvitation(int id, int recipientId)
		{
			if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
				return Unauthorized();

			var friendship = await _repo.GetFriensShip(recipientId, id);
			var friendshipMe = await _repo.GetFriensShip(id, recipientId);

			if (friendship.IsAccepted)
				return BadRequest("Już zaakceptowałeś zaproszenie");

			friendship.IsAccepted = true;
			if (friendshipMe != null)
				friendshipMe.IsAccepted = true;

			if(await _repo.SaveAll())
				return NoContent();

			return BadRequest("Nie można zaakceptować tego zaproszenia");
		}

		[HttpGet("invite/{id}")]
		public async Task<IActionResult> Invite(int id)
		{
			bool isInvite = await _repo.IsInvite(id);

			return Ok(isInvite);
		}

		[HttpDelete("{id}/friends/{recipientId}")]
		public async Task<IActionResult> DeleteFriendship(int id, int recipientId)
		{
			if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
				return Unauthorized();

			var friendship = await _repo.GetFriensShip(id, recipientId);
			var friendshipMe = await _repo.GetFriensShip(recipientId, id);

			if (friendship != null)
				_repo.Delete(friendship);
			if (friendshipMe != null)
				_repo.Delete(friendshipMe);

			if (await _repo.SaveAll())
				return NoContent();

			throw new Exception("Błąd podczas usuwania znajomego");
		}


		[HttpGet("search")]
		public async Task<IActionResult> SearchUsers([FromQuery]string term)
		{
			List<string> users = await _repo.SearchObjects(int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value), term);

			return Ok(users);
		}

	}
}