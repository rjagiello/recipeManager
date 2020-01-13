using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeManager.Models
{
    public class Product
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
		public ProductUnit Unit { get; set; }
        public double Count { get; set; }
        public bool IsInFridge { get; set; }

        public Recipe Recipe { get; set; }
        public int? RecipeId { get; set; }
        public Shopping Shopping { get; set; }
        public int? ShoppingId { get; set; }
		public Fridge Fridge { get; set; }
		public int? FridgeId { get; set; }
	}
}
