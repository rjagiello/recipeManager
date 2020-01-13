using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeManager.Dtos
{
	public class ShoppingListsDto
	{
		public int Id { get; set; }
		public string Name { get; set; }

		public int RecipeId { get; set; }

		public ICollection<ProductShoppingFinishDto> Products { get; set; }
	}
}
