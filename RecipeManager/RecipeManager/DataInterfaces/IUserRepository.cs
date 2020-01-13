using RecipeManager.Helpers;
using RecipeManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeManager.DataInterfaces
{
    public interface IUserRepository : IGenericRepository
    {
        Task<User> GetUser(int id);
        Task<PagedList<User>> Getusers(UserParam userParams);
        Task<Photo> GetPhoto(int id);
		Task<Photo> GetPhotoByUrl(string url);
		Task<Friendship> GetFriensShip(int userId, int recipientId);
		Task<bool> IsInvite(int userId);
	}
}
