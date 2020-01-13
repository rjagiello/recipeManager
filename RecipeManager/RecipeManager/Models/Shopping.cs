using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeManager.Models
{
    public class Shopping
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public User User { get; set; }
        public int UserId { get; set; }

		public Recipe Recipe { get; set; }
		public int? RecipeId { get; set; }

		public ICollection<Product> Products { get; set; }
    }
}
