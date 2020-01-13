using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeManager.Dtos
{
	public class ProductShoppingDto
	{
		public string Name { get; set; }
		public ProductUnit Unit { get; set; }
		public double Count { get; set; }
		public bool IsInFridge { get; set; }
	}

	public class ProductShoppingFinishDto : ProductShoppingDto
	{
		public double BoughtCount { get; set; }
		public double FridgeCount { get; set; }
	}
}
