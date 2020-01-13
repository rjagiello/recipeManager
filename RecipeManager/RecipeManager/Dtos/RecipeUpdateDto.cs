using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeManager.Dtos
{
	public class RecipeUpdateDto
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public RecipeCategory Category { get; set; }
		public string Preparation { get; set; }
		public int Portions { get; set; }

		public ICollection<ProductDto> Products { get; set; }
	}
}
