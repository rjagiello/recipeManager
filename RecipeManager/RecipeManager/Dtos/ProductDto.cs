using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeManager.Dtos
{
	public class ProductDto
	{
		public string Name { get; set; }
		public ProductUnit Unit { get; set; }
		public double Count { get; set; }
	}

	public class ProductEditDto : ProductDto
	{
		public int Id { get; set; }
	}
}
