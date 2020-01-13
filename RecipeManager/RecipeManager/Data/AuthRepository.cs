using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RecipeManager.DataInterfaces;
using RecipeManager.Models;
using System;
using System.Text;
using System.Threading.Tasks;

namespace RecipeManager.Data
{
	public class AuthRepository : IAuthRepository
	{
		private readonly DataContext _context;

		public AuthRepository(DataContext context)
		{
			_context = context;
		}

		public async Task<User> Login(string userName, string password)
		{
			var user = await _context.Users.Include(p => p.Photo).FirstOrDefaultAsync(x => x.UserName == userName);

			if (user == null)
				return null;

			if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
				return null;

			return user;
		}

		public async Task<User> Register(User user, string password)
		{
			CreatePasswordHashSalt(password, out byte[] passwordHash, out byte[] passwordSalt);

			user.PasswordHash = passwordHash;
			user.PasswordSalt = passwordSalt;

			await _context.Users.AddAsync(user);
			await _context.SaveChangesAsync();

			user.Fridge = new Fridge() { UserId = user.Id };
			await _context.SaveChangesAsync();

			return user;
		}

		public User SetHashPassword(User user, string password)
		{
			CreatePasswordHashSalt(password, out byte[] passwordHash, out byte[] passwordSalt);

			user.PasswordHash = passwordHash;
			user.PasswordSalt = passwordSalt;

			return user;
		}

        public async Task<bool> UserExist(string userName)
        {
            if (await _context.Users.AnyAsync(x => x.UserName == userName))
                return true;

            return false;
        }

		public async Task<string> SetTempPassword(string userEmail)
		{
			var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);

			if (user != null)
			{
				var password = CreatePassword(10);

				CreatePasswordHashSalt(password, out byte[] passwordHash, out byte[] passwordSalt);

				user.PasswordHash = passwordHash;
				user.PasswordSalt = passwordSalt;

				await _context.SaveChangesAsync();

				return password;
			}

			return null;
		}

		public async Task<bool> UserExistEmail(string email)
		{
			if (await _context.Users.AnyAsync(x => x.Email == email))
				return true;

			return false;
		}

		///

		private string CreatePassword(int length)
		{
			const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
			StringBuilder res = new StringBuilder();
			Random rnd = new Random();
			while (0 < length--)
			{
				res.Append(valid[rnd.Next(valid.Length)]);
			}
			return res.ToString();
		}

		private void CreatePasswordHashSalt(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i])
                        return false;
                }
                return true;
            }
        }

	}
}
