using RecipeManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeManager.DataInterfaces
{
    public interface IAuthRepository
    {
        Task<User> Login(string userName, string password);
        Task<User> Register(User user, string password);
        Task<bool> UserExist(string userName);
		Task<bool> UserExistEmail(string email);
		Task<string> SetTempPassword(string userEmail);
		User SetHashPassword(User user, string password);
	}
}
