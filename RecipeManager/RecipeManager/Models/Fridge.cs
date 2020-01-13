using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeManager.Models
{
	public class Fridge
	{
		public int Id { get; set; }

		public User User { get; set; }
		public int UserId { get; set; }

		public ICollection<Product> Products { get; set; }
	}
}
