using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeManager.Models
{
    public class Recipe
    {
        public int Id { get; set; }
		private string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value.ToLower();
			}
		}
		public string Description { get; set; }
        public RecipeCategory Category { get; set; }
        public string Preparation { get; set; }
		public int Portions { get; set; }
		public bool IsCompleteProducts { get; set; }
        public bool IsListAdded { get; set; }

        public User User { get; set; }
        public int UserId { get; set; }
        public Photo Photo { get; set; }
        public int? PhotoId { get; set; }
		public Shopping Shopping { get; set; }
		public int? ShoppingId { get; set; }

		public ICollection<Product> Products { get; set; }
    }
}
