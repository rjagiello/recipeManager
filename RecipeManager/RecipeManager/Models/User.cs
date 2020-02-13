using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeManager.Models
{
    public class User
    {
        public int Id { get; set; }
		private string _userName;
		public string UserName
		{
			get { return _userName; }
			set
			{
				_userName = value.ToLower();
			}
		}
		public string Email { get; set; }
		public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
		public string PasswordRemindToken { get; set; }

		public Photo Photo { get; set; }
        public int? PhotoId { get; set; }

		public Fridge Fridge { get; set; }
		public int? FridgeId { get; set; }

		public ICollection<Shopping> Shoppings { get; set; }
        public ICollection<Recipe> Recipes { get; set; }
		public ICollection<Friendship> UserFollow { get; set; }
		public ICollection<Friendship> UserIsFollowed { get; set; }
	}
}
