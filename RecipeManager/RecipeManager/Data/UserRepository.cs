using Microsoft.EntityFrameworkCore;
using RecipeManager.DataInterfaces;
using RecipeManager.Helpers;
using RecipeManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeManager.Data
{
    public class UserRepository : GenericRepository, IUserRepository
    {
        private readonly DataContext _context;

        public UserRepository(DataContext context) : base(context)
        {
            _context = context;
        }

		public async Task<Friendship> GetFriensShip(int userId, int recipientId)
		{
				return await _context.Friendships.FirstOrDefaultAsync(u => u.UserFollowId == userId && u.UserIsFollowedId == recipientId);
		}

		public async Task<Photo> GetPhoto(int id)
        {
            var photo = await _context.Photos.FirstOrDefaultAsync(p => p.Id == id);
            return photo;
        }

		public async Task<Photo> GetPhotoByUrl(string url)
		{
			var photo = await _context.Photos.FirstOrDefaultAsync(p => p.Url == url);
			return photo;
		}

        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users.Include(p => p.Photo).FirstOrDefaultAsync(u => u.Id == id);
            return user;
        }

		public override async Task<object> GetSearchResult(int userId, string name)
		{
			return await _context.Users.Include(p => p.Photo).FirstOrDefaultAsync(u => u.UserName == name && u.Id != userId);
		}

		public async Task<PagedList<User>> Getusers(UserParam userParams)
        {
			var users = _context.Users.Include(p => p.Photo).AsQueryable();

			users = users.Where(u => u.Id != userParams.UserId);

			if (userParams.UserFollow)
			{
				var userLikes = await GetUserFollows(userParams.UserId, userParams.UserFollow);
				users = users.Where(u => userLikes.Contains(u.Id));
			}

			if (userParams.UserIsFollowed)
			{
				var userIsLiked = await GetUserFollows(userParams.UserId, userParams.UserFollow);
				users = users.Where(u => userIsLiked.Contains(u.Id));
			}

			return await PagedList<User>.CreateListAsync(users, userParams.PageNumber, userParams.PageSize);
		}

		public override async Task<List<string>> SearchObjects(int userId, string term)
		{
			var users = await _context.Users.Where(u => u.UserName.Contains(term) && u.Id != userId).Select(r => r.UserName).ToListAsync();
			return users;
		}

		public async Task<bool> IsInvite(int userId)
		{
			var invite = await _context.Friendships.Where(u => u.UserIsFollowedId == userId && !u.IsAccepted).ToListAsync();

			return invite.Count > 0;
		}



		/// 

		private async Task<IEnumerable<int>> GetUserFollows(int id, bool userFollow)
		{
			var user = await _context.Users
								.Include(x => x.UserFollow)
								.Include(x => x.UserIsFollowed)
								.FirstOrDefaultAsync(u => u.Id == id);

			if (userFollow)
			{
				return user.UserFollow.Where(u => u.UserIsFollowedId == id && !u.IsAccepted).Select(i => i.UserFollowId);
			}
			else
			{
				return user.UserFollow.Where(u => u.IsAccepted).Select(i => i.UserFollowId)
									  .Union(user.UserIsFollowed.Where(u => u.IsAccepted).Select(i => i.UserIsFollowedId));
			}
		}
	}
}
