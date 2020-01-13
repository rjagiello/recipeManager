using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeManager.Dtos
{
	public class ShoppingListToAddDto
	{
		public string Name { get; set; }

		public ICollection<ProductShoppingDto> Products{ get; set; }
	}
}
